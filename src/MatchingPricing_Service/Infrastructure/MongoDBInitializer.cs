using MatchingPricing_Service.Domain;
using MongoDB.Driver;

namespace MatchingPricing_Service.Infrastructure;

public sealed class MongoDBInitializer
{
    private readonly MongoDBContext _context;

    public MongoDBInitializer(MongoDBContext context)
    {
        this._context = context;
    }

    public async Task InitializeAsync(
        CancellationToken cancellationToken = default)
    {
        var driverIdIndex =
            new CreateIndexModel<DriverLocation>(
                Builders<DriverLocation>
                    .IndexKeys
                    .Ascending(driver => driver.DriverUuid),
                new CreateIndexOptions
                {
                    Name = "index_driver_id",
                    Unique = true
                });

        var locationIndex =
            new CreateIndexModel<DriverLocation>(
                Builders<DriverLocation>
                    .IndexKeys
                    .Geo2DSphere(
                        driver => driver.Location),
                new CreateIndexOptions
                {
                    Name = "index_location_2dsphere"
                });

        await _context
            .DriverLocationCollection
            .Indexes
            .CreateManyAsync(
                [driverIdIndex, locationIndex],
                cancellationToken);
    }
}
