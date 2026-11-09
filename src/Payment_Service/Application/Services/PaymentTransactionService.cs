using MassTransit;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Payment_Service.Application.Contracts;
using Payment_Service.Application.Services.Impl;
using Payment_Service.Domain;
using Payment_Service.Infrastructure.Persistence.Impl;
using SwiftRide.Contracts.Events;
using System.Text.RegularExpressions;

namespace Payment_Service.Application.Services;

public sealed class PaymentTransactionService(
    IPaymentTransactionRepository paymentTransactionRepository,
    IPaymentRepository paymentRepository,
    IPublishEndpoint publishEndpoint) : IPaymentTransactionService
{
    private const string ReferenceCodeIndex = "IX_PaymentTransactions_ReferenceCode";

    public async Task<IReadOnlyList<PaymentTransactionResponseContract>> GetByPaymentUuidAsync(
        Guid paymentUuid,
        CancellationToken cancellationToken)
    {
        var transactions = await paymentTransactionRepository.GetByPaymentUuidAsync(
            paymentUuid,
            cancellationToken);

        return transactions.Select(MapResponse).ToList();
    }

    public async Task<PaymentIpnResult> ProcessIpnAsync(
        SepayIpnContract contract,
        CancellationToken cancellationToken)
    {
        if (!string.Equals(contract.TransferType, "in", StringComparison.OrdinalIgnoreCase))
        {
            return new PaymentIpnResult(PaymentIpnOutcome.Ignored, "Không xử lý giao dịch tiền ra.");
        }

        if (contract.TransferAmount <= 0 || string.IsNullOrWhiteSpace(contract.ReferenceCode))
        {
            return new PaymentIpnResult(PaymentIpnOutcome.Invalid, "Dữ liệu giao dịch không hợp lệ.");
        }

        var existingTransaction = await paymentTransactionRepository.GetByReferenceCodeAsync(
            contract.ReferenceCode,
            cancellationToken);

        if (existingTransaction is not null)
        {
            return GetExistingTransactionResult(existingTransaction);
        }

        var transferCode = ExtractTransferCode(contract);
        if (transferCode is null)
        {
            return new PaymentIpnResult(
                PaymentIpnOutcome.Invalid,
                "Không tìm thấy mã thanh toán trong giao dịch.");
        }

        var payment = await paymentRepository.GetPaymentByCodeAsync(transferCode, cancellationToken);
        if (payment is null)
        {
            return new PaymentIpnResult(
                PaymentIpnOutcome.PaymentNotFound,
                "Không tìm thấy payment tương ứng.");
        }

        if (payment.Status == PaymentStatus.Settled)
        {
            return await RecordTransactionAsync(
                payment,
                contract,
                PaymentTransactionStatus.Refunded,
                "Payment đã được thanh toán trước đó.",
                PaymentIpnOutcome.RefundRequired,
                "Payment đã được thanh toán trước đó. Đã ghi nhận hoàn tiền.",
                cancellationToken);
        }

        if (payment.Amount != contract.TransferAmount)
        {
            return await RecordTransactionAsync(
                payment,
                contract,
                PaymentTransactionStatus.Refunded,
                $"Sai số tiền. Cần thanh toán {payment.Amount}, đã nhận {contract.TransferAmount}.",
                PaymentIpnOutcome.RefundRequired,
                "Số tiền không chính xác. Đã ghi nhận hoàn tiền.",
                cancellationToken);
        }

        payment.Status = PaymentStatus.Settled;
        var result = await RecordTransactionAsync(
            payment,
            contract,
            PaymentTransactionStatus.Succeeded,
            null,
            PaymentIpnOutcome.Processed,
            "Payment đã được cập nhật thành công.",
            cancellationToken);

        if (result.Outcome == PaymentIpnOutcome.Processed)
        {
            await publishEndpoint.Publish(new PaymentSettled(payment.TripUuid), cancellationToken);
        }

        return result;
    }

    private async Task<PaymentIpnResult> RecordTransactionAsync(
        Payment payment,
        SepayIpnContract contract,
        PaymentTransactionStatus status,
        string? reason,
        PaymentIpnOutcome outcome,
        string message,
        CancellationToken cancellationToken)
    {
        var transaction = new PaymentTransaction
        {
            Uuid = Guid.NewGuid(),
            PaymentUuid = payment.Uuid,
            ReferenceCode = contract.ReferenceCode,
            Amount = contract.TransferAmount,
            Reason = reason,
            Status = status
        };

        try
        {
            await paymentTransactionRepository.CreateAsync(transaction, cancellationToken);
            return new PaymentIpnResult(outcome, message);
        }
        catch (DbUpdateException exception) when (
            exception.InnerException is PostgresException
            {
                SqlState: PostgresErrorCodes.UniqueViolation,
                ConstraintName: ReferenceCodeIndex
            })
        {
            var existingTransaction = await paymentTransactionRepository.GetByReferenceCodeAsync(
                contract.ReferenceCode,
                cancellationToken);

            return existingTransaction is null
                ? new PaymentIpnResult(PaymentIpnOutcome.Invalid, "Không thể xác định trạng thái IPN.")
                : GetExistingTransactionResult(existingTransaction);
        }
    }

    private static PaymentIpnResult GetExistingTransactionResult(PaymentTransaction transaction)
    {
        return transaction.Status == PaymentTransactionStatus.Succeeded
            ? new PaymentIpnResult(
                PaymentIpnOutcome.AlreadyProcessed,
                $"ReferenceCode {transaction.ReferenceCode} đã được xử lý và thanh toán thành công trước đó.")
            : new PaymentIpnResult(
                PaymentIpnOutcome.RefundRequired,
                $"ReferenceCode {transaction.ReferenceCode} đã hoàn tất xử lý trước đó với trạng thái hoàn tiền; Payment chưa được thanh toán.");
    }

    private static string? ExtractTransferCode(SepayIpnContract contract)
    {
        foreach (var value in new[] { contract.Code, contract.Content, contract.Description })
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                continue;
            }

            var match = Regex.Match(
                value.Replace('+', ' '),
                @"\bSEVQR\s+([A-Z0-9]+)\b",
                RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

            if (match.Success)
            {
                return match.Groups[1].Value.ToUpperInvariant();
            }

            if (ReferenceEquals(value, contract.Code))
            {
                return value.Trim().ToUpperInvariant();
            }
        }

        return null;
    }

    private static PaymentTransactionResponseContract MapResponse(PaymentTransaction transaction) => new()
    {
        Uuid = transaction.Uuid,
        PaymentUuid = transaction.PaymentUuid,
        ReferenceCode = transaction.ReferenceCode,
        Amount = transaction.Amount,
        Reason = transaction.Reason,
        Status = transaction.Status
    };
}
