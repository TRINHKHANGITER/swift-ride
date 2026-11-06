using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Trip_Service.Application.Contracts.Auth;
using Trip_Service.Application.Services.Impl;
using Trip_Service.Domain;

namespace Trip_Service.Api;

[ApiController]
[Route("auth")]
public sealed class AuthController(IAuthService authService) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseContract>> LoginAsync(
        [FromBody] LoginRequestContract contract,
        CancellationToken cancellationToken)
    {
        var session = await authService.LoginAsync(contract, cancellationToken);
        return Ok(new LoginResponseContract
        {
            Token = session.Token,
            AccountUuid = session.Account.Uuid,
            Role = session.Account.Role.ToString(),
            Iat = session.IssuedAt,
            Exp = session.ExpiresAt
        });
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<RegisterResponseContract>> RegisterAsync(
        [FromBody] RegisterRequestContract contract,
        CancellationToken cancellationToken)
    {
        var account = await authService.RegisterAsync(contract, cancellationToken);
        return CreatedAtRoute(nameof(AboutMeAsync), MapRegister(account));
    }

    [Authorize]
    [HttpPost("register-driver")]
    public async Task<ActionResult<RegisterDriverResponseContract>> RegisterDriverAsync(
        [FromBody] RegisterDriverRequestContract contract,
        CancellationToken cancellationToken)
    {
        var accountUuid = GetAccountUuid();
        var result = await authService.RegisterDriverAsync(accountUuid, contract, cancellationToken);
        return Ok(MapRegisterDriver(result.Account, result.Profile));
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> LogoutAsync(CancellationToken cancellationToken)
    {
        await authService.LogoutAsync(GetAccountUuid(), cancellationToken);
        return NoContent();
    }

    [Authorize]
    [HttpGet("about-me", Name = nameof(AboutMeAsync))]
    public async Task<ActionResult<AboutMeResponseContract>> AboutMeAsync(CancellationToken cancellationToken)
    {
        var result = await authService.AboutMeAsync(GetAccountUuid(), cancellationToken);
        return Ok(MapAboutMe(result.Account, result.Profile));
    }

    // Helper function
    private Guid GetAccountUuid()
    {
        if (Guid.TryParse(User.FindFirst("uuid")?.Value, out var accountUuid))
        {
            return accountUuid;
        }

        throw new UnauthorizedAccessException("Token không chứa Uuid hợp lệ.");
    }

    private static RegisterResponseContract MapRegister(Account account) => new()
    {
        Uuid = account.Uuid,
        Fullname = account.Fullname,
        Phone = account.Phone,
        Role = account.Role.ToString()
    };

    private static AboutMeResponseContract MapAboutMe(Account account, DriverProfile? profile) => new()
    {
        Account = new AboutMeResponseContract.AccountData
        {
            Uuid = account.Uuid,
            Fullname = account.Fullname,
            Phone = account.Phone,
            Role = account.Role.ToString()
        },
        Profile = profile is null ? null : new AboutMeResponseContract.ProfileData
        {
            ProfileUuid = profile.DriverUuid,
            LicenseNumber = profile.LicenseNumber,
            VehicleName = profile.VehicleName,
            LicensePlate = profile.LicensePlate,
            IsVerified = profile.IsVerified
        }
    };

    private static RegisterDriverResponseContract MapRegisterDriver(Account account, DriverProfile profile) => new()
    {
        AccountUuid = account.Uuid,
        Role = account.Role.ToString(),
        ProfileUuid = profile.DriverUuid,
        LicenseNumber = profile.LicenseNumber,
        VehicleName = profile.VehicleName,
        LicensePlate = profile.LicensePlate,
        IsVerified = profile.IsVerified
    };
}
