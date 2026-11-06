using MatchingPricing_Service.Application.Contracts;
using MatchingPricing_Service.Domain;
using MatchingPricing_Service.Infrastructure.Persistence.Impl;
using Microsoft.AspNetCore.Mvc;

namespace MatchingPricing_Service.Api;

[ApiController]
[Route("drivers")]
public sealed class DriverLocationController(IDriverLocationRepository repository) : ControllerBase
{
    [HttpGet("{driverUuid:guid}/location")]
    public async Task<ActionResult<DriverLocationResponse>> GetLocationAsync(
        Guid driverUuid,
        CancellationToken cancellationToken)
    {
        var location = await repository.FindByDriverUuidAsync(driverUuid, cancellationToken);
        return location is null ? NotFound() : Ok(Map(location));
    }

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

    [HttpPatch("{driverUuid:guid}/availability")]
    public async Task<IActionResult> UpdateAvailabilityAsync(
        Guid driverUuid,
        UpdateDriverAvailabilityRequest request,
        CancellationToken cancellationToken)
    {
        await repository.SetAvailabilityAsync(driverUuid, request.IsAvailable, cancellationToken);
        return NoContent();
    }

    private static DriverLocationResponse Map(DriverLocation location) => new(
        location.DriverUuid,
        location.Location.Coordinates.Latitude,
        location.Location.Coordinates.Longitude,
        location.IsOnline,
        location.IsAvailable,
        location.UpdatedAt);
}
