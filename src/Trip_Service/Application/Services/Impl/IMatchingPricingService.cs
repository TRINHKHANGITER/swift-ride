using Trip_Service.Application.Contracts;

namespace Trip_Service.Application.Services.Impl;

public interface IMatchingPricingService
{
    Task<QuoteResponseContract> GetQuoteAsync(QuoteRequestContract request, CancellationToken cancellationToken = default);
    Task<MatchingResponseContract> ProcessMatchAsync(MatchingRequestContract request, CancellationToken cancellationToken = default);
    Task<DriverLocationContract?> GetDriverLocationAsync(Guid driverUuid, CancellationToken cancellationToken = default);
    Task<DriverLocationContract> UpdateDriverLocationAsync(Guid driverUuid, UpdateDriverLocationContract request, CancellationToken cancellationToken = default);
    Task SetDriverAvailabilityAsync(Guid driverUuid, bool isAvailable, CancellationToken cancellationToken = default);
}
