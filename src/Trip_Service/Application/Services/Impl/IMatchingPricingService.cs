using Trip_Service.Application.Contracts;
using Trip_Service.Domain;

namespace Trip_Service.Application.Services.Impl;

public interface IMatchingPricingService
{
    Task<QuoteResponseContract> GetQuoteAsync(QuoteRequestContract request, CancellationToken cancellationToken = default);
    Task<MatchingResponseContract> ProcessMatchAsync(MatchingRequestContract request, CancellationToken cancellationToken = default);

    Task<MatchingResponseContract> FindAndNotifyNextDriverAsync(
        Trip trip,
        CancellationToken cancellationToken = default);
}
