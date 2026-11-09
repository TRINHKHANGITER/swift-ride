namespace Trip_Service.Application.Contracts.Driver;

public sealed record DriverLocationResponseContract(
    Guid DriverUuid,
    double Latitude,
    double Longitude,
    bool IsOnline,
    bool IsAvailable,
    DateTime UpdatedAt
);