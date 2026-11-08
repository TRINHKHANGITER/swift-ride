namespace SwiftRide.Contracts.Events;

public sealed record TripDroppedOff(
    Guid TripUuid,
    Guid DriverUuid,
    DateTime DroppedOffAt);
