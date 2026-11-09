using MassTransit;
using MatchingPricing_Service.Application.Services.Impl;
using SwiftRide.Contracts.Events;

namespace MatchingPricing_Service.Infrastructure.RabbitMQ;

public sealed class TripCancelledConsumer(IDriverLocationService driverLocationService)
    : IConsumer<TripCancelled>
{
    public async Task Consume(ConsumeContext<TripCancelled> context)
    {
        await driverLocationService.UpdateAvailabilityAsync(
            context.Message.DriverUuid,
            true,
            context.CancellationToken);
    }
}
