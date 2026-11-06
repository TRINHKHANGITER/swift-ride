using System.ComponentModel.DataAnnotations;

namespace MatchingPricing_Service.Application.Contracts;

public sealed class MatchingRequestContract
{
    public Guid TripUuid { get; set; }

    [Required]
    public LocationContract Pickup { get; set; } = null!;

    public IReadOnlyCollection<Guid> ExcludeDriverUuids { get; set; } = [];
}

public sealed class MatchingResponseContract
{
    public Guid TripUuid { get; set; }
    public Guid DriverUuid { get; set; }
    public bool IsSuccess { get; set; }
}
