using System.Text.Json;
using System.Text.Json.Serialization;

namespace MatchingPricing_Service.Application.Contracts;

public sealed record LocationContract(double Latitude, double Longitude);

public sealed class OSRMResponseContract
{
    public string Code { get; init; } = string.Empty;

    public IReadOnlyList<OSRMRouteContract> Routes { get; init; } = [];

    public IReadOnlyList<OSRMWaypointContract> Waypoints { get; init; } = [];

    [JsonExtensionData]
    public IDictionary<string, JsonElement> AdditionalData { get; init; } =
        new Dictionary<string, JsonElement>();
}

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
