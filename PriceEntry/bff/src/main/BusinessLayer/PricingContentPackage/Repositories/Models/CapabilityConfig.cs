namespace BusinessLayer.PricingContentPackage.Repositories.Models
{
    using MongoDB.Bson.Serialization.Attributes;

    [BsonIgnoreExtraElements]
    public class CapabilityConfig
    {
        [BsonElement("value")]
        public string? Value { get; set; }
    }
}