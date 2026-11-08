using MassTransit;
using SwiftRide.Contracts.Events;
using Trip_Service.Application.Services.Impl;
using Trip_Service.Domain;

namespace Trip_Service.Infrastructure.RabbitMQ.Consumer;

public sealed class PaymentSettledConsumer(ITripService tripService) : IConsumer<PaymentSettled>
{
    public async Task Consume(ConsumeContext<PaymentSettled> context)
    {
        var trip = await tripService.GetTripByUuidAsync(
            context.Message.TripUuid,
            context.CancellationToken);

        if (trip.Status == TripStatus.Paid)
        {
            return;
        }

        trip.Status = TripStatus.Paid;
        await tripService.UpdateTripAsync(trip, context.CancellationToken);
    }
}
