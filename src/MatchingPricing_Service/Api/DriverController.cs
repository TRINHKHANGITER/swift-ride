using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using MatchingPricing_Service.Application.Contracts;
using MatchingPricing_Service.Application.Services.Impl;
using Microsoft.AspNetCore.Mvc;

namespace MatchingPricing_Service.Api;

[ApiController]
[Route("driver")]
[Route("drivers")]
public sealed class DriverLocationController(IDriverLocationService service) : ControllerBase
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


        var location = await service.FindByDriverUuidAsync(driverUuid, cancellationToken);
        return location is null ? NotFound() : Ok(location);
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
        var location = await service.UpdateLocationAsync(
            driverUuid, request, cancellationToken);
        return Ok(location);
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
