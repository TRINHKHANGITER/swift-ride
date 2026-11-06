using MatchingPricing_Service.Application.Contracts;

namespace MatchingPricing_Service.Application.Services.Impl;

public interface IMatchingService
{
    Task<MatchingResponseContract> MatchAsync(
        MatchingRequestContract contract,
        CancellationToken cancellationToken);
}
