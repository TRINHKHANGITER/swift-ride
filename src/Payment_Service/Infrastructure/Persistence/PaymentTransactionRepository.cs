using Microsoft.EntityFrameworkCore;
using Payment_Service.Domain;
using Payment_Service.Infrastructure.Persistence.Impl;

namespace Payment_Service.Infrastructure.Persistence;

public sealed class PaymentTransactionRepository(PaymentDBContext context)
    : IPaymentTransactionRepository
{
    public Task<PaymentTransaction?> GetByReferenceCodeAsync(
        string referenceCode,
        CancellationToken cancellationToken)
    {
        return context.PaymentTransactions
            .AsNoTracking()
            .FirstOrDefaultAsync(
                transaction => transaction.ReferenceCode == referenceCode,
                cancellationToken);
    }

    public async Task<IReadOnlyList<PaymentTransaction>> GetByPaymentUuidAsync(
        Guid paymentUuid,
        CancellationToken cancellationToken)
    {
        return await context.PaymentTransactions
            .AsNoTracking()
            .Where(transaction => transaction.PaymentUuid == paymentUuid)
            .ToListAsync(cancellationToken);
    }

    public async Task CreateAsync(
        PaymentTransaction transaction,
        CancellationToken cancellationToken)
    {
        await context.PaymentTransactions.AddAsync(transaction, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }
}
