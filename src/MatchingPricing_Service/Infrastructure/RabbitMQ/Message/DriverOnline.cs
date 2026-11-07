namespace MatchingPricing_Service.Infrastructure.RabbitMQ.Message;

public sealed record DriverOnline(
    Guid DriverUuid,
    bool IsOnline
);
