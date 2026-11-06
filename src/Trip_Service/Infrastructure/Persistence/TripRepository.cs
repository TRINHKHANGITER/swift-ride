using Microsoft.EntityFrameworkCore;
using Trip_Service.Domain;
using Trip_Service.Infrastructure.Persistence.Impl;

namespace Trip_Service.Infrastructure.Persistence.Repositories;



public sealed class TripRepository(TripDbContext dbContext) : ITripRepository
{
    public Task<Trip?> FindByUuidAsync(Guid uuid, CancellationToken cancellationToken = default) =>
        dbContext.Trips.AsNoTracking()
            .SingleOrDefaultAsync(trip => trip.Uuid == uuid, cancellationToken);

    public Task<Trip?> FindActiveByRiderAsync(
        Guid riderUuid,
        Guid? excludeTripUuid = null,
        CancellationToken cancellationToken = default) =>
        dbContext.Trips.AsNoTracking()
            .OrderByDescending(trip => trip.UpdatedAt)
            .FirstOrDefaultAsync(trip =>
                trip.RiderUuid == riderUuid &&
                trip.Uuid != excludeTripUuid &&
                trip.DriverUuid != null &&
                trip.Status >= TripStatus.Requested &&
                trip.Status <= TripStatus.PickedUp,
                cancellationToken);

    public Task<Trip?> FindActiveByDriverAsync(
        Guid driverUuid,
        Guid? excludeTripUuid = null,
        CancellationToken cancellationToken = default) =>
        dbContext.Trips.AsNoTracking()
            .OrderByDescending(trip => trip.UpdatedAt)
            .FirstOrDefaultAsync(trip =>
                trip.DriverUuid == driverUuid &&
                trip.Uuid != excludeTripUuid &&
                trip.Status >= TripStatus.Requested &&
                trip.Status <= TripStatus.PickedUp,
                cancellationToken);

    public async Task<IReadOnlyList<Trip>> FindHistoryByRiderAsync(
        Guid riderUuid,
        CancellationToken cancellationToken = default) =>
        await dbContext.Trips.AsNoTracking()
            .Where(trip =>
                trip.RiderUuid == riderUuid &&
                (trip.Status == TripStatus.DroppedOff ||
                 trip.Status == TripStatus.Paid ||
                 trip.Status == TripStatus.Cancelled ||
                 trip.Status == TripStatus.Failed))
            .OrderByDescending(trip => trip.UpdatedAt)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(Trip trip, CancellationToken cancellationToken = default)
    {
        dbContext.Trips.Add(trip);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
    public async Task UpdateAsync(Trip trip, CancellationToken cancellationToken = default)
    {
        dbContext.Trips.Update(trip);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
