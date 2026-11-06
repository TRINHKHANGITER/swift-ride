using Trip_Service.Domain;

namespace Trip_Service.Application.Services.Impl;

public interface IDriverNotificationService
{
    IAsyncEnumerable<Trip> SubscribeAsync(
        Guid driverUuid,
        CancellationToken cancellationToken = default);

    bool Notify(Guid driverUuid, Trip trip);
}
