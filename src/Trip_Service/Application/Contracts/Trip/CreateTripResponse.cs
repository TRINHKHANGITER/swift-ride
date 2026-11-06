namespace Trip_Service.Application.Contracts.Trips;

public sealed class LocationView
{
    public required string Name { get; init; }

    public double Latitude { get; init; }

    public double Longitude { get; init; }
}

public sealed class GeometryView
{
    public required string Type { get; init; }

    public required double[][] Coordinates { get; init; }
}

public sealed class CreateTripResponse
{
    public Guid Uuid { get; init; }

    public Guid RiderUuid { get; init; }

    public required LocationView Pickup { get; init; }

    public required LocationView Destination { get; init; }

    public required GeometryView Geometry { get; init; }

    public required string Status { get; init; }

    public decimal? DistanceKM { get; init; }

    public decimal? DurationMin { get; init; }

    public decimal? Price { get; init; }

    public DateTime CreatedAt { get; init; }
}
