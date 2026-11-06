using Microsoft.EntityFrameworkCore;
using Trip_Service.Application.Contracts.Auth;
using Trip_Service.Application.Services.Impl;
using Trip_Service.Domain;
using Trip_Service.Infrastructure.Persistence.Impl;


namespace Trip_Service.Application.Services;



public sealed class AuthService(
    IAccountRepository accountRepository,
    IDriverRepository driverRepository,
    IPasswordService passwordService,
    IJwtService jwtService) : IAuthService
{
    public async Task<(string Token, Account Account, long IssuedAt, long ExpiresAt)> LoginAsync(
        LoginRequestContract contract,
        CancellationToken cancellationToken = default)
    {
        var phone = Normalize(contract.Phone);
        var account = await accountRepository.FindByPhoneAsync(phone, cancellationToken);
        if (account is null || !passwordService.Verify(account, account.Password, contract.Password))
        {
            throw new UnauthorizedAccessException("Số điện thoại hoặc mật khẩu không đúng.");
        }

        var profile = await driverRepository.FindByAccountUuidAsync(account.Uuid, cancellationToken);
        var token = jwtService.Create(account, profile);
        return (token.Token, account, token.IssuedAt, token.ExpiresAt);
    }

    public async Task<Account> RegisterAsync(
        RegisterRequestContract contract,
        CancellationToken cancellationToken = default)
    {
        var phone = Normalize(contract.Phone);
        if (await accountRepository.PhoneExistsAsync(phone, cancellationToken))
        {
            throw new InvalidOperationException("Số điện thoại đã được đăng ký.");
        }

        var account = new Account
        {
            Uuid = Guid.NewGuid(),
            Fullname = contract.Fullname.Trim(),
            Phone = phone,
            Role = Role.Rider
        };
        account.Password = passwordService.Hash(account, contract.Password);

        try
        {
            await accountRepository.AddAsync(account, cancellationToken);
            return account;
        }
        catch (DbUpdateException exception)
        {
            throw new InvalidOperationException("Không thể tạo tài khoản do dữ liệu bị trùng.", exception);
        }
    }

    public async Task<(Account Account, DriverProfile Profile)> RegisterDriverAsync(
        Guid accountUuid,
        RegisterDriverRequestContract contract,
        CancellationToken cancellationToken = default)
    {
        var account = await accountRepository.FindByUuidAsync(accountUuid, cancellationToken);
        if (account is null)
        {
            throw new KeyNotFoundException("Không tìm thấy tài khoản khách.");
        }

        if (account.Role == Role.Driver ||
            await driverRepository.FindByAccountUuidAsync(accountUuid, cancellationToken) is not null)
        {
            throw new InvalidOperationException("Tài khoản đã đăng ký tài xế.");
        }

        var licenseNumber = Normalize(contract.LicenseNumber);
        var licensePlate = Normalize(contract.LicensePlate);
        if (await driverRepository.HasDuplicateAsync(licenseNumber, licensePlate, cancellationToken))
        {
            throw new InvalidOperationException("Giấy phép lái xe hoặc biển số xe đã được đăng ký.");
        }

        var profile = new DriverProfile
        {
            DriverUuid = Guid.NewGuid(),
            AccountUuid = accountUuid,
            LicenseNumber = licenseNumber,
            VehicleName = contract.VehicleName.Trim(),
            LicensePlate = licensePlate,
            IsVerified = false
        };

        try
        {
            await driverRepository.AddAndPromoteAsync(account, profile, cancellationToken);
            return (account, profile);
        }
        catch (DbUpdateException exception)
        {
            throw new InvalidOperationException("Không thể đăng ký tài xế do dữ liệu bị trùng.", exception);
        }
    }

    public async Task<(Account Account, DriverProfile? Profile)> AboutMeAsync(
        Guid accountUuid,
        CancellationToken cancellationToken = default)
    {
        var account = await accountRepository.FindByUuidAsync(accountUuid, cancellationToken);
        if (account is null)
        {
            throw new KeyNotFoundException("Không tìm thấy tài khoản.");
        }

        var profile = await driverRepository.FindByAccountUuidAsync(accountUuid, cancellationToken);
        return (account, profile);
    }

    public async Task LogoutAsync(Guid accountUuid, CancellationToken cancellationToken = default)
    {
        if (await accountRepository.FindByUuidAsync(accountUuid, cancellationToken) is null)
        {
            throw new KeyNotFoundException("Tài khoản không còn tồn tại.");
        }
    }

    private static string Normalize(string value) => value.Trim().ToUpperInvariant();
}
