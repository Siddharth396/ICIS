namespace BusinessLayer.PricingContentPackage.Repositories.Models
{
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    [BsonIgnoreExtraElements]
    public class ContentPackage
    {
        [BsonId]
        [BsonElement("_id")]
        [BsonRepresentation(BsonType.String)]
        public required string ContentPackageId { get; set; }

        [BsonElement("version")]
        public required int Version { get; set; }

        [BsonElement("revision")]
        public required int Revision { get; set; }

        [BsonElement("applies_from")]
        public required long AppliesFrom { get; set; }

        [BsonElement("created_on")]
        public required long CreatedOn { get; set; }

        [BsonElement("created_by")]
        public required string CreatedBy { get; set; }

        [BsonElement("modified_on")]
        public required long ModifiedOn { get; set; }

        [BsonElement("modified_by")]
        public required string ModifiedBy { get; set; }

        [BsonElement("published_on")]
        public long? PublishedOn { get; set; }

        [BsonElement("published_by")]
        public string? PublishedBy { get; set; }

        [BsonElement("status")]
        public required string Status { get; set; }

        [BsonElement("title")]
        public required Title Title { get; set; }

        [BsonElement("contents")]
        public required Contents Contents { get; set; }

        [BsonElement("tags")]
        public required Tag[] Tags { get; set; }

        [BsonElement("metadata")]
        public required Config[] Metadata { get; set; }

        [BsonElement("sequence_id")]
        public required string SequenceId { get; set; }
    }
}
