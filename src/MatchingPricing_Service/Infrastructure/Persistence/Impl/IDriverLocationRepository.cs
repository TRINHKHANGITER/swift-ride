using MatchingPricing_Service.Application.Contracts;
using MatchingPricing_Service.Domain;

namespace MatchingPricing_Service.Infrastructure.Persistence.Impl;

public interface IDriverLocationRepository
{
    Task<DriverLocation?> FindByDriverUuidAsync(Guid driverUuid, CancellationToken cancellationToken);
    Task<DriverLocation> UpdateLocationAsync(Guid driverUuid, double latitude, double longitude, CancellationToken cancellationToken);
    Task SetAvailabilityAsync(Guid driverUuid, bool isAvailable, CancellationToken cancellationToken);
    Task SetOnlineStatusAsync(Guid driverUuid, bool isOnline, CancellationToken cancellationToken);

    Task<DriverLocation?> FindNearestAvailableAsync(
        LocationContract location,
        IReadOnlyCollection<Guid> excludeDriverUuids,
        double maxDistanceMeters,
        CancellationToken cancellationToken
    );
}
