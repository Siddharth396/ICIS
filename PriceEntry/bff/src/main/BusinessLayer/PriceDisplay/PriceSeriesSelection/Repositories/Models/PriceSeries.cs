namespace BusinessLayer.PriceDisplay.PriceSeriesSelection.Repositories.Models
{
    using System;

    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    [BsonIgnoreExtraElements]
    public class PriceSeries
    {
        [BsonId]
        public string Id { get; set; } = default!;

        [BsonElement("series_name")]
        public Name SeriesName { get; set; } = default!;

        [BsonElement("series_item_type_code")]
        public string SeriesItemTypeCode { get; set; } = default!;

        [BsonElement("commodity")]
        public Commodity Commodity { get; set; } = default!;

        [BsonElement("currency_unit")]
        public CurrencyUnit CurrencyUnit { get; set; } = default!;

        [BsonElement("measure_unit")]
        public MeasureUnit MeasureUnit { get; set; } = default!;

        [BsonElement("item_frequency")]
        public ItemFrequency ItemFrequency { get; set; } = default!;

        [BsonElement("location")]
        public Location Location { get; set; } = default!;

        [BsonElement("price_settlement_type")]
        public PriceSettlementType? PriceSettlementType { get; set; } = default!;

        [BsonElement("price_category")]
        public PriceCategory PriceCategory { get; set; } = default!;

        [BsonElement("launch_date")]
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime LaunchDate { get; set; } = default!;

        [BsonElement("termination_date")]
        public DateTime? TerminationDate { get; set; }

        internal string GetPriceSeriesName(string name, DateTime currentDateTime)
        {
            var currentDate = DateOnly.FromDateTime(currentDateTime);
            var launchDate = DateOnly.FromDateTime(LaunchDate);

            var isPreLaunch = currentDate < launchDate;
            if (isPreLaunch)
            {
                return $"Pre-Launch_{name}";
            }

            if (TerminationDate.HasValue)
            {
                var terminationDate = DateOnly.FromDateTime(TerminationDate.Value);
                var isTerminated = currentDate > terminationDate;

                if (isTerminated)
                {
                    return $"Terminated_{name}";
                }
            }

            return name;
        }
    }
}
