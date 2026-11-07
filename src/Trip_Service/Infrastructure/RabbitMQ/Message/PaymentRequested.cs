namespace SwiftRide.Contracts.Payment;

// ! Used to send to Trip Service
public sealed record PaymentRequested(
    Guid TripUuid,
    Guid PaymentUuid,
    string QrUrl
);
