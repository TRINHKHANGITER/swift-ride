using MassTransit;
using MatchingPricing_Service.Application.Services.Impl;
using MatchingPricing_Service.Infrastructure.RabbitMQ.Message;

namespace MatchingPricing_Service.Infrastructure.RabbitMQ;

public sealed class DriverOnlineConsumer(
    IDriverLocationService driverLocationService)
    : IConsumer<DriverOnline>
{
    public async Task Consume(ConsumeContext<DriverOnline> context)
    {
        await driverLocationService.UpdateOnlineStatus(
            context.Message.DriverUuid,
            context.Message.IsOnline,
            context.CancellationToken);
    }
}
