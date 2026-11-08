namespace MatchingPricing_Service.Application.Services.Pricing;

public sealed record PricingContext(
    decimal DistanceKilometers,
    decimal DurationMinutes,
    decimal PromotionMultiplier);
