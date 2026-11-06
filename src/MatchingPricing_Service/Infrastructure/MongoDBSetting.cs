namespace MatchingPricing_Service.Infrastructure;

public sealed class MongoDBSetting
{
    public string ConnectionString { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
}