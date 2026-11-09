using Payment_Service.Application.Contracts;

namespace Payment_Service.Application.Services.Impl;

public interface IPaymentService
{
    Task<PaymentDetailResponseContract?> GetByUuidAsync(Guid uuid, CancellationToken cancellationToken);
    Task<IReadOnlyList<PaymentResponseContract>> GetByRiderUuidAsync(
        Guid riderUuid,
        CancellationToken cancellationToken);
    Task<IReadOnlyList<PaymentResponseContract>> GetByDriverUuidAsync(
        Guid driverUuid,
        CancellationToken cancellationToken);
}
