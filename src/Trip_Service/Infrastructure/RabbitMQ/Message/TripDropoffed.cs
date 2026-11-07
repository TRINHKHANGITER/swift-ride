namespace Trip_Service.Infrastructure.RabbitMQ.Message;

public record TripDropoffed(
    Guid DriverUuid
);