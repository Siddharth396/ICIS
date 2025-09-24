namespace BusinessLayer.DataPackage.Repositories.Models
{
    using System;

    using MongoDB.Bson.Serialization.Attributes;

    [BsonIgnoreExtraElements]
    public class DataPackageIdMetadata
    {
        [BsonElement("content_block_id")]
        public required string ContentBlockId { get; set; }

        [BsonElement("content_block_version")]
        public required int ContentBlockVersion { get; set; }

        [BsonElement("assessed_datetime")]
        public required DateTime AssessedDateTime { get; set; }
    }
}
