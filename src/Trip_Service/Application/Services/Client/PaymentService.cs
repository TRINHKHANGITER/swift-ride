using System.Net.Http.Json;
using Trip_Service.Application.Contracts.Payment;
using Trip_Service.Application.Services.Impl;

namespace Trip_Service.Application.Services.Client;

public sealed class PaymentService(HttpClient httpClient) : IPaymentService
{
    public async Task<IReadOnlyList<PaymentResponseContract>> GetPaymentsByRiderUuidAsync(
        Guid riderUuid,
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.GetAsync(
            $"pm/payment/rider/{riderUuid}",
            cancellationToken);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<List<PaymentResponseContract>>(cancellationToken)
            ?? [];
    }

    public async Task<IReadOnlyList<PaymentResponseContract>> GetPaymentsByDriverUuidAsync(
        Guid driverUuid,
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.GetAsync(
            $"pm/payment/driver/{driverUuid}",
            cancellationToken);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<List<PaymentResponseContract>>(cancellationToken)
            ?? [];
    }
}
