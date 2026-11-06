using System.ComponentModel.DataAnnotations;

namespace Trip_Service.Application.Contracts.Auth;

public sealed class LoginRequestContract
{
    [Required, Phone]
    public string Phone { get; set; } = string.Empty;

    [Required, MinLength(6)]
    public string Password { get; set; } = string.Empty;
}

public sealed class LoginResponseContract
{
    public required string Token { get; init; }
    public Guid AccountUuid { get; init; }
    public required string Role { get; init; }
    public long Iat { get; init; }
    public long Exp { get; init; }
}
