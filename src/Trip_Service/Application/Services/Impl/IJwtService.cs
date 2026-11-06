using Trip_Service.Domain;

namespace Trip_Service.Application.Services.Impl;

public interface IJwtService
{
    (string Token, long IssuedAt, long ExpiresAt) Create(Account account, DriverProfile? profile);
}
