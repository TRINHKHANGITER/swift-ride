using Payment_Service.Domain;

namespace Payment_Service.Infrastructure.Persistence.Impl;

public interface IPaymentTransactionRepository
{
    Task<PaymentTransaction?> GetByReferenceCodeAsync(
        string referenceCode,
        CancellationToken cancellationToken);
    Task<IReadOnlyList<PaymentTransaction>> GetByPaymentUuidAsync(
        Guid paymentUuid,
        CancellationToken cancellationToken);
    Task CreateAsync(
        PaymentTransaction transaction,
        CancellationToken cancellationToken);
}
