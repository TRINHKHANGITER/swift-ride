namespace Trip_Service.Application.Contracts.Trip;

public sealed class HistoryTripResponseContract
{
    public Guid Uuid { get; init; }
    public required string PickupName { get; init; }
    public required string DestinationName { get; init; }
    public decimal? DistanceKM { get; init; }
    public decimal? DurationMin { get; init; }
    public decimal? Price { get; init; }
    public required string Status { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}
