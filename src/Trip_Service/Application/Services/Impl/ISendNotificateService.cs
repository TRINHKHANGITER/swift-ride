using Trip_Service.Domain;

namespace Trip_Service.Application.Services.Impl;

public interface ISendNotificateService
{
    IAsyncEnumerable<Trip> SubscribeAsync(
        Guid driverUuid,
        CancellationToken cancellationToken = default);

    bool Notify(Guid driverUuid, Trip trip);
    bool HasPendingOffer(Guid tripUuid);
    bool IsPendingOffer(Guid driverUuid, Guid tripUuid);
    void RemovePendingOffer(Guid tripUuid);
}
