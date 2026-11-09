using MatchingPricing_Service.Application.Contracts;
using MatchingPricing_Service.Application.Services.Impl;
using Microsoft.AspNetCore.Mvc;

namespace MatchingPricing_Service.Api;

[ApiController]
[Route("driver")]
public sealed class DriverLocationController(IDriverLocationService service) : ControllerBase
{
    // * Get driver location by Driver Uuid
    // ! Used in trip service
    [HttpGet("{driverUuid:guid}/location")]
    public async Task<ActionResult<DriverLocationResponseContract>> GetLocationAsync(
        Guid driverUuid,
        CancellationToken cancellationToken)
    {
        var location = await service.FindByDriverUuidAsync(driverUuid, cancellationToken);
        return location is null ? NotFound() : Ok(location);
    }
}
