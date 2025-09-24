namespace BusinessLayer.PricingContentPackage.Repositories.Models
{
    using MongoDB.Bson.Serialization.Attributes;

    [BsonIgnoreExtraElements]
    public class Title
    {
        [BsonElement("locale")]
        public required string Locale { get; set; }

        [BsonElement("text")]
        public required string Text { get; set; }
    }
}