using MatchingPricing_Service.Application.Contracts;
using MatchingPricing_Service.Application.Services.Impl;
using MatchingPricing_Service.Domain;
using MatchingPricing_Service.Infrastructure.Persistence.Impl;

namespace MatchingPricing_Service.Application.Services;

public sealed class DriverLocationService(IDriverLocationRepository repository)
    : IDriverLocationService
{
    public async Task<DriverLocationResponse?> FindByDriverUuidAsync(
        Guid driverUuid,
        CancellationToken cancellationToken)
    {
        ValidateDriverUuid(driverUuid);

        var location = await repository.FindByDriverUuidAsync(driverUuid, cancellationToken);
        return location is null ? null : Map(location);
    }

    public async Task<DriverLocationResponse> UpdateLocationAsync(
        Guid driverUuid,
        double latitude,
        double longitude,
        CancellationToken cancellationToken)
    {
        ValidateDriverUuid(driverUuid);
        ValidateCoordinates(latitude, longitude);

        var location = await repository.UpdateLocationAsync(
            driverUuid,
            latitude,
            longitude,
            cancellationToken);

        return Map(location);
    }

    public async Task UpdateAvailabilityAsync(
        Guid driverUuid,
        bool isAvailable,
        CancellationToken cancellationToken)
    {
        ValidateDriverUuid(driverUuid);

        await repository.SetAvailabilityAsync(
            driverUuid,
            isAvailable,
            cancellationToken);
    }

    public async Task UpdateOnlineStatus(
        Guid driverUuid,
        bool isOnline,
        CancellationToken cancellationToken)
    {
        ValidateDriverUuid(driverUuid);

        await repository.SetOnlineStatusAsync(
            driverUuid,
            isOnline,
            cancellationToken);
    }

    private static void ValidateDriverUuid(Guid driverUuid)
    {
        if (driverUuid == Guid.Empty)
        {
            throw new ArgumentException("DriverUuid không hợp lệ.", nameof(driverUuid));
        }
    }

    private static void ValidateCoordinates(double latitude, double longitude)
    {
        if (!double.IsFinite(latitude) || latitude is < -90 or > 90)
        {
            throw new ArgumentOutOfRangeException(
                nameof(latitude),
                "Latitude phải nằm trong khoảng từ -90 đến 90.");
        }

        if (!double.IsFinite(longitude) || longitude is < -180 or > 180)
        {
            throw new ArgumentOutOfRangeException(
                nameof(longitude),
                "Longitude phải nằm trong khoảng từ -180 đến 180.");
        }
    }

    private static DriverLocationResponse Map(DriverLocation location) =>
        new(
            location.DriverUuid,
            location.Location.Coordinates.Latitude,
            location.Location.Coordinates.Longitude,
            location.IsOnline,
            location.IsAvailable,
            location.UpdatedAt);
}
