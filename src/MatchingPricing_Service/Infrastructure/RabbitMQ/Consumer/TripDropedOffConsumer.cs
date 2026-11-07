using MassTransit;
using MatchingPricing_Service.Application.Services.Impl;
using MatchingPricing_Service.Infrastructure.RabbitMQ.Message;

namespace MatchingPricing_Service.Infrastructure.RabbitMQ;

public sealed class TripDropedOffConsumer(IDriverLocationService driverLocationService) : IConsumer<TripDropedOff>
{
    public async Task Consume(ConsumeContext<TripDropedOff> context)
    {
        var message = context.Message;

        await driverLocationService.UpdateAvailabilityAsync(
            message.DriverUuid,
            true,
            context.CancellationToken
        );
    }
}