namespace Trip_Service.Application.Contracts.Driver;

public sealed class DriverDetailsResponse
{
    public Guid DriverUuid { get; init; }
    public Guid AccountUuid { get; init; }
    public required string Fullname { get; init; }
    public required string Phone { get; init; }
    public required string LicenseNumber { get; init; }
    public required string VehicleName { get; init; }
    public required string LicensePlate { get; init; }
    public bool IsVerified { get; init; }
    public DriverLocationResponse? Location { get; init; }
}

public sealed class DriverLocationResponse
{
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public bool IsOnline { get; init; }
    public bool IsAvailable { get; init; }
    public DateTime UpdatedAt { get; init; }
}
