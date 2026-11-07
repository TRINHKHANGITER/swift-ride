using Payment_Service.Domain;

namespace Payment_Service.Application.Contracts;

public sealed class PaymentResponseContract
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