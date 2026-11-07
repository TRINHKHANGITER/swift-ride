using MatchingPricing_Service.Application.Contracts;

namespace MatchingPricing_Service.Application.Services.Impl;

public interface IDriverLocationService
{
    Task<DriverLocationResponse?> FindByDriverUuidAsync(
        Guid driverUuid,
        CancellationToken cancellationToken);

    Task<DriverLocationResponse> UpdateLocationAsync(
        Guid driverUuid,
        UpdateDriverLocationRequest request,
        CancellationToken cancellationToken);

    Task UpdateAvailabilityAsync(
        Guid driverUuid,
        bool isAvailable,
        CancellationToken cancellationToken);

    Task UpdateOnlineStatus(
        Guid driverUuid,
        bool isOnline,
        CancellationToken cancellationToken);
}
