using System.ComponentModel.DataAnnotations;

namespace MatchingPricing_Service.Application.Contracts;

public sealed class QuoteRequestContract
{
    [Required]
    public LocationContract Pickup { get; set; } = null!;
    [Required]
    public LocationContract Destination { get; set; } = null!;
}

public sealed class QuoteResponseContract
{
    public decimal Price { get; set; }
    public double DistanceKM { get; set; }
    public double DurationMin { get; set; }
    public OSRMRouteContract Route { get; set; } = null!;
}