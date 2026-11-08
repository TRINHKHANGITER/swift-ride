using MatchingPricing_Service.Application.Services.Impl;
using MatchingPricing_Service.Domain;
using Microsoft.Extensions.Options;

namespace MatchingPricing_Service.Application.Services.Pricing;

public sealed class RainPricingStrategy(IOptions<PricingOptions> options) : IPricingStrategy
{
    private readonly PricingOptions _options = options.Value;

    public WeatherCondition WeatherCondition => WeatherCondition.Rain;
    public decimal Multiplier => _options.RainMultiplier;

    public decimal Calculate(PricingContext context)
    {
        var subtotal =
            context.DistanceKilometers * _options.PricePerKilometer +
            context.DurationMinutes * _options.PricePerMinute;

        return (subtotal * Multiplier + _options.Toll) *
               context.PromotionMultiplier *
               (1 + _options.TaxRate);
    }
}
