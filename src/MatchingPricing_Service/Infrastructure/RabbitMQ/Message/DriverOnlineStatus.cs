namespace MatchingPricing_Service.Infrastructure.RabbitMQ.Message;

public sealed record DriverOnlineStatus(
    Guid DriverUuid,
    bool IsOnline
);
