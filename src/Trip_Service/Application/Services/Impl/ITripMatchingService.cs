using Trip_Service.Application.Contracts;
using Trip_Service.Domain;

namespace Trip_Service.Application.Services.Impl;

public interface ITripMatchingService
{
    Task<MatchingResponseContract> FindAndNotifyNextDriverAsync(
        Trip trip,
        CancellationToken cancellationToken = default);
}
