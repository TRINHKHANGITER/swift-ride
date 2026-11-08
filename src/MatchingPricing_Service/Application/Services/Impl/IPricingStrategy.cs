using MatchingPricing_Service.Application.Services.Pricing;
using MatchingPricing_Service.Domain;

namespace MatchingPricing_Service.Application.Services.Impl;

public interface IPricingStrategy
{
    WeatherCondition WeatherCondition { get; }
    decimal Multiplier { get; }
    decimal Calculate(PricingContext context);
}
