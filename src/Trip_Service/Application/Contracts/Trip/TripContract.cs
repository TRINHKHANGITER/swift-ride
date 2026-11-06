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

public sealed class CreateTripRequest
{
    [Required]
    public GeoLocationContract Pickup { get; set; } = null!;

    [Required]
    public GeoLocationContract Destination { get; set; } = null!;
}
