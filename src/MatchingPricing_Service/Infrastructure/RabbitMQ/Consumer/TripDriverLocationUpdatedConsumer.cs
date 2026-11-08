using MassTransit;
using MatchingPricing_Service.Application.Services.Impl;
using SwiftRide.Contracts.Events;

namespace MatchingPricing_Service.Infrastructure.RabbitMQ;

public sealed class TripDriverLocationUpdatedConsumer(
    IDriverLocationService driverLocationService)
    : IConsumer<TripDriverLocationUpdated>
{
    public async Task Consume(ConsumeContext<TripDriverLocationUpdated> context)
    {
        await driverLocationService.UpdateLocationAsync(
            context.Message.DriverUuid,
            context.Message.Latitude,
            context.Message.Longitude,
            context.CancellationToken);
    }
}
