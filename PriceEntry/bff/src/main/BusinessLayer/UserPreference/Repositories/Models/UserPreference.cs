namespace BusinessLayer.UserPreference.Repositories.Models
{
    using System.Collections.Generic;

    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    [BsonIgnoreExtraElements]
    public class UserPreference
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public required string Id { get; set; }

        [BsonElement("content_block_id")]
        public required string ContentBlockId { get; set; }

        [BsonElement("user_id")]
        public required string UserId { get; set; }

        [BsonElement("price_series_grids")]
        public required List<PriceSeriesGrid> PriceSeriesGrids { get; set; } = new();
    }
}
