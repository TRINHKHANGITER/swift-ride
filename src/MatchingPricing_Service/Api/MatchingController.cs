using MatchingPricing_Service.Application.Contracts;
using MatchingPricing_Service.Application.Services.Impl;
using Microsoft.AspNetCore.Mvc;

namespace MatchingPricing_Service.Api;

[ApiController]
[Route("matching")]
public sealed class MatchingController(IMatchingService matchingService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<MatchingResponseContract>> MatchAsync(
        [FromBody] MatchingRequestContract contract,
        CancellationToken cancellationToken)
    {
        var result = await matchingService.MatchAsync(contract, cancellationToken);
        return Ok(result);
    }
}
