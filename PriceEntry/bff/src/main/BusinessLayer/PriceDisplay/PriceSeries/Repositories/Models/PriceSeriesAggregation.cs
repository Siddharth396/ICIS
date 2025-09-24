namespace BusinessLayer.PriceDisplay.PriceSeries.Repositories.Models
{
    using BusinessLayer.PriceDisplay.PriceSeriesSelection.Repositories.Models;

    using MongoDB.Bson.Serialization.Attributes;

    [BsonIgnoreExtraElements]
    public class PriceSeriesAggregation : PriceSeries
    {
        [BsonElement("series_short_name")]
        public Name? SeriesShortName { get; set; } = default!;

        [BsonElement("status")]
        public string Status { get; set; } = default!;

        [BsonElement("price_series_item")]
        public PriceSeriesItem? PriceSeriesItem { get; set; } = default!;

        [BsonElement("delta_type")]
        public PriceDeltaType? PriceDeltaType { get; set; } = default!;
    }
}