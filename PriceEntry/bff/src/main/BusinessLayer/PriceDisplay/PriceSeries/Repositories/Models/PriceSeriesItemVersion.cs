namespace BusinessLayer.PriceDisplay.PriceSeries.Repositories.Models
{
    using Infrastructure.MongoDB.Models;

    using MongoDB.Bson.Serialization.Attributes;

    [BsonIgnoreExtraElements]
    public class PriceSeriesItemVersion
    {
        [BsonElement("price_value")]
        public decimal? Price { get; set; } = default!;

        [BsonElement("price_low")]
        public decimal? PriceLow { get; set; } = default!;

        [BsonElement("price_high")]
        public decimal? PriceHigh { get; set; } = default!;

        [BsonElement("price_mid")]
        public decimal? PriceMid { get; set; } = default!;

        [BsonElement("last_modified")]
        public required AuditInfo LastModified { get; set; }
    }
}
