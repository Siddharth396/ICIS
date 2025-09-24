namespace BusinessLayer.PriceEntry.Repositories.Models
{
    using MongoDB.Bson.Serialization.Attributes;

    [BsonIgnoreExtraElements]
    public class CustomConfig
    {
        [BsonElement("price_delta")]
        public PriceDelta? PriceDelta { get; set; }
    }
}
