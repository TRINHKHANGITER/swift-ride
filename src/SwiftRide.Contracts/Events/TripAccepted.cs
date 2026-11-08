namespace SwiftRide.Contracts.Events;

public sealed record TripAccepted(
    Guid TripUuid,
    Guid DriverUuid,
    DateTime AcceptedAt);
