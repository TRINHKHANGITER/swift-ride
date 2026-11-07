using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using MatchingPricing_Service.Application.Contracts;
using MatchingPricing_Service.Domain;
using MatchingPricing_Service.Infrastructure.Persistence.Impl;
using Microsoft.AspNetCore.Mvc;

namespace MatchingPricing_Service.Api;

[ApiController]
[Route("driver")]
[Route("drivers")]
public sealed class DriverLocationController(IDriverLocationRepository repository) : ControllerBase
{
    // * Get driver location by Driver Uuid
    // ! Used
    [HttpGet("{driverUuid:guid}/location")]
    public async Task<ActionResult<DriverLocationResponse>> GetLocationAsync(
        Guid driverUuid,
        CancellationToken cancellationToken)
    {
        var profileUuid = GetDriverUuidFromHeader();
        if (profileUuid is not null && driverUuid != profileUuid)
        {
            throw new InvalidDataException("Thông tin tài xế không khớp");
        }


        var location = await repository.FindByDriverUuidAsync(driverUuid, cancellationToken);
        return location is null ? NotFound() : Ok(Map(location));
    }

    // * Update driver location
    // * Save driver location -> Send socket to Rider
    // ! Used in TripService
    [HttpPut("{driverUuid:guid}/location")]
    public async Task<ActionResult<DriverLocationResponse>> UpdateLocationAsync(
        Guid driverUuid,
        UpdateDriverLocationRequest request,
        CancellationToken cancellationToken)
    {
        if (!double.IsFinite(request.Latitude) || request.Latitude is < -90 or > 90 ||
            !double.IsFinite(request.Longitude) || request.Longitude is < -180 or > 180)
        {
            throw new ArgumentException("Tọa độ tài xế không hợp lệ.");
        }

        var location = await repository.UpdateLocationAsync(
            driverUuid, request.Latitude, request.Longitude, cancellationToken);
        return Ok(Map(location));
    }

    // * Update Driver Availability
    // ! Used in TripService
    [HttpPatch("{driverUuid:guid}/availability")]
    public async Task<IActionResult> UpdateAvailabilityAsync(
        Guid driverUuid,
        UpdateDriverAvailabilityRequest request,
        CancellationToken cancellationToken)
    {
        await repository.SetAvailabilityAsync(driverUuid, request.IsAvailable, cancellationToken);
        return NoContent();
    }

    // * Update Driver Is Online Status
    // ! Used
    [HttpPatch("{driverUuid:guid}/online")]
    public async Task<IActionResult> UpdateOnlineStatus(
        Guid driverUuid,
        UpdateDriverOnlineStatusRequest request,
        CancellationToken cancellationToken
    )
    {
        var profileUuid = GetDriverUuidFromHeader();
        if (profileUuid is not null && driverUuid != profileUuid)
        {
            throw new InvalidDataException("Thông tin tài xế không khớp");
        }

        await repository.SetOnlineStatusAsync(driverUuid, request.IsOnline, cancellationToken);
        return NoContent();
    }

    // ! Mapping response
    private static DriverLocationResponse Map(DriverLocation location)
    {
        return new DriverLocationResponse(
        location.DriverUuid,
        location.Location.Coordinates.Latitude,
        location.Location.Coordinates.Longitude,
        location.IsOnline,
        location.IsAvailable,
        location.UpdatedAt
        );
    }

    // ! Parse access_token
    private Guid? GetDriverUuidFromHeader()
    {
        var authHeader = Request.Headers.Authorization.ToString();
        if (!authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        var accessToken = authHeader["Bearer ".Length..].Trim();
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            return null;
        }

        var handler = new JwtSecurityTokenHandler();
        if (!handler.CanReadToken(accessToken))
        {
            return null;
        }

        var jwtToken = handler.ReadJwtToken(accessToken);
        if (!jwtToken.Payload.TryGetValue("profile", out var profileValue) ||
            profileValue is not JsonElement { ValueKind: JsonValueKind.Object } profile ||
            !profile.TryGetProperty("profileUuid", out var profileUuid))
        {
            return null;
        }

        return Guid.TryParse(profileUuid.ToString(), out var driverUuid)
            ? driverUuid
            : null;
    }
}
