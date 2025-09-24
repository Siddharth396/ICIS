namespace BusinessLayer.PriceDisplay.GridConfiguration.Repositories.Models
{
    using System.Collections.Generic;

    using MongoDB.Bson.Serialization.Attributes;

    [BsonIgnoreExtraElements]
    public record AlternateField
    {
        [BsonElement("series_item_type_codes")]
        public required IEnumerable<string> SeriesItemTypeCodes { get; init; }

        [BsonElement("field")]
        public required string Field { get; init; }

        [BsonElement("price_delta_field")]
        public string? PriceDeltaField { get; init; }
    }
}