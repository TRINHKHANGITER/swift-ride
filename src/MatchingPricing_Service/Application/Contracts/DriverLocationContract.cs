namespace MatchingPricing_Service.Application.Contracts;

public sealed record DriverLocationResponseContract(
    Guid DriverUuid,
    double Latitude,
    double Longitude,
    bool IsOnline,
    bool IsAvailable,
    DateTime UpdatedAt
);
