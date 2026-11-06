using System.ComponentModel.DataAnnotations;

namespace Trip_Service.Domain;

public sealed class DriverProfile
{
    [Key]
    public Guid DriverUuid { get; set; }
    public Guid AccountUuid { get; set; }
    public string LicenseNumber { get; set; } = string.Empty;
    public string VehicleName { get; set; } = string.Empty; 
    public string LicensePlate { get; set; } = string.Empty;
    public bool IsVerified { get; set; }
}
