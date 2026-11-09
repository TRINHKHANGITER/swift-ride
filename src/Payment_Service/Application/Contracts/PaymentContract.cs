using Payment_Service.Domain;

namespace Payment_Service.Application.Contracts;

public class PaymentResponseContract
{
    public Guid Uuid { get; set; }
    public Guid TripUuid { get; set; }
    public Guid RiderUuid { get; set; }
    public Guid DriverUuid { get; set; }
    public decimal Amount { get; set; }
    public string QrUrl { get; set; } = string.Empty;
    public string TransferCode { get; set; } = string.Empty;
    public PaymentStatus Status { get; set; }
}

public sealed class PaymentDetailResponseContract : PaymentResponseContract
{
    public IReadOnlyList<PaymentTransactionResponseContract> Transactions { get; set; } = [];
}

public sealed class PaymentTransactionResponseContract
{
    public Guid Uuid { get; set; }
    public Guid PaymentUuid { get; set; }
    public string ReferenceCode { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? Reason { get; set; }
    public PaymentTransactionStatus Status { get; set; }
}
