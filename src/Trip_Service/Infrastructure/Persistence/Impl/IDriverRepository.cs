using Trip_Service.Domain;

namespace Trip_Service.Infrastructure.Persistence.Impl;

public interface IDriverRepository
{
    Task<DriverProfile?> FindByUuidAsync(
        Guid driverUuid,
        CancellationToken cancellationToken = default);

    Task<DriverProfile?> FindByAccountUuidAsync(
        Guid accountUuid,
        CancellationToken cancellationToken = default);

    Task<bool> HasDuplicateAsync(
        string licenseNumber,
        string licensePlate,
        CancellationToken cancellationToken = default);

    Task AddAndPromoteAsync(
        Account account,
        DriverProfile profile,
        CancellationToken cancellationToken = default);
}
