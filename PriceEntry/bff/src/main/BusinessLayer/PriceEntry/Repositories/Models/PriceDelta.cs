namespace BusinessLayer.PriceEntry.Repositories.Models
{
    using MongoDB.Bson.Serialization.Attributes;

    [BsonIgnoreExtraElements]
    public class PriceDelta
    {
        [BsonElement("price_field")]
        public string PriceField { get; set; } = default!;

        [BsonElement("price_delta_field")]
        public string PriceDeltaField { get; set; } = default!;

        [BsonElement("precision_field")]
        public string PrecisionField { get; set; } = default!;
    }
}
