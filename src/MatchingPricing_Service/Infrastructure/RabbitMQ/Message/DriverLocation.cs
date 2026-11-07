namespace MatchingPricing_Service.Infrastructure.RabbitMQ.Message;

public sealed record DriverLocation(
    Guid DriverUuid,
    double Longitude,
    double Latitude
);