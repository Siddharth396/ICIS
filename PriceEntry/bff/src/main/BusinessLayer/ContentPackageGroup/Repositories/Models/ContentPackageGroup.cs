namespace BusinessLayer.ContentPackageGroup.Repositories.Models
{
    using System.Collections.Generic;

    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    [BsonIgnoreExtraElements]
    public class ContentPackageGroup
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public required string Id { get; set; }

        [BsonElement("sequence_id")]
        public required string SequenceId { get; set; }

        [BsonElement("price_series_ids")]
        public required List<string> PriceSeriesIds { get; set; }

        [BsonElement("content_block_definitions")]
        public required List<ContentPackageGroupContentBlock> ContentPackageGroupContentBlocks { get; set; }
    }
}