using Trip_Service.Application.Contracts.Payment;

namespace Trip_Service.Application.Services.Impl;

public interface IPaymentService
{
    Task<IReadOnlyList<PaymentResponseContract>> GetPaymentsByRiderUuidAsync(
        Guid riderUuid,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PaymentResponseContract>> GetPaymentsByDriverUuidAsync(
        Guid driverUuid,
        CancellationToken cancellationToken = default);
}
