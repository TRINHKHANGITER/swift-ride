using Trip_Service.Application.Contracts;
using Trip_Service.Application.Services.Impl;
using Trip_Service.Domain;
using Trip_Service.Infrastructure.Persistence.Impl;

namespace Trip_Service.Application.Services;

public sealed class TripMatchingService(
    IMatchingPricingService matchingPricingService,
    ITripService tripService,
    ISendNotificateService sendNotificateService,
    ITripDriverRejectionRepository rejectionRepository) : ITripMatchingService
{
    private const int MaxMatchingAttempts = 10;

    public async Task<MatchingResponseContract> FindAndNotifyNextDriverAsync(
        Trip trip,
        CancellationToken cancellationToken = default)
    {
        if (sendNotificateService.HasPendingOffer(trip.Uuid))
        {
            throw new InvalidOperationException("Chuyến đi đang chờ một tài xế khác phản hồi.");
        }

        var excludedDrivers = (await rejectionRepository.FindDriverUuidsByTripAsync(
                trip.Uuid,
                cancellationToken))
            .ToHashSet();

        for (var attempt = 0; attempt < MaxMatchingAttempts; attempt++)
        {
            var match = await matchingPricingService.ProcessMatchAsync(
                new MatchingRequestContract(
                    trip.Uuid,
                    new LocationContract(trip.PickupLatitude, trip.PickupLongitude),
                    excludedDrivers),
                cancellationToken);

            if (!match.IsSuccess || match.DriverUuid is not { } driverUuid)
            {
                break;
            }

            if (await tripService.GetActiveTripByDriverAsync(
                    driverUuid,
                    cancellationToken: cancellationToken) is not null)
            {
                excludedDrivers.Add(driverUuid);
                continue;
            }

            if (sendNotificateService.Notify(driverUuid, trip))
            {
                return match;
            }

            // The driver can still be marked online while their SSE stream is gone.
            // Skip that driver and continue matching another available driver.
            excludedDrivers.Add(driverUuid);
        }

        trip.DriverUuid = null;
        trip.Status = TripStatus.Failed;
        await tripService.UpdateTripAsync(trip, cancellationToken);

        throw new InvalidOperationException("Không còn tài xế phù hợp để nhận chuyến đi.");
    }
}
