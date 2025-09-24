namespace BusinessLayer.PricingContentPackage.Repositories.Models
{
    using MongoDB.Bson.Serialization.Attributes;

    [BsonIgnoreExtraElements]
    public class Config
    {
        [BsonElement("key")]
        public required string Key { get; set; }

        [BsonElement("value")]
        public required string Value { get; set; }
    }
}