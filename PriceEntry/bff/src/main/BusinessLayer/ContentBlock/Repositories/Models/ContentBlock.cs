namespace BusinessLayer.ContentBlock.Repositories.Models
{
    using System.Collections.Generic;
    using System.Linq;

    using Infrastructure.MongoDB.Models;

    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    [BsonIgnoreExtraElements]
    public class ContentBlock
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public required string Id { get; set; }

        [BsonElement("content_block_id")]
        public required string ContentBlockId { get; set; }

        [BsonElement("version")]
        [BsonRepresentation(BsonType.Int32)]
        public required int Version { get; set; }

        [BsonElement("title")]
        public string? Title { get; set; }

        [BsonElement("price_series_grids")]
        public List<PriceSeriesGrid>? PriceSeriesGrids { get; set; }

        [BsonElement("last_modified")]
        public required AuditInfo LastModified { get; set; }

        public List<string> GetPriceSeriesIds()
        {
            return PriceSeriesGrids?.SelectMany(psg => psg.PriceSeriesIds ?? []).ToList() ?? [];
        }
    }
}