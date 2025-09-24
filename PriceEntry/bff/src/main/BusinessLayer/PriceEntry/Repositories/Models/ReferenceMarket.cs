namespace BusinessLayer.PriceEntry.Repositories.Models
{
    using System.Collections.Generic;

    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    [BsonIgnoreExtraElements]
    public class ReferenceMarket
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public required string Id { get; set; }

        [BsonElement("name")]
        public required string Name { get; set; }

        [BsonElement("type")]
        public required string Type { get; set; }

        [BsonElement("price_series_ids")]
        public List<string>? PriceSeriesIds { get; set; } = default!;
    }
}
