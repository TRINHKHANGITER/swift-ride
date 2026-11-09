using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Payment_Service.Application.Contracts;
using Payment_Service.Application.Services.Impl;

namespace Payment_Service.Api;

[ApiController]
[Route("payment")]
public sealed class PaymentController(
    IPaymentService paymentService,
    IPaymentTransactionService paymentTransactionService
    ) : ControllerBase
{
    [HttpGet("{uuid:guid}")]
    public async Task<ActionResult<PaymentDetailResponseContract>> GetByUuidAsync(
        Guid uuid,
        CancellationToken cancellationToken)
    {
        var payment = await paymentService.GetByUuidAsync(uuid, cancellationToken);

        return payment is null ? NotFound() : Ok(payment);
    }

    [HttpGet("rider/{riderUuid:guid}")]
    public async Task<ActionResult<IReadOnlyList<PaymentResponseContract>>> GetByRiderUuidAsync(
        Guid riderUuid,
        CancellationToken cancellationToken)
    {
        var payments = await paymentService.GetByRiderUuidAsync(riderUuid, cancellationToken);
        return Ok(payments);
    }

    [HttpGet("driver/{driverUuid:guid}")]
    public async Task<ActionResult<IReadOnlyList<PaymentResponseContract>>> GetByDriverUuidAsync(
        Guid driverUuid,
        CancellationToken cancellationToken)
    {
        var payments = await paymentService.GetByDriverUuidAsync(driverUuid, cancellationToken);
        return Ok(payments);
    }

    [HttpPost("ipn")]
    [AllowAnonymous]
    public async Task<ActionResult<SepayIpnResponseContract>> ReceiveIpnAsync(
        SepayIpnContract contract,
        CancellationToken cancellationToken)
    {
        var result = await paymentTransactionService.ProcessIpnAsync(contract, cancellationToken);
        var response = new SepayIpnResponseContract(
            result.Outcome is not PaymentIpnOutcome.Invalid and not PaymentIpnOutcome.PaymentNotFound,
            result.Message);

        return result.Outcome switch
        {
            PaymentIpnOutcome.Invalid => BadRequest(response),
            PaymentIpnOutcome.PaymentNotFound => NotFound(response),
            _ => Ok(response)
        };
    }
}
