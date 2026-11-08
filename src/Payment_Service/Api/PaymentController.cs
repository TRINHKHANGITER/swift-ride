using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Payment_Service.Application.Contracts;
using Payment_Service.Domain;
using Payment_Service.Infrastructure.Persistence.Impl;
using SwiftRide.Contracts.Events;
using System.Text.RegularExpressions;

namespace Payment_Service.Api;

[ApiController]
[Route("payment")]
public sealed class PaymentController(
    IPaymentRepository paymentRepository,
    IPublishEndpoint publishEndpoint
    ) : ControllerBase
{
    [HttpGet("{uuid:guid}")]
    public async Task<ActionResult<PaymentResponseContract>> GetByUuidAsync(
        Guid uuid,
        CancellationToken cancellationToken)
    {
        var payment = await paymentRepository.GetPaymentByUuidAsync(uuid, cancellationToken);

        return payment is null ? NotFound() : Ok(MapResponse(payment));
    }

    [HttpGet("rider/{riderUuid:guid}")]
    public async Task<ActionResult<IReadOnlyList<PaymentResponseContract>>> GetByRiderUuidAsync(
        Guid riderUuid,
        CancellationToken cancellationToken)
    {
        var payments = await paymentRepository.GetPaymentsByRiderUuidAsync(riderUuid, cancellationToken);
        return Ok(payments.Select(MapResponse).ToList());
    }

    [HttpGet("driver/{driverUuid:guid}")]
    public async Task<ActionResult<IReadOnlyList<PaymentResponseContract>>> GetByDriverUuidAsync(
        Guid driverUuid,
        CancellationToken cancellationToken)
    {
        var payments = await paymentRepository.GetPaymentsByDriverUuidAsync(driverUuid, cancellationToken);
        return Ok(payments.Select(MapResponse).ToList());
    }

    // ! IPN
    [HttpPost("ipn")]
    [AllowAnonymous]
    public async Task<ActionResult<SepayIpnResponseContract>> ReceiveIpnAsync(
        SepayIpnContract contract,
        CancellationToken cancellationToken)
    {
        var transferCode = ExtractTransferCode(contract);
        if (transferCode is null)
        {
            return BadRequest(new SepayIpnResponseContract(false, "Không tìm thấy mã thanh toán trong giao dịch."));
        }

        var payment = await paymentRepository.GetPaymentByCodeAsync(transferCode, cancellationToken);
        if (payment is null)
        {
            return NotFound(new SepayIpnResponseContract(false, "Không tìm thấy payment tương ứng."));
        }

        if (payment.Status != PaymentStatus.Settled)
        {
            payment.Status = PaymentStatus.Settled;
            await paymentRepository.UpdatePaymentAsync(payment, cancellationToken);
        }

        await publishEndpoint.Publish(new PaymentSettled(payment.TripUuid), cancellationToken);

        return Ok(new SepayIpnResponseContract(true, "Payment đã được cập nhật thành công."));
    }

    // ! Error IPN
    [HttpPost("ipn-error")]
    [AllowAnonymous]
    public async Task<ActionResult> ReceiveIpnErrorAsync()
    {
        Console.WriteLine("IPN ERROR");
        return Ok();
    }

    private static string? ExtractTransferCode(SepayIpnContract contract)
    {
        foreach (var value in new[] { contract.Code, contract.Content, contract.Description })
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                continue;
            }

            var normalizedValue = value.Replace('+', ' ');
            var match = Regex.Match(
                normalizedValue,
                @"\bSEVQR\s+([A-Z0-9]+)\b",
                RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

            if (match.Success)
            {
                return match.Groups[1].Value.ToUpperInvariant();
            }

            // SePay có thể trả riêng mã đã được bóc tách trong trường Code.
            if (ReferenceEquals(value, contract.Code))
            {
                return value.Trim().ToUpperInvariant();
            }
        }

        return null;
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
}
