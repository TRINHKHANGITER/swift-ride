using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading.Channels;
using Trip_Service.Application.Services.Impl;
using Trip_Service.Domain;

namespace Trip_Service.Application.Services;

public sealed class DriverNotificationService : IDriverNotificationService
{
    private readonly ConcurrentDictionary<Guid, Channel<Trip>> _subscribers = new();

    public async IAsyncEnumerable<Trip> SubscribeAsync(
        Guid driverUuid,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var channel = Channel.CreateUnbounded<Trip>();
        if (!_subscribers.TryAdd(driverUuid, channel))
        {
            throw new InvalidOperationException("Tài xế đã mở một kết nối nhận thông báo khác.");
        }

        try
        {
            await foreach (var trip in channel.Reader.ReadAllAsync(cancellationToken))
            {
                yield return trip;
            }
        }
        finally
        {
            _subscribers.TryRemove(driverUuid, out _);
        }
    }

    public bool Notify(Guid driverUuid, Trip trip)
    {
        return _subscribers.TryGetValue(driverUuid, out var channel) &&
               channel.Writer.TryWrite(trip);
    }
}
