namespace SwiftRide.Contracts.Payment;

// ! Used to send to Trip Service
public sealed record PaymentSettled(
    Guid TripUuid
);
