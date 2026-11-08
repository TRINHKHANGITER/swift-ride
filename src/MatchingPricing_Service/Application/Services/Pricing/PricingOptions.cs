namespace MatchingPricing_Service.Application.Services.Pricing;

public sealed class PricingOptions
{
    public const string SectionName = "Pricing";

    public decimal PricePerKilometer { get; set; }
    public decimal PricePerMinute { get; set; }
    public decimal Toll { get; set; }
    public decimal TaxRate { get; set; }
    public decimal NormalMultiplier { get; set; }
    public decimal RainMultiplier { get; set; }
}
