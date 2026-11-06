using Trip_Service.Application.Contracts.Auth;
using Trip_Service.Domain;

namespace Trip_Service.Application.Services.Impl;

public interface IAuthService
{
    Task<(string Token, Account Account, long IssuedAt, long ExpiresAt)> LoginAsync(
        LoginRequestContract contract,
        CancellationToken cancellationToken = default);

    Task<Account> RegisterAsync(
        RegisterRequestContract contract,
        CancellationToken cancellationToken = default);

    Task<(Account Account, DriverProfile Profile)> RegisterDriverAsync(
        Guid accountUuid,
        RegisterDriverRequestContract contract,
        CancellationToken cancellationToken = default);

    Task<(Account Account, DriverProfile? Profile)> AboutMeAsync(
        Guid accountUuid,
        CancellationToken cancellationToken = default);

    Task LogoutAsync(Guid accountUuid, CancellationToken cancellationToken = default);
}
