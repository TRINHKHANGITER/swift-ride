using Trip_Service.Domain;

namespace Trip_Service.Infrastructure.Persistence.Impl;

public interface ITripDriverRejectionRepository
{
    Task<IReadOnlyList<Guid>> FindDriverUuidsByTripAsync(
        Guid tripUuid,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        TripDriverRejection rejection,
        CancellationToken cancellationToken = default);
}
