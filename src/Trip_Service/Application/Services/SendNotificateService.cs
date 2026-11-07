using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading.Channels;
using Trip_Service.Application.Services.Impl;
using Trip_Service.Domain;

namespace Trip_Service.Application.Services;

public sealed class SendNotificateService : ISendNotificateService
{
    private readonly ConcurrentDictionary<Guid, Channel<Trip>> _subscribers = new();
    private readonly ConcurrentDictionary<Guid, Guid> _pendingOffers = new();

    public async IAsyncEnumerable<Trip> SubscribeAsync(
        Guid driverUuid,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var channel = Channel.CreateUnbounded<Trip>(new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = false
        });

        Channel<Trip>? previousChannel = null;
        _subscribers.AddOrUpdate(
            driverUuid,
            channel,
            (_, currentChannel) =>
            {
                previousChannel = currentChannel;
                return channel;
            });
        previousChannel?.Writer.TryComplete();

        try
        {
            await foreach (var trip in channel.Reader.ReadAllAsync(cancellationToken))
            {
                yield return trip;
            }
        }
        finally
        {
            ((ICollection<KeyValuePair<Guid, Channel<Trip>>>)_subscribers)
                .Remove(new KeyValuePair<Guid, Channel<Trip>>(driverUuid, channel));
        }
    }

    public bool Notify(Guid driverUuid, Trip trip)
    {
        if (!_subscribers.TryGetValue(driverUuid, out var channel)) return false;

        _pendingOffers[trip.Uuid] = driverUuid;
        if (channel.Writer.TryWrite(trip)) return true;

        _pendingOffers.TryRemove(trip.Uuid, out _);
        return false;
    }

    public bool IsPendingOffer(Guid driverUuid, Guid tripUuid) =>
        _pendingOffers.TryGetValue(tripUuid, out var offeredDriverUuid) &&
        offeredDriverUuid == driverUuid;

    public bool HasPendingOffer(Guid tripUuid) =>
        _pendingOffers.ContainsKey(tripUuid);

    public void RemovePendingOffer(Guid tripUuid) =>
        _pendingOffers.TryRemove(tripUuid, out _);
}
