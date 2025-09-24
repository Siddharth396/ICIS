namespace BusinessLayer.PriceEntry.Repositories.Models
{
    using System.Diagnostics.CodeAnalysis;

    using BusinessLayer.PriceEntry.Validators;
    using BusinessLayer.PriceEntry.ValueObjects;

    using MongoDB.Bson.Serialization.Attributes;

    [BsonDiscriminator(SeriesItemTypeCode.CharterRateSingleValue)]
    [BsonIgnoreExtraElements]
    public class CharterRatePriceItemSingleValue : BasePriceItem, ICharterRateSingleValueValidationData
    {
        [BsonElement("price_value")]
        public decimal? Price { get; set; }

        [BsonElement("price_delta")]
        public decimal? PriceDelta { get; set; }

        [BsonElement("adjusted_price_delta")]
        public decimal? AdjustedPriceDelta { get; set; }

        [BsonElement("data_used")]
        public string? DataUsed { get; set; } = string.Empty;

        [ExcludeFromCodeCoverage(
            Justification =
                "There are no test scenarios where a Charter Rate Price is used as input series for a derived series")]
        public override decimal? GetPriceValue()
        {
            return Price;
        }

        protected override void CancelNonMarketAdjustment()
        {
            AdjustedPriceDelta = null;
        }
    }
}
