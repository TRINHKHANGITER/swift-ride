using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.GeoJsonObjectModel;

namespace MatchingPricing_Service.Domain;

public sealed class DriverLocation
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("driverUuid")]
    [BsonGuidRepresentation(GuidRepresentation.Standard)]    
    public Guid DriverUuid { get; set; }

    [BsonElement("location")]
    public required GeoJsonPoint<GeoJson2DGeographicCoordinates> Location { get; set; }
    
    [BsonElement("isOnline")]
    public bool IsOnline { get; set; }

    [BsonElement("isAvailable")]
    public bool IsAvailable { get; set; }

    [BsonElement("updatedAt")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
