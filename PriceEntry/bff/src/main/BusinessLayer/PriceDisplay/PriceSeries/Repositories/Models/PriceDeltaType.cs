namespace BusinessLayer.PriceDisplay.PriceSeries.Repositories.Models
{
    using MongoDB.Bson.Serialization.Attributes;

    [BsonIgnoreExtraElements]
    public class PriceDeltaType
    {
        [BsonElement("guid")]
        public required string Guid { get; set; }

        [BsonElement("code")]
        public required string Code { get; set; } = default!;

        [BsonElement("is_deleted")]
        public required bool IsDeleted { get; set; }
    }
}
