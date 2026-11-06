using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Trip_Service.Application.Services.Impl;

namespace Trip_Service.Api;

[ApiController]
[Authorize]
[Route("drivers/notifications")]
public sealed class DriverNotificationController(
    IAuthService authService,
    IDriverNotificationService notificationService) : ControllerBase
{
    [HttpGet("stream")]
    public async Task StreamAsync(CancellationToken cancellationToken)
    {
        var session = await authService.AboutMeAsync(GetAccountUuid(), cancellationToken);
        if (session.Profile is null)
        {
            throw new UnauthorizedAccessException("Tài khoản không phải là tài xế.");
        }

        Response.StatusCode = StatusCodes.Status200OK;
        Response.ContentType = "text/event-stream";
        Response.Headers.CacheControl = "no-cache";
        Response.Headers.Connection = "keep-alive";

        await foreach (var trip in notificationService.SubscribeAsync(
                           session.Profile.DriverUuid,
                           cancellationToken))
        {
            var data = JsonSerializer.Serialize(trip, JsonSerializerOptions.Web);
            await Response.WriteAsync($"event: trip-request\ndata: {data}\n\n", cancellationToken);
            await Response.Body.FlushAsync(cancellationToken);
        }
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
