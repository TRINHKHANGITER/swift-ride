using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Trip_Service.Application.Contracts.Payment;
using Trip_Service.Application.Services.Impl;

namespace Trip_Service.Api;

[ApiController]
[Authorize]
[Route("payments")]
public sealed class PaymentController(
    IPaymentService paymentService,
    IAuthService authService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<PaymentResponseContract>>> GetMyPaymentsAsync(
        CancellationToken cancellationToken)
    {
        var payments = await paymentService.GetPaymentsByRiderUuidAsync(
            GetAccountUuid(),
            cancellationToken);

        return Ok(payments);
    }

    [HttpGet("driver")]
    public async Task<ActionResult<IReadOnlyList<PaymentResponseContract>>> GetMyDriverPaymentsAsync(
        CancellationToken cancellationToken)
    {
        var session = await authService.AboutMeAsync(GetAccountUuid(), cancellationToken);
        var driverUuid = session.Profile?.DriverUuid
            ?? throw new UnauthorizedAccessException("Tài khoản không phải là tài xế.");

        var payments = await paymentService.GetPaymentsByDriverUuidAsync(
            driverUuid,
            cancellationToken);

        return Ok(payments);
    }

    private Guid GetAccountUuid()
    {
        if (Guid.TryParse(User.FindFirst("uuid")?.Value, out var accountUuid))
        {
            return accountUuid;
        }

        throw new UnauthorizedAccessException("Token không chứa Uuid hợp lệ.");
    }
}
