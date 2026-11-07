namespace SwiftRide.Contracts.Payment;

public sealed record RideCompleted(
    Guid TripUuid,
    Guid RiderUuid,
    Guid DriverUuid,
    decimal TotalPrice
);
