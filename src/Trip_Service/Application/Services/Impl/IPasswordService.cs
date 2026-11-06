using Trip_Service.Domain;

namespace Trip_Service.Application.Services.Impl;

public interface IPasswordService
{
    string Hash(Account account, string password);
    bool Verify(Account account, string hashedPassword, string password);
}
