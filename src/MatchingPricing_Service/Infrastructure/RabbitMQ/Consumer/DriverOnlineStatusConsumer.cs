using MassTransit;
using MatchingPricing_Service.Application.Services.Impl;
using MatchingPricing_Service.Infrastructure.RabbitMQ.Message;

namespace MatchingPricing_Service.Infrastructure.RabbitMQ;

public sealed class DriverOnlineStatusConsumer(
    IDriverLocationService driverLocationService)
    : IConsumer<DriverOnlineStatus>
{
    public async Task Consume(ConsumeContext<DriverOnlineStatus> context)
    {
        await driverLocationService.UpdateOnlineStatus(
            context.Message.DriverUuid,
            context.Message.IsOnline,
            context.CancellationToken);
    }
}
