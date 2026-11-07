using Microsoft.EntityFrameworkCore;
using Trip_Service.Domain;
using Trip_Service.Infrastructure.Persistence.Impl;

namespace Trip_Service.Infrastructure.Persistence.Repositories;

public sealed class TripDriverRejectionRepository(TripDbContext dbContext)
    : ITripDriverRejectionRepository
{
    public async Task<IReadOnlyList<Guid>> FindDriverUuidsByTripAsync(
        Guid tripUuid,
        CancellationToken cancellationToken = default) =>
        await dbContext.TripDriverRejections
            .AsNoTracking()
            .Where(rejection => rejection.TripUuid == tripUuid)
            .Select(rejection => rejection.DriverUuid)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(
        TripDriverRejection rejection,
        CancellationToken cancellationToken = default)
    {
        var exists = await dbContext.TripDriverRejections.AnyAsync(
            item => item.TripUuid == rejection.TripUuid &&
                    item.DriverUuid == rejection.DriverUuid,
            cancellationToken);
        if (exists) return;

        dbContext.TripDriverRejections.Add(rejection);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
