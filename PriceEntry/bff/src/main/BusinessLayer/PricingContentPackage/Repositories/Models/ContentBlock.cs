namespace BusinessLayer.PricingContentPackage.Repositories.Models
{
    using MongoDB.Bson.Serialization.Attributes;

    [BsonIgnoreExtraElements]
    public class ContentBlock
    {
        [BsonElement("id")]
        public required string Id { get; set; }

        [BsonElement("name")]
        public required string Name { get; set; }

        [BsonElement("version")]
        public required string Version { get; set; }

        [BsonElement("capability_config")]
        public CapabilityConfig? CapabilityConfig { get; set; }

        [BsonElement("display_mode")]
        public string? DisplayMode { get; set; }

        [BsonElement("is_valid")]
        public bool? IsValid { get; set; }

        [BsonElement("last_published_date")]
        public long? LastPublishedDate { get; set; }

        [BsonElement("config")]
        public required Config[] Config { get; set; }

        [BsonElement("tags")]
        public required Tag[] Tags { get; set; }

        [BsonElement("sequence_id")]
        public required string SequenceId { get; set; }
    }
}