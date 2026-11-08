using System.ComponentModel.DataAnnotations;

namespace Trip_Service.Domain;

public enum TripStatus
{
    Requested,
    DriverAccepted,
    // Tài xế đến điểm đón
    EnRoute,
    // Khách lên xe rồi
    PickedUp,
    // Chở khách tới
    DroppedOff,
    // Chờ khách trả tiền 
    PaymentPending,
    // Đã trả
    Paid,
    // Hủy khi khách hoặc tài xế hủy chuyến
    Cancelled,
    // Thất bại khi không tài xế nào nhận cuốc (chưa tới bước DriverAccepted)
    Failed
}


public sealed class Trip
{
    [Key]
    public Guid Uuid { get; set; }
    public Guid RiderUuid { get; set; }
    public Guid? DriverUuid { get; set; }
    public Guid? PaymentUuid { get; set; }
    public string? QrUrl { get; set; }

    // Pickup and Destination information
    public double PickupLatitude { get; set; }
    public double PickupLongitude { get; set; }

    [MaxLength(255)]
    public string PickupName { get; set; } = string.Empty;
    
    public double DestinationLatitude { get; set; }
    public double DestinationLongitude { get; set; }
    [MaxLength(255)]
    public string DestinationName { get; set; } = string.Empty;

    // Pricing and trip details
    public decimal? DistanceKM { get; set; }
    public decimal? DurationMin { get; set; }
    public decimal? Price { get; set; }
    public string? RouteGeometryJson { get; set; }
    public double? DriverLatitude { get; set; }
    public double? DriverLongitude { get; set; }
    public DateTime? DriverLocationUpdatedAt { get; set; }

    // Trip status and timestamps
    public TripStatus Status { get; set; } = TripStatus.Requested;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;  
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
