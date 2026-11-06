using System.Text.Json;
using System.Text.Json.Serialization;

namespace Trip_Service.Application.Contracts;

public sealed record QuoteRequestContract(LocationContract Pickup, LocationContract Destination);
public sealed record QuoteResponseContract(decimal Price, OSRMRouteContract Route, double DistanceKM, double DurationMin);

public sealed record MatchingRequestContract(Guid TripUuid, LocationContract Pickup, IReadOnlyCollection<Guid> ExcludedDriverIds);
public sealed record MatchingResponseContract(Guid TripUuid, Guid? DriverUuid, bool IsSuccess);
public sealed record DriverLocationContract(Guid DriverUuid, double Latitude, double Longitude, bool IsOnline, bool IsAvailable, DateTime UpdatedAt);
public sealed record UpdateDriverLocationContract(double Latitude, double Longitude);
public sealed record UpdateDriverAvailabilityContract(bool IsAvailable);

// Helper class
public sealed record LocationContract(double Latitude, double Longitude);

public sealed class OSRMRouteContract
{
    public IReadOnlyList<OSRMLegContract> Legs { get; init; } = [];

    [JsonPropertyName("weight_name")]
    public string WeightName { get; init; } = string.Empty;

    public OSRMGeometryContract Geometry { get; init; } = new();

    public double Weight { get; init; }
    public double Duration { get; init; }
    public double Distance { get; init; }

    // Unused
    [JsonExtensionData]
    public IDictionary<string, JsonElement> AdditionalData { get; init; } =
        new Dictionary<string, JsonElement>();
}

public sealed class OSRMLegContract
{
    public IReadOnlyList<JsonElement> Steps { get; init; } = [];
    public double Weight { get; init; }
    public string Summary { get; init; } = string.Empty;
    public double Duration { get; init; }
    public double Distance { get; init; }

    [JsonExtensionData]
    public IDictionary<string, JsonElement> AdditionalData { get; init; } =
        new Dictionary<string, JsonElement>();
}

public sealed class OSRMGeometryContract
{
    public string Type { get; init; } = string.Empty;
    public double[][] Coordinates { get; init; } = [];

    // Unused
    [JsonExtensionData]
    public IDictionary<string, JsonElement> AdditionalData { get; init; } =
        new Dictionary<string, JsonElement>();
}

public sealed class OSRMWaypointContract
{
    public string Hint { get; init; } = string.Empty;
    public double[] Location { get; init; } = [];
    public string Name { get; init; } = string.Empty;
    public double Distance { get; init; }

    // Unused
    [JsonExtensionData]
    public IDictionary<string, JsonElement> AdditionalData { get; init; } =
        new Dictionary<string, JsonElement>();
}
