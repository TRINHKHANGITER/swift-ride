using System.ComponentModel.DataAnnotations;

namespace Trip_Service.Application.Contracts.Auth;

public sealed class RegisterDriverRequestContract
{
    [Required, MaxLength(50)]
    public string LicenseNumber { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string VehicleName { get; set; } = string.Empty;

    [Required, MaxLength(30)]
    public string LicensePlate { get; set; } = string.Empty;
}

public sealed class RegisterDriverResponseContract
{
    public Guid AccountUuid { get; init; }
    public required string Role { get; init; }
    public Guid ProfileUuid { get; init; }
    public required string LicenseNumber { get; init; }
    public required string VehicleName { get; init; }
    public required string LicensePlate { get; init; }
    public bool IsVerified { get; init; }
}
