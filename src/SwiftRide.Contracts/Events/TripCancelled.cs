namespace SwiftRide.Contracts.Events;

public sealed record TripCancelled(
    Guid TripUuid,
    Guid DriverUuid,
    DateTime CancelledAt);
