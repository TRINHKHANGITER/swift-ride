using MassTransit;
using MatchingPricing_Service.Application.Contracts;
using MatchingPricing_Service.Application.Services.Impl;
using MatchingPricing_Service.Infrastructure.RabbitMQ.Message;

namespace MatchingPricing_Service.Infrastructure.RabbitMQ;

public sealed class DriverLocationConsumer(
    IDriverLocationService driverLocationService)
    : IConsumer<DriverLocation>
{
    public async Task Consume(ConsumeContext<DriverLocation> context)
    {
        await driverLocationService.UpdateLocationAsync(
            context.Message.DriverUuid,
            new UpdateDriverLocationRequest(
                Longitude: context.Message.Longitude,
                Latitude: context.Message.Latitude
            ), context.CancellationToken);
    }
}