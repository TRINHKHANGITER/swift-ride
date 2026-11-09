using System.ComponentModel.DataAnnotations;

namespace Payment_Service.Domain;

public enum PaymentTransactionStatus
{
    Succeeded,
    Refunded
}

public class PaymentTransaction
{
    [Key]
    public Guid Uuid { get; set; }
    public Guid PaymentUuid { get; set; }
    public string ReferenceCode { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? Reason { get; set; }
    public PaymentTransactionStatus Status { get; set; }
    public Payment Payment { get; set; } = null!;
}
