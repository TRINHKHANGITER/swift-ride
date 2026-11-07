using System.ComponentModel.DataAnnotations;

namespace Trip_Service.Domain;

public sealed class TripDriverRejection
{
    [Key]
    public Guid Uuid { get; set; }
    public Guid TripUuid { get; set; }
    public Guid DriverUuid { get; set; }
    public DateTime RejectedAt { get; set; } = DateTime.UtcNow;
}
