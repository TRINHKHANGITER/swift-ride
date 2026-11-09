using Payment_Service.Application.Contracts;
using Payment_Service.Application.Services.Impl;
using Payment_Service.Domain;
using Payment_Service.Infrastructure.Persistence.Impl;

namespace Payment_Service.Application.Services;

public sealed class PaymentService(
    IPaymentRepository paymentRepository,
    IPaymentTransactionService paymentTransactionService) : IPaymentService
{
    public async Task<PaymentDetailResponseContract?> GetByUuidAsync(
        Guid uuid,
        CancellationToken cancellationToken)
    {
        var payment = await paymentRepository.GetPaymentByUuidAsync(uuid, cancellationToken);
        if (payment is null)
        {
            return null;
        }

        var transactions = await paymentTransactionService.GetByPaymentUuidAsync(
            payment.Uuid,
            cancellationToken);

        return MapDetailResponse(payment, transactions);
    }

    public async Task<IReadOnlyList<PaymentResponseContract>> GetByRiderUuidAsync(
        Guid riderUuid,
        CancellationToken cancellationToken)
    {
        var payments = await paymentRepository.GetPaymentsByRiderUuidAsync(riderUuid, cancellationToken);
        return payments.Select(MapResponse).ToList();
    }

    public async Task<IReadOnlyList<PaymentResponseContract>> GetByDriverUuidAsync(
        Guid driverUuid,
        CancellationToken cancellationToken)
    {
        var payments = await paymentRepository.GetPaymentsByDriverUuidAsync(driverUuid, cancellationToken);
        return payments.Select(MapResponse).ToList();
    }

    private static PaymentResponseContract MapResponse(Payment payment) => new()
    {
        Uuid = payment.Uuid,
        TripUuid = payment.TripUuid,
        RiderUuid = payment.RiderUuid,
        DriverUuid = payment.DriverUuid,
        Amount = payment.Amount,
        QrUrl = payment.QrUrl,
        TransferCode = payment.TransferCode,
        Status = payment.Status
    };

    private static PaymentDetailResponseContract MapDetailResponse(
        Payment payment,
        IReadOnlyList<PaymentTransactionResponseContract> transactions) => new()
    {
        Uuid = payment.Uuid,
        TripUuid = payment.TripUuid,
        RiderUuid = payment.RiderUuid,
        DriverUuid = payment.DriverUuid,
        Amount = payment.Amount,
        QrUrl = payment.QrUrl,
        TransferCode = payment.TransferCode,
        Status = payment.Status,
        Transactions = transactions
    };
}
