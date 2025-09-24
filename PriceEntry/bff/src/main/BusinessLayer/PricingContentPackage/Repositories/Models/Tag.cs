namespace BusinessLayer.PricingContentPackage.Repositories.Models
{
    using MongoDB.Bson.Serialization.Attributes;

    [BsonIgnoreExtraElements]
    public class Tag
    {
        [BsonElement("tag_id")]
        public required string TagId { get; set; }

        [BsonElement("type")]
        public required string Type { get; set; }

        [BsonElement("category")]
        public string? Category { get; set; }
    }
}