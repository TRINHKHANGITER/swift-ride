namespace Trip_Service.Application.Contracts.Trip;

public sealed class TripDetailsResponse
{
    public Guid Uuid { get; init; }
    public Guid RiderUuid { get; init; }
    public Guid? DriverUuid { get; init; }
    public Guid? PaymentUuid { get; init; }
    public required string PickupName { get; init; }
    public double PickupLatitude { get; init; }
    public double PickupLongitude { get; init; }
    public required string DestinationName { get; init; }
    public double DestinationLatitude { get; init; }
    public double DestinationLongitude { get; init; }
    public decimal? DistanceKM { get; init; }
    public decimal? DurationMin { get; init; }
    public decimal? Price { get; init; }
    public required string Status { get; init; }
    public double? DriverLatitude { get; init; }
    public double? DriverLongitude { get; init; }
    public TripDriverView? Driver { get; init; }
    public TripRouteView? Geometry { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}

public sealed class TripDriverView
{
    public Guid Uuid { get; init; }
    public required string Fullname { get; init; }
    public required string Phone { get; init; }
    public required string VehicleName { get; init; }
    public required string LicensePlate { get; init; }
}

public sealed class TripRouteView
{
    public string Type { get; init; } = "LineString";
    public required double[][] Coordinates { get; init; }
}

public sealed class UpdateDriverLocationRequest
{
    public double Latitude { get; init; }
    public double Longitude { get; init; }
}
