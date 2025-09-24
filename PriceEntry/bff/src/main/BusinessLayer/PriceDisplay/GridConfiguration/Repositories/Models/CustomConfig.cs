namespace BusinessLayer.PriceDisplay.GridConfiguration.Repositories.Models
{
    using MongoDB.Bson.Serialization.Attributes;

    public class CustomConfig
    {
        [BsonElement("price_delta")]
        public PriceDelta? PriceDelta { get; set; }
    }
}
