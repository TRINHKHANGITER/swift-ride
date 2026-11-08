using MatchingPricing_Service.Domain;

namespace MatchingPricing_Service.Application.Services.Impl;

public interface IPricingService
{
    decimal ProcessPrice(
        double distanceKilometers,
        double durationMinutes,
        WeatherCondition weatherCondition,
        decimal promotionMultiplier = 1);

    decimal GetMultiplier(WeatherCondition weatherCondition);
}
