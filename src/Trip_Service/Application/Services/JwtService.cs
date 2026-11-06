using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Trip_Service.Application.Contracts.Auth;
using Trip_Service.Application.Services.Impl;
using Trip_Service.Domain;
using Trip_Service.Infrastructure.Authentication;

namespace Trip_Service.Application.Services;

public sealed class JwtService(IOptions<JwtOptions> options) : IJwtService
{
    private readonly JwtOptions _options = options.Value;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public (string Token, long IssuedAt, long ExpiresAt) Create(Account account, DriverProfile? profile)
    {
        var issuedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(_options.ExpirationMinutes).ToUnixTimeSeconds();

        var contract = new JwtContract
        {
            Uuid = account.Uuid,
            Account = new AccountPayload
            {
                Fullname = account.Fullname,
                Phone = account.Phone,
                Role = account.Role.ToString()
            },
            Profile = profile is null ? null : new ProfilePayload
            {
                ProfileUuid = profile.DriverUuid,
                LicenseNumber = profile.LicenseNumber,
                VehicleName = profile.VehicleName,
                LicensePlate = profile.LicensePlate,
                IsVerified = profile.IsVerified
            },
            Iat = issuedAt,
            Exp = expiresAt
        };

        // Generate JwtPayload
        var payload = new JwtPayload
        {
            ["uuid"] = contract.Uuid.ToString(),
            ["account"] = JsonSerializer.SerializeToElement(contract.Account, JsonOptions),
            ["profile"] = JsonSerializer.SerializeToElement(contract.Profile, JsonOptions),
            [JwtRegisteredClaimNames.Iat] = contract.Iat,
            [JwtRegisteredClaimNames.Exp] = contract.Exp,
            [JwtRegisteredClaimNames.Iss] = _options.Issuer,
            [JwtRegisteredClaimNames.Aud] = _options.Audience
        };

        var credentials = new SigningCredentials(
           new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Secret)),
           SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(new JwtHeader(credentials), payload);

        return (new JwtSecurityTokenHandler().WriteToken(token), issuedAt, expiresAt);
    }
}
