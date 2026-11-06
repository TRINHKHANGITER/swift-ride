using Microsoft.EntityFrameworkCore;
using Trip_Service.Domain;
using Trip_Service.Infrastructure.Persistence.Impl;

namespace Trip_Service.Infrastructure.Persistence.Repositories;



public sealed class DriverRepository(TripDbContext dbContext) : IDriverRepository
{
    public Task<DriverProfile?> FindByUuidAsync(Guid driverUuid, CancellationToken cancellationToken = default) =>
        dbContext.DriverProfiles.AsNoTracking()
            .SingleOrDefaultAsync(profile => profile.DriverUuid == driverUuid, cancellationToken);

    public Task<DriverProfile?> FindByAccountUuidAsync(Guid accountUuid, CancellationToken cancellationToken = default) =>
        dbContext.DriverProfiles.AsNoTracking()
            .SingleOrDefaultAsync(profile => profile.AccountUuid == accountUuid, cancellationToken);

    public Task<bool> HasDuplicateAsync(
        string licenseNumber,
        string licensePlate,
        CancellationToken cancellationToken = default) =>
        dbContext.DriverProfiles.AnyAsync(
            profile => profile.LicenseNumber == licenseNumber || profile.LicensePlate == licensePlate,
            cancellationToken);

    public async Task AddAndPromoteAsync(
        Account account,
        DriverProfile profile,
        CancellationToken cancellationToken = default)
    {
        account.Role = Role.Driver;
        dbContext.Accounts.Update(account);
        dbContext.DriverProfiles.Add(profile);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
