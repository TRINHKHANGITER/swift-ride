using System.Net.Http.Json;
using Trip_Service.Application.Contracts;
using Trip_Service.Application.Services.Impl;

namespace Trip_Service.Infrastructure;

public sealed class MatchingPricingService(HttpClient httpClient) : IMatchingPricingService
{
    public async Task<QuoteResponseContract> GetQuoteAsync(QuoteRequestContract contract, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsJsonAsync("http://localhost:6666/quote", contract, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<QuoteResponseContract>(cancellationToken)
            ?? throw new InvalidOperationException("MatchingPricingService trả về dữ liệu rỗng.");
    }

    public async Task<MatchingResponseContract> ProcessMatchAsync(
        MatchingRequestContract contract,
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsJsonAsync("http://localhost:6666/matching", contract, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<MatchingResponseContract>(cancellationToken)
            ?? throw new InvalidOperationException("MatchingPricingService trả về dữ liệu rỗng.");
    }

    public async Task<DriverLocationContract?> GetDriverLocationAsync(
        Guid driverUuid,
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.GetAsync(
            $"http://localhost:6666/drivers/{driverUuid}/location", cancellationToken);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound) return null;
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<DriverLocationContract>(cancellationToken);
    }

    public async Task<DriverLocationContract> UpdateDriverLocationAsync(
        Guid driverUuid,
        UpdateDriverLocationContract request,
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PutAsJsonAsync(
            $"http://localhost:6666/drivers/{driverUuid}/location", request, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<DriverLocationContract>(cancellationToken)
            ?? throw new InvalidOperationException("MatchingPricingService trả về vị trí rỗng.");
    }

    public async Task SetDriverAvailabilityAsync(
        Guid driverUuid,
        bool isAvailable,
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PatchAsJsonAsync(
            $"http://localhost:6666/drivers/{driverUuid}/availability",
            new UpdateDriverAvailabilityContract(isAvailable),
            cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}
