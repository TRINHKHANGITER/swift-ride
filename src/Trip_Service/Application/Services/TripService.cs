using Trip_Service.Application.Contracts.Trip;
using Trip_Service.Application.Services.Impl;
using Trip_Service.Domain;
using Trip_Service.Infrastructure.Persistence.Impl;

namespace Trip_Service.Application.Services;

public sealed class TripService(
    IAccountRepository accountRepository,
    ITripRepository tripRepository) : ITripService
{
    public async Task<Trip> GetTripByUuidAsync(
        Guid tripUuid,
        CancellationToken cancellationToken = default)
    {
        return await tripRepository.FindByUuidAsync(tripUuid, cancellationToken)
            ?? throw new KeyNotFoundException("Không tìm thấy chuyến đi.");
    }

    public Task<Trip?> GetActiveTripByRiderAsync(
        Guid riderUuid,
        Guid? excludeTripUuid = null,
        CancellationToken cancellationToken = default) =>
        tripRepository.FindActiveByRiderAsync(riderUuid, excludeTripUuid, cancellationToken);

    public Task<Trip?> GetActiveTripByDriverAsync(
        Guid driverUuid,
        Guid? excludeTripUuid = null,
        CancellationToken cancellationToken = default) =>
        tripRepository.FindActiveByDriverAsync(driverUuid, excludeTripUuid, cancellationToken);

    public Task<IReadOnlyList<Trip>> GetTripHistoryByRiderAsync(
        Guid riderUuid,
        CancellationToken cancellationToken = default) =>
        tripRepository.FindHistoryByRiderAsync(riderUuid, cancellationToken);

    public async Task<Trip> CreateTripAsync(
        Guid riderUuid,
        CreateTripRequest contract,
        CancellationToken cancellationToken = default)
    {
        if (await tripRepository.FindActiveByRiderAsync(riderUuid, cancellationToken: cancellationToken) is not null)
        {
            throw new InvalidOperationException("Bạn đang có một chuyến đi chưa hoàn thành.");
        }

        var rider = await accountRepository.FindByUuidAsync(riderUuid, cancellationToken);
        if (rider is null)
        {
            throw new KeyNotFoundException("Không tìm thấy tài khoản khách.");
        }

        if (rider.Role != Role.Rider)
        {
            throw new InvalidOperationException("Chỉ tài khoản khách mới có thể tạo chuyến đi.");
        }

        if (contract.Pickup.Latitude == contract.Destination.Latitude &&
            contract.Pickup.Longitude == contract.Destination.Longitude)
        {
            throw new ArgumentException("Điểm đón và điểm đến không được trùng nhau.");
        }

        var now = DateTime.UtcNow;
        var trip = new Trip
        {
            Uuid = Guid.NewGuid(),
            RiderUuid = riderUuid,
            PickupName = contract.Pickup.Name.Trim(),
            PickupLatitude = contract.Pickup.Latitude,
            PickupLongitude = contract.Pickup.Longitude,
            DestinationName = contract.Destination.Name.Trim(),
            DestinationLatitude = contract.Destination.Latitude,
            DestinationLongitude = contract.Destination.Longitude,
            Status = TripStatus.Requested,
            CreatedAt = now,
            UpdatedAt = now
        };

        await tripRepository.AddAsync(trip, cancellationToken);
        return trip;
    }

    public async Task UpdateTripAsync(
        Trip trip,
        CancellationToken cancellationToken = default)
    {
        trip.UpdatedAt = DateTime.UtcNow;
        await tripRepository.UpdateAsync(trip, cancellationToken);
    }
}
