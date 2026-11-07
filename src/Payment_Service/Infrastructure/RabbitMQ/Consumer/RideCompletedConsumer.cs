using MassTransit;
using Payment_Service.Domain;
using Payment_Service.Infrastructure.Persistence.Impl;
using SwiftRide.Contracts.Payment;

namespace Payment_Service.Infrastructure.RabbitMQ.Consumer;


public sealed class RideCompletedConsumer(
    IPaymentRepository paymentRepository,
    IPublishEndpoint publishEndpoint)
    : IConsumer<RideCompleted>
{
    public async Task Consume(ConsumeContext<RideCompleted> context)
    {
        var existingPayment = await paymentRepository.GetPaymentByTripUuidAsync(
            context.Message.TripUuid,
            context.CancellationToken);

        if (existingPayment is not null)
        {
            await publishEndpoint.Publish(
                new PaymentRequested(existingPayment.TripUuid, existingPayment.Uuid, existingPayment.QrUrl),
                context.CancellationToken);
            return;
        }

        string code = paymentRepository.GenerateCode();
        string qrUrl = paymentRepository.GenerateQrUrl(code, context.Message.TotalPrice);

        var payment = new Payment
        {
            Uuid = Guid.NewGuid(),
            TripUuid = context.Message.TripUuid,
            RiderUuid = context.Message.RiderUuid,
            DriverUuid = context.Message.DriverUuid,
            Amount = context.Message.TotalPrice,
            QrUrl = qrUrl,
            TransferCode = code,
            Status = PaymentStatus.Pending
        };

        await paymentRepository.CreatePaymentAsync(payment, context.CancellationToken);

        await publishEndpoint.Publish(
            new PaymentRequested(payment.TripUuid, payment.Uuid, payment.QrUrl),
            context.CancellationToken);
    }
}
