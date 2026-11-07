using MassTransit;
using MatchingPricing_Service.Application.Services.Impl;
using MatchingPricing_Service.Infrastructure.RabbitMQ.Message;

namespace MatchingPricing_Service.Infrastructure.RabbitMQ;

public sealed class TripAcceptedConsumer(IDriverLocationService driverLocationService) : IConsumer<TripAccepted>
{
    public async Task Consume(ConsumeContext<TripAccepted> context)
    {
        var message = context.Message;

        await driverLocationService.UpdateAvailabilityAsync(
            message.DriverUuid,
            false,
            context.CancellationToken
        );
    }
}