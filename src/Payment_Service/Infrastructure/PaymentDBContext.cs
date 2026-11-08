using Microsoft.EntityFrameworkCore;
using Payment_Service.Domain;

namespace Payment_Service.Infrastructure;

public sealed class PaymentDBContext : DbContext
{
    public PaymentDBContext(DbContextOptions<PaymentDBContext> options) : base(options) { }


    public DbSet<Payment> Payments => Set<Payment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasIndex(payment => payment.TripUuid)
                .IsUnique();

            entity.HasIndex(payment => payment.TransferCode)
                .IsUnique();
        });

    }
}