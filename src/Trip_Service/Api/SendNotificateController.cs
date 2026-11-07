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
    ISendNotificateService sendNotificateService) : ControllerBase
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
        Response.Headers.CacheControl = "no-cache, no-store";
        Response.Headers.Connection = "keep-alive";
        Response.Headers.Append("X-Accel-Buffering", "no");

        await Response.StartAsync(cancellationToken);
        await Response.WriteAsync("retry: 3000\n: connected\n\n", cancellationToken);
        await Response.Body.FlushAsync(cancellationToken);

        await using var trips = sendNotificateService
            .SubscribeAsync(session.Profile.DriverUuid, cancellationToken)
            .GetAsyncEnumerator(cancellationToken);
        var nextTrip = trips.MoveNextAsync().AsTask();

        while (!cancellationToken.IsCancellationRequested)
        {
            var heartbeat = Task.Delay(TimeSpan.FromSeconds(15), cancellationToken);
            var completedTask = await Task.WhenAny(nextTrip, heartbeat);

            if (completedTask == heartbeat)
            {
                await Response.WriteAsync(": heartbeat\n\n", cancellationToken);
                await Response.Body.FlushAsync(cancellationToken);
                continue;
            }

            if (!await nextTrip) break;

            var trip = trips.Current;
            var data = JsonSerializer.Serialize(trip, JsonSerializerOptions.Web);
            await Response.WriteAsync(
                $"event: trip-request\ndata: {data}\n\n", cancellationToken);
            await Response.Body.FlushAsync(cancellationToken);
            nextTrip = trips.MoveNextAsync().AsTask();
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
