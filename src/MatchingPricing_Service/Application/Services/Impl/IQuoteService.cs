using MatchingPricing_Service.Application.Contracts;

namespace MatchingPricing_Service.Application.Services.Impl;

public interface IQuoteService
{
    Task<QuoteResponseContract> RequestQuoteAsync(QuoteRequestContract contract, CancellationToken cancellationToken);
}