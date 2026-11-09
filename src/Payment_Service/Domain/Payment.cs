using System.ComponentModel.DataAnnotations;

namespace Payment_Service.Domain;

public enum PaymentStatus
{
    Pending, Settled
}

public class Payment
{
    [Key]
    public Guid Uuid { get; set; }
    public Guid TripUuid { get; set; }
    public Guid RiderUuid { get; set; }
    public Guid DriverUuid { get; set; }
    public decimal Amount { get; set; }
    public string QrUrl { get; set; } = string.Empty;
    public string TransferCode { get; set; } = string.Empty;
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public ICollection<PaymentTransaction> Transactions { get; set; } = [];
}
