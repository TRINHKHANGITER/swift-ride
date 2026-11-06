using Microsoft.EntityFrameworkCore;
using Trip_Service.Domain;
using Trip_Service.Infrastructure.Persistence.Impl;

namespace Trip_Service.Infrastructure.Persistence.Repositories;

public sealed class AccountRepository(TripDbContext dbContext) : IAccountRepository
{
    public Task<Account?> FindByPhoneAsync(string phone, CancellationToken cancellationToken = default) =>
        dbContext.Accounts.AsNoTracking()
            .SingleOrDefaultAsync(account => account.Phone == phone, cancellationToken);

    public Task<Account?> FindByUuidAsync(Guid uuid, CancellationToken cancellationToken = default) =>
        dbContext.Accounts.AsNoTracking()
            .SingleOrDefaultAsync(account => account.Uuid == uuid, cancellationToken);

    public Task<bool> PhoneExistsAsync(string phone, CancellationToken cancellationToken = default) =>
        dbContext.Accounts.AnyAsync(account => account.Phone == phone, cancellationToken);

    public async Task AddAsync(Account account, CancellationToken cancellationToken = default)
    {
        dbContext.Accounts.Add(account);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
