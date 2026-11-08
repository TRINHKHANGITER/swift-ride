using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MatchingPricing_Service.Domain;

namespace MatchingPricing_Service.Application.Contracts;

public sealed class QuoteRequestContract
{
    [Required]
    public LocationContract Pickup { get; set; } = null!;
    [Required]
    public LocationContract Destination { get; set; } = null!;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public WeatherCondition WeatherCondition { get; set; } = WeatherCondition.Normal;
}

public sealed class QuoteResponseContract
{
    public decimal Price { get; set; }
    public double DistanceKM { get; set; }
    public double DurationMin { get; set; }
    public OSRMRouteContract Route { get; set; } = null!;
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public WeatherCondition WeatherCondition { get; set; }
    public decimal Multiplier { get; set; }
}
