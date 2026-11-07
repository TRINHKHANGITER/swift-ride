using System.Text.Json.Serialization;

namespace Trip_Service.Application.Contracts.Payment;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PaymentStatusContract
{
    Pending,
    Settled,
    Failed,
    Refunded
}

public sealed class PaymentResponseContract
{
    public Guid Uuid { get; init; }
    public Guid TripUuid { get; init; }
    public Guid RiderUuid { get; init; }
    public Guid DriverUuid { get; init; }
    public decimal Amount { get; init; }
    public string QrUrl { get; init; } = string.Empty;
    public string TransferCode { get; init; } = string.Empty;
    public PaymentStatusContract Status { get; init; }
}
