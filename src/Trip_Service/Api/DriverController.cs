using System.Text.Json;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Trip_Service.Application.Contracts;
using Trip_Service.Application.Contracts.Driver;
using Trip_Service.Application.Contracts.Trip;
using Trip_Service.Application.Services.Impl;
using Trip_Service.Domain;
using Trip_Service.Infrastructure.Persistence.Impl;
using Trip_Service.Infrastructure.RabbitMQ.Message;
using MatchingPricing_Service.Infrastructure.RabbitMQ.Message;

namespace Trip_Service.Api;

[ApiController]
[Authorize]
[Route("driver")]
public sealed class DriverController(
    IDriverRepository driverRepository,
    IAccountRepository accountRepository,
    IMatchingPricingService matchingPricingService,
    ITripService tripService,
    ITripMatchingService tripMatchingService,
    IAuthService authService,
    ISendNotificateService sendNotificateService,
    ITripDriverRejectionRepository rejectionRepository,
    // Rabbitmq 
    IPublishEndpoint publishEndpoint) : ControllerBase
{
    [HttpGet("{driverUuid:guid}")]
    public async Task<ActionResult<DriverDetailsResponse>> GetByUuidAsync(
        Guid driverUuid,
        CancellationToken cancellationToken)
    {
        var profile = await driverRepository.FindByUuidAsync(driverUuid, cancellationToken);
        if (profile is null)
        {
            return NotFound();
        }

        var account = await accountRepository.FindByUuidAsync(profile.AccountUuid, cancellationToken);
        if (account is null)
        {
            return NotFound();
        }

        var location = await matchingPricingService.GetDriverLocationAsync(driverUuid, cancellationToken);

        return Ok(new DriverDetailsResponse
        {
            DriverUuid = profile.DriverUuid,
            AccountUuid = profile.AccountUuid,
            Fullname = account.Fullname,
            Phone = account.Phone,
            LicenseNumber = profile.LicenseNumber,
            VehicleName = profile.VehicleName,
            LicensePlate = profile.LicensePlate,
            IsVerified = profile.IsVerified,
            Location = location is null
                ? null
                : new DriverLocationResponse
                {
                    Latitude = location.Latitude,
                    Longitude = location.Longitude,
                    IsOnline = location.IsOnline,
                    IsAvailable = location.IsAvailable,
                    UpdatedAt = location.UpdatedAt
                }
        });
    }

    [HttpPatch("{driverUuid:guid}/online")]
    public async Task<IActionResult> UpdateOnlineStatusAsync(
        [FromRoute] Guid driverUuid,
        DriverUpdateStatusContract contract,
        CancellationToken cancellationToken)
    {
        var session = await authService.AboutMeAsync(GetAccountUuid(), cancellationToken);
        if (session.Profile?.DriverUuid != driverUuid)
        {
            throw new UnauthorizedAccessException("Không thể cập nhật trạng thái của tài xế khác.");
        }

        await publishEndpoint.Publish(
            new DriverOnlineStatus(driverUuid, contract.IsOnline),
            cancellationToken);

        return Accepted();
    }

    // ! [Driver] - Accept trip
    [HttpPost("trip/{tripUuid:guid}/accept")]
    public async Task<ActionResult<Trip>> AcceptTripAsync(
        [FromRoute] Guid tripUuid,
        CancellationToken cancellationToken)
    {
        var (trip, driverUuid) = await GetPendingTripOfferAsync(tripUuid, cancellationToken);
        if (trip.Status != TripStatus.Requested)
        {
            throw new InvalidOperationException("Chuyến đi không còn chờ tài xế xác nhận.");
        }

        if (await tripService.GetActiveTripByDriverAsync(
                driverUuid,
                trip.Uuid,
                cancellationToken) is not null)
        {
            throw new InvalidOperationException("Bạn đang có một chuyến đi khác chưa hoàn thành.");
        }

        trip.DriverUuid = driverUuid;
        trip.Status = TripStatus.DriverAccepted;

        await tripService.UpdateTripAsync(trip, cancellationToken);

        sendNotificateService.RemovePendingOffer(trip.Uuid);

        // * Send to rabbitmq
        // await matchingPricingService.SetDriverAvailabilityAsync(
        //     driverUuid,
        //     false,
        //     cancellationToken);

        await publishEndpoint.Publish(new TripAccepted(trip.Uuid, driverUuid, DateTime.UtcNow), cancellationToken);

        return Ok(trip);
    }

    // ! [Driver] - Reject trip
    [HttpPost("trip/{tripUuid:guid}/reject")]
    public async Task<ActionResult<Trip>> RejectTripAsync(
        [FromRoute] Guid tripUuid,
        CancellationToken cancellationToken)
    {
        var (trip, driverUuid) = await GetPendingTripOfferAsync(tripUuid, cancellationToken);
        if (trip.Status != TripStatus.Requested)
        {
            throw new InvalidOperationException("Chuyến đi không còn chờ tài xế xác nhận.");
        }

        await rejectionRepository.AddAsync(new TripDriverRejection
        {
            Uuid = Guid.NewGuid(),
            TripUuid = trip.Uuid,
            DriverUuid = driverUuid,
            RejectedAt = DateTime.UtcNow
        }, cancellationToken);

        sendNotificateService.RemovePendingOffer(trip.Uuid);
        await tripMatchingService.FindAndNotifyNextDriverAsync(trip, cancellationToken);
        return Ok(trip);
    }

    // ! [Driver] - Pick up rider
    [HttpPost("trip/{tripUuid:guid}/pickup")]
    public async Task<ActionResult<TripDetailsResponse>> PickupTripAsync(
        [FromRoute] Guid tripUuid,
        CancellationToken cancellationToken)
    {
        var trip = await GetAssignedDriverTripAsync(tripUuid, cancellationToken);
        if (trip.Status is not (TripStatus.DriverAccepted or TripStatus.EnRoute))
        {
            throw new InvalidOperationException("Chuyến đi chưa sẵn sàng đón khách.");
        }

        trip.Status = TripStatus.PickedUp;
        var location = await matchingPricingService.UpdateDriverLocationAsync(
            trip.DriverUuid!.Value,
            new UpdateDriverLocationContract(trip.PickupLatitude, trip.PickupLongitude),
            cancellationToken);
        await tripService.UpdateTripAsync(trip, cancellationToken);
        return Ok(await MapDetailsAsync(trip, cancellationToken, location));
    }

    // ! [Driver] - Drop off rider
    [HttpPost("trip/{tripUuid:guid}/dropoff")]
    public async Task<ActionResult<TripDetailsResponse>> DropoffTripAsync(
        [FromRoute] Guid tripUuid,
        CancellationToken cancellationToken)
    {
        var trip = await GetAssignedDriverTripAsync(tripUuid, cancellationToken);
        if (trip.Status != TripStatus.PickedUp)
        {
            throw new InvalidOperationException("Cần đón khách trước khi kết thúc chuyến.");
        }

        trip.Status = TripStatus.DroppedOff;
        var location = await matchingPricingService.UpdateDriverLocationAsync(
            trip.DriverUuid!.Value,
            new UpdateDriverLocationContract(trip.DestinationLatitude, trip.DestinationLongitude),
            cancellationToken);

        // RabbitMQ
        await publishEndpoint.Publish(new TripDropoffed(trip.DriverUuid.Value), cancellationToken);
        // await matchingPricingService.SetDriverAvailabilityAsync(
        //     trip.DriverUuid.Value,
        //     true,
        //     cancellationToken);
        
        await tripService.UpdateTripAsync(trip, cancellationToken);
        return Ok(await MapDetailsAsync(trip, cancellationToken, location));
    }

    private async Task<Trip> GetAssignedDriverTripAsync(
        Guid tripUuid,
        CancellationToken cancellationToken)
    {
        var trip = await tripService.GetTripByUuidAsync(tripUuid, cancellationToken);
        var session = await authService.AboutMeAsync(GetAccountUuid(), cancellationToken);
        if (session.Profile?.DriverUuid != trip.DriverUuid)
        {
            throw new UnauthorizedAccessException("Chuyến đi không được giao cho tài xế hiện tại.");
        }

        return trip;
    }

    private async Task<(Trip Trip, Guid DriverUuid)> GetPendingTripOfferAsync(
        Guid tripUuid,
        CancellationToken cancellationToken)
    {
        var session = await authService.AboutMeAsync(GetAccountUuid(), cancellationToken);
        var driverUuid = session.Profile?.DriverUuid
            ?? throw new UnauthorizedAccessException("Tài khoản không phải là tài xế.");

        if (!sendNotificateService.IsPendingOffer(driverUuid, tripUuid))
        {
            throw new UnauthorizedAccessException("Yêu cầu chuyến đi không được gửi tới tài xế hiện tại.");
        }

        var trip = await tripService.GetTripByUuidAsync(tripUuid, cancellationToken);
        return (trip, driverUuid);
    }

    private Guid GetAccountUuid()
    {
        if (Guid.TryParse(User.FindFirst("uuid")?.Value, out var accountUuid))
        {
            return accountUuid;
        }

        throw new UnauthorizedAccessException("Token không chứa Uuid hợp lệ.");
    }

    private async Task<TripDetailsResponse> MapDetailsAsync(
        Trip trip,
        CancellationToken cancellationToken,
        DriverLocationContract? driverLocation = null)
    {
        TripDriverView? driverView = null;
        if (trip.DriverUuid is { } driverUuid)
        {
            driverLocation ??= await matchingPricingService.GetDriverLocationAsync(driverUuid, cancellationToken);
            var profile = await driverRepository.FindByUuidAsync(driverUuid, cancellationToken);
            if (profile is not null)
            {
                var account = await accountRepository.FindByUuidAsync(profile.AccountUuid, cancellationToken);
                if (account is not null)
                {
                    driverView = new TripDriverView
                    {
                        Uuid = driverUuid,
                        Fullname = account.Fullname,
                        Phone = account.Phone,
                        VehicleName = profile.VehicleName,
                        LicensePlate = profile.LicensePlate
                    };
                }
            }
        }

        double[][]? coordinates = null;
        if (!string.IsNullOrWhiteSpace(trip.RouteGeometryJson))
        {
            coordinates = JsonSerializer.Deserialize<double[][]>(trip.RouteGeometryJson);
        }

        return new TripDetailsResponse
        {
            Uuid = trip.Uuid,
            RiderUuid = trip.RiderUuid,
            DriverUuid = trip.DriverUuid,
            PaymentUuid = trip.PaymentUuid,
            PickupName = trip.PickupName,
            PickupLatitude = trip.PickupLatitude,
            PickupLongitude = trip.PickupLongitude,
            DestinationName = trip.DestinationName,
            DestinationLatitude = trip.DestinationLatitude,
            DestinationLongitude = trip.DestinationLongitude,
            DistanceKM = trip.DistanceKM,
            DurationMin = trip.DurationMin,
            Price = trip.Price,
            Status = trip.Status.ToString(),
            DriverLatitude = driverLocation?.Latitude,
            DriverLongitude = driverLocation?.Longitude,
            Driver = driverView,
            Geometry = coordinates is null ? null : new TripRouteView { Coordinates = coordinates },
            CreatedAt = trip.CreatedAt,
            UpdatedAt = trip.UpdatedAt
        };
    }
}
