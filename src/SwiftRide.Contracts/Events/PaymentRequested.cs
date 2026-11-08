namespace SwiftRide.Contracts.Events;

public sealed record PaymentRequested(
    Guid TripUuid,
    Guid PaymentUuid,
    string QrUrl);
