using Microsoft.AspNetCore.Identity;
using Trip_Service.Application.Services.Impl;
using Trip_Service.Domain;

namespace Trip_Service.Application.Services;

public sealed class PasswordService(IPasswordHasher<Account> passwordHasher) : IPasswordService
{
    public string Hash(Account account, string password)
    {
        return passwordHasher.HashPassword(account, password);
    }

    public bool Verify(Account account, string hashedPassword, string password)
    {
        return passwordHasher.VerifyHashedPassword(account, hashedPassword, password) != PasswordVerificationResult.Failed;
    }
}
