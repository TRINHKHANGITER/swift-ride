using System.ComponentModel.DataAnnotations;

namespace Trip_Service.Application.Contracts.Trip;

public sealed class GeoLocationContract
{
    [Required, MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    [Range(-90, 90)]
    public double Latitude { get; set; }

    [Range(-180, 180)]
    public double Longitude { get; set; }
}

public sealed class CreateTripRequestContract
{
    [Required]
    public GeoLocationContract Pickup { get; set; } = null!;

    [Required]
    public GeoLocationContract Destination { get; set; } = null!;
}

public sealed class CreateTripLocationContract
{
    public required string Name { get; init; }
    public double Latitude { get; init; }
    public double Longitude { get; init; }
}

public sealed class CreateTripGeometryContract
{
    public required string Type { get; init; }
    public required double[][] Coordinates { get; init; }
}

public sealed class CreateTripResponseContract
{
    public Guid Uuid { get; init; }
    public Guid RiderUuid { get; init; }
    public required CreateTripLocationContract Pickup { get; init; }
    public required CreateTripLocationContract Destination { get; init; }
    public required CreateTripGeometryContract Geometry { get; init; }
    public required string Status { get; init; }
    public decimal? DistanceKM { get; init; }
    public decimal? DurationMin { get; init; }
    public decimal? Price { get; init; }
    public DateTime CreatedAt { get; init; }
}
