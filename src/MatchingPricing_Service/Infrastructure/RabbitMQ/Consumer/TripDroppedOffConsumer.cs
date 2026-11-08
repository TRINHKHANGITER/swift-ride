using MassTransit;
using MatchingPricing_Service.Application.Services.Impl;
using SwiftRide.Contracts.Events;

namespace MatchingPricing_Service.Infrastructure.RabbitMQ;

public sealed class TripDroppedOffConsumer(IDriverLocationService driverLocationService) : IConsumer<TripDroppedOff>
{
    public async Task Consume(ConsumeContext<TripDroppedOff> context)
    {
        var message = context.Message;

        await driverLocationService.UpdateAvailabilityAsync(
            message.DriverUuid,
            true,
            context.CancellationToken
        );
    }
}
