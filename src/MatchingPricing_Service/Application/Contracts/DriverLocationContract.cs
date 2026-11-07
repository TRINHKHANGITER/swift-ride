namespace MatchingPricing_Service.Application.Contracts;

public sealed record UpdateDriverLocationRequest(double Latitude, double Longitude);

public sealed record DriverLocationResponse(
    Guid DriverUuid,
    double Latitude,
    double Longitude,
    bool IsOnline,
    bool IsAvailable,
    DateTime UpdatedAt
);
