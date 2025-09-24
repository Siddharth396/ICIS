namespace BusinessLayer.PriceEntry.Repositories.Models
{
    using System.Diagnostics.CodeAnalysis;

    using BusinessLayer.PriceEntry.Validators;
    using BusinessLayer.PriceEntry.ValueObjects;

    using MongoDB.Bson.Serialization.Attributes;

    [BsonDiscriminator(SeriesItemTypeCode.Range)]
    [BsonIgnoreExtraElements]
    public class PriceItemRange : BasePriceItem, IRangeValidationData
    {
        [BsonElement("price_low")]
        public decimal? PriceLow { get; set; }

        [BsonElement("price_low_delta")]
        public decimal? PriceLowDelta { get; set; }

        [BsonElement("adjusted_price_low_delta")]
        public decimal? AdjustedPriceLowDelta { get; set; }

        [BsonElement("price_high")]
        public decimal? PriceHigh { get; set; }

        [BsonElement("price_high_delta")]
        public decimal? PriceHighDelta { get; set; }

        [BsonElement("adjusted_price_high_delta")]
        public decimal? AdjustedPriceHighDelta { get; set; }

        [BsonElement("price_mid")]
        public decimal? PriceMid { get; set; }

        [BsonElement("price_mid_delta")]
        public decimal? PriceMidDelta { get; set; }

        [BsonElement("adjusted_price_mid_delta")]
        public decimal? AdjustedPriceMidDelta { get; set; }

        [ExcludeFromCodeCoverage(
            Justification =
                "There are no test scenarios where a Range Price is used as input series for a derived series")]
        public override decimal? GetPriceValue()
        {
            return PriceMid;
        }

        protected override void CancelNonMarketAdjustment()
        {
            AdjustedPriceHighDelta = null;
            AdjustedPriceLowDelta = null;
            AdjustedPriceMidDelta = null;
        }
    }
}
