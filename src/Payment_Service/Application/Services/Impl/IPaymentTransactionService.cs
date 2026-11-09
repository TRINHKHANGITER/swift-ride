using Payment_Service.Application.Contracts;

namespace Payment_Service.Application.Services.Impl;

public interface IPaymentTransactionService
{
    Task<IReadOnlyList<PaymentTransactionResponseContract>> GetByPaymentUuidAsync(
        Guid paymentUuid,
        CancellationToken cancellationToken);
    Task<PaymentIpnResult> ProcessIpnAsync(
        SepayIpnContract contract,
        CancellationToken cancellationToken);
}

public enum PaymentIpnOutcome
{
    Processed,
    AlreadyProcessed,
    RefundRequired,
    Ignored,
    Invalid,
    PaymentNotFound
}

public sealed record PaymentIpnResult(PaymentIpnOutcome Outcome, string Message);
