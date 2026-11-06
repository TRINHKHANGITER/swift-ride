namespace Trip_Service.Application.Contracts.Auth;

public class JwtContract
{
    // Account Uuid
    public required Guid Uuid { get; set; }
    public required AccountPayload Account { get; set; }
    public ProfilePayload? Profile { get; set; }
    public long Iat { get; set; }
    public long Exp { get; set; }
}

public class AccountPayload
{
    public string Fullname { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}

public class ProfilePayload
{
    public Guid ProfileUuid { get; set; }
    public string LicenseNumber { get; set; } = string.Empty;
    public string VehicleName { get; set; } = string.Empty; 
    public string LicensePlate { get; set; } = string.Empty;
    public bool IsVerified { get; set; }
}
