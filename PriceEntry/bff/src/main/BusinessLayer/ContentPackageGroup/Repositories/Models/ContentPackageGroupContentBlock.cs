namespace BusinessLayer.ContentPackageGroup.Repositories.Models
{
    using MongoDB.Bson.Serialization.Attributes;

    [BsonIgnoreExtraElements]
    public class ContentPackageGroupContentBlock
    {
        [BsonElement("content_block_id")]
        public required string ContentBlockId { get; set; }

        [BsonElement("version")]
        public required int Version { get; set; }
    }
}
