using System.Net.Http.Json;
using Trip_Service.Application.Contracts;
using Trip_Service.Application.Services.Impl;
using Trip_Service.Domain;
using Trip_Service.Infrastructure.Persistence.Impl;

namespace Trip_Service.Application.Services.Client;

public sealed class MatchingPricingService(
    HttpClient httpClient,
    ITripService tripService,
    ISendNotificateService sendNotificateService,
    ITripDriverRejectionRepository rejectionRepository)
    : IMatchingPricingService, ITripMatchingService
{
    private const int MaxMatchingAttempts = 10;

    public async Task<QuoteResponseContract> GetQuoteAsync(QuoteRequestContract contract, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsJsonAsync("mp/quote", contract, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<QuoteResponseContract>(cancellationToken)
            ?? throw new InvalidOperationException("MatchingPricingService trả về dữ liệu rỗng.");
    }

    public async Task<MatchingResponseContract> ProcessMatchAsync(
        MatchingRequestContract contract,
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsJsonAsync("mp/matching", contract, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<MatchingResponseContract>(cancellationToken)
            ?? throw new InvalidOperationException("MatchingPricingService trả về dữ liệu rỗng.");
    }

    public async Task<DriverLocationContract?> GetDriverLocationAsync(
        Guid driverUuid,
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.GetAsync(
            $"mp/drivers/{driverUuid}/location", cancellationToken);
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
            $"mp/drivers/{driverUuid}/location", request, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<DriverLocationContract>(cancellationToken)
            ?? throw new InvalidOperationException("MatchingPricingService trả về vị trí rỗng.");
    }

    public async Task<MatchingResponseContract> FindAndNotifyNextDriverAsync(
        Trip trip,
        CancellationToken cancellationToken = default)
    {
        if (sendNotificateService.HasPendingOffer(trip.Uuid))
        {
            throw new InvalidOperationException("Chuyến đi đang chờ một tài xế khác phản hồi.");
        }

        var excludedDrivers = (await rejectionRepository.FindDriverUuidsByTripAsync(
                trip.Uuid,
                cancellationToken))
            .ToHashSet();

        for (var attempt = 0; attempt < MaxMatchingAttempts; attempt++)
        {
            var match = await ProcessMatchAsync(
                new MatchingRequestContract(
                    trip.Uuid,
                    new LocationContract(trip.PickupLatitude, trip.PickupLongitude),
                    excludedDrivers),
                cancellationToken);

            if (!match.IsSuccess || match.DriverUuid is not { } driverUuid)
            {
                break;
            }

            if (await tripService.GetActiveTripByDriverAsync(
                    driverUuid,
                    cancellationToken: cancellationToken) is not null)
            {
                excludedDrivers.Add(driverUuid);
                continue;
            }

            if (sendNotificateService.Notify(driverUuid, trip))
            {
                return match;
            }

            excludedDrivers.Add(driverUuid);
        }

        // trip.DriverUuid = null;
        // trip.Status = TripStatus.Failed;
        // await tripService.UpdateTripAsync(trip, cancellationToken);

        throw new InvalidOperationException("Không còn tài xế phù hợp để nhận chuyến đi.");
    }
}
