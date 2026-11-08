namespace SwiftRide.Contracts.Events;

public sealed record RideCompleted(
    Guid TripUuid,
    Guid RiderUuid,
    Guid DriverUuid,
    decimal TotalPrice);
