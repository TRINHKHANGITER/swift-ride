using System.Text.Json.Serialization;

namespace Trip_Service.Application.Contracts.Auth;

public sealed class AboutMeResponseContract
{
    public required AccountData Account { get; init; }
    public ProfileData? Profile { get; init; }

    // Helper class
    public sealed class AccountData
    {
        public Guid Uuid { get; init; }
        public required string Fullname { get; init; }
        public required string Phone { get; init; }
        public required string Role { get; init; }
    }

    public sealed class ProfileData
    {
        public Guid ProfileUuid { get; init; }
        public required string LicenseNumber { get; init; }
        public required string VehicleName { get; init; }
        public required string LicensePlate { get; init; }
        public bool IsVerified { get; init; }
    }
}
