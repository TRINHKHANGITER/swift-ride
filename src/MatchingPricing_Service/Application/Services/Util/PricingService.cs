using MatchingPricing_Service.Application.Services.Impl;

namespace MatchingPricing_Service.Application.Services.Util;

public sealed class PricingService : IPricingService
{
    // Hard config 
    private static decimal PRICE_PER_KM = 5000;
    private static decimal PRICE_PER_MIN = 500;

    private static decimal _SURGE = 1;
    private static decimal _TOLL = 0;
    // 0 -> 1
    private static decimal _TAXT = 0;

    public decimal ProcessPrice(double distance, double time, double promotePercent)
    {
        if (promotePercent is < 0 or > 1)
        {
            throw new ArgumentOutOfRangeException(
                nameof(promotePercent),
                "Promotion percent must be between 0 and 1.");
        }

        var subtotal = (decimal)distance * PRICE_PER_KM + (decimal)time * PRICE_PER_MIN;

        return (subtotal * _SURGE + _TOLL) * (decimal)promotePercent * (1 + _TAXT);
    }
}
