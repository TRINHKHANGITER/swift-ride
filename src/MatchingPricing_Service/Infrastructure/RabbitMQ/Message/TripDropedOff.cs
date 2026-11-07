namespace MatchingPricing_Service.Infrastructure.RabbitMQ.Message;

public record TripDropedOff(
    Guid DriverUuid
);