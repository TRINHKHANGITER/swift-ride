namespace SwiftRide.Contracts.Events;

public sealed record TripDriverLocationUpdated(
    Guid TripUuid,
    Guid DriverUuid,
    double Longitude,
    double Latitude,
    DateTime UpdatedAt);
