using MatchingPricing_Service.Domain;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace MatchingPricing_Service.Infrastructure;

public class MongoDBContext
{
    public IMongoCollection<DriverLocation> DriverLocationCollection { get; }

    public MongoDBContext(
        IMongoClient mongoClient,
        IOptions<MongoDBSetting> options)
    {
         var setting = options.Value;

        var database = mongoClient.GetDatabase(setting.DatabaseName);

        DriverLocationCollection = database.GetCollection<DriverLocation>("driver_locations");
    }
}