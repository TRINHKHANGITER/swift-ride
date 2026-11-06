using System.ComponentModel.DataAnnotations;

namespace Trip_Service.Application.Contracts.Auth;

public sealed class RegisterRequestContract
{
    [Required, MaxLength(100)]
    public string Fullname { get; set; } = string.Empty;

    [Required, Phone, MaxLength(20)]
    public string Phone { get; set; } = string.Empty;

    [Required, MinLength(6), MaxLength(100)]
    public string Password { get; set; } = string.Empty;
}

public sealed class RegisterResponseContract
{
    public Guid Uuid { get; init; }
    public required string Fullname { get; init; }
    public required string Phone { get; init; }
    public required string Role { get; init; }
}
