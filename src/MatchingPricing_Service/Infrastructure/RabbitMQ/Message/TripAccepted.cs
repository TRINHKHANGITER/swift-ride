namespace MatchingPricing_Service.Infrastructure.RabbitMQ.Message;

public record TripAccepted(
    Guid TripUuid,
    Guid DriverUuid,
    DateTime AcceptedAt
);