namespace MatchingPricing_Service.Application.Services.Impl;

public interface IPricingService
{
    public decimal ProcessPrice (double distance, double time, double promotePercent);
}