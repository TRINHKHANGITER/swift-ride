using Payment_Service.Domain;

namespace Payment_Service.Infrastructure.Persistence.Impl;

public interface IPaymentRepository
{
    string GenerateCode();
    string GenerateQrUrl(string code, decimal amount);

    Task<Payment?> GetPaymentByUuidAsync(Guid uuid, CancellationToken cancellationToken);
    Task<Payment?> GetPaymentByTripUuidAsync(Guid tripUuid, CancellationToken cancellationToken);
    Task<Payment?> GetPaymentByCodeAsync(string code, CancellationToken cancellationToken);
    Task<IReadOnlyList<Payment>> GetPaymentsByRiderUuidAsync(Guid riderUuid, CancellationToken cancellationToken);
    Task<IReadOnlyList<Payment>> GetPaymentsByDriverUuidAsync(Guid driverUuid, CancellationToken cancellationToken);

    Task CreatePaymentAsync(Payment payment, CancellationToken cancellationToken);
    Task UpdatePaymentAsync(Payment payment, CancellationToken cancellationToken);
}
