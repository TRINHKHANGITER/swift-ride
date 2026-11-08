namespace SwiftRide.Contracts.Events;

public sealed record DriverOnline(
    Guid DriverUuid,
    bool IsOnline);
