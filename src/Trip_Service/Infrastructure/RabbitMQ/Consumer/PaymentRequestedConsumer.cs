using MassTransit;
using SwiftRide.Contracts.Payment;
using Trip_Service.Application.Services.Impl;
using Trip_Service.Domain;

namespace Trip_Service.Infrastructure.RabbitMQ.Consumer;

public sealed class PaymentRequestedConsumer(ITripService tripService) : IConsumer<PaymentRequested>
{
    public async Task Consume(ConsumeContext<PaymentRequested> context)
    {
        var message = context.Message;
        var trip = await tripService.GetTripByUuidAsync(message.TripUuid, context.CancellationToken);

        trip.PaymentUuid = message.PaymentUuid;
        trip.QrUrl = message.QrUrl;

        // A delayed PaymentRequested event must not revert a settled trip.
        if (trip.Status != TripStatus.Paid)
        {
            trip.Status = TripStatus.PaymentPending;
        }

        await tripService.UpdateTripAsync(trip, context.CancellationToken);
    }
}
