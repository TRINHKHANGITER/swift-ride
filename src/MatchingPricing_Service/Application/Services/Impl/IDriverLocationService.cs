using MatchingPricing_Service.Application.Contracts;

namespace MatchingPricing_Service.Application.Services.Impl;

public interface IDriverLocationService
{
    Task<DriverLocationResponseContract?> FindByDriverUuidAsync(
        Guid driverUuid,
        CancellationToken cancellationToken);

    Task<DriverLocationResponseContract> UpdateLocationAsync(
        Guid driverUuid,
        double latitude,
        double longitude,
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
