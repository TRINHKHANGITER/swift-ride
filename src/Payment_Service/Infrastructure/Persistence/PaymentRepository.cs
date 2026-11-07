using Microsoft.EntityFrameworkCore;
using Payment_Service.Domain;
using Payment_Service.Infrastructure.Persistence.Impl;
using System.Globalization;

namespace Payment_Service.Infrastructure.Persistence;

public sealed class PaymentRepository(PaymentDBContext context) : IPaymentRepository
{
    public async Task CreatePaymentAsync(Payment payment, CancellationToken cancellationToken)
    {
        await context.Payments.AddAsync(payment, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public string GenerateCode()
    {
        const string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        Span<char> code = stackalloc char[8];

        for (var i = 0; i < code.Length; i++)
        {
            code[i] = characters[Random.Shared.Next(characters.Length)];
        }

        return new string(code);
    }

    public string GenerateQrUrl(string code, decimal amount)
    {
        var formattedAmount = amount.ToString(CultureInfo.InvariantCulture);
        return $"https://vietqr.app/img?bank=VietinBank&acc=109881092754&template=qronly&amount={formattedAmount}&des=SEVQR+{code}";
    }

    public Task<Payment?> GetPaymentByTripUuidAsync(Guid tripUuid, CancellationToken cancellationToken)
    {
        return context.Payments.Where(payment => payment.TripUuid == tripUuid).FirstOrDefaultAsync(cancellationToken);
    }

    public Task<Payment?> GetPaymentByUuidAsync(Guid uuid, CancellationToken cancellationToken)
    {
        return context.Payments
            .AsNoTracking()
            .FirstOrDefaultAsync(payment => payment.Uuid == uuid, cancellationToken);
    }

    public Task<Payment?> GetPaymentByCodeAsync(string code, CancellationToken cancellationToken)
    {
        return context.Payments.FirstOrDefaultAsync(
            payment => payment.TransferCode == code,
            cancellationToken);
    }

    public async Task<IReadOnlyList<Payment>> GetPaymentsByRiderUuidAsync(
        Guid riderUuid,
        CancellationToken cancellationToken)
    {
        return await context.Payments
            .AsNoTracking()
            .Where(payment => payment.RiderUuid == riderUuid)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Payment>> GetPaymentsByDriverUuidAsync(
        Guid driverUuid,
        CancellationToken cancellationToken)
    {
        return await context.Payments
            .AsNoTracking()
            .Where(payment => payment.DriverUuid == driverUuid)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdatePaymentAsync(Payment payment, CancellationToken cancellationToken)
    {
        context.Payments.Update(payment);
        await context.SaveChangesAsync(cancellationToken);
    }
}
