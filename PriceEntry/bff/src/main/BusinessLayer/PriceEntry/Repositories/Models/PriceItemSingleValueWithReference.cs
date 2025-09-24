namespace BusinessLayer.PriceEntry.Repositories.Models
{
    using BusinessLayer.PriceEntry.Validators;
    using BusinessLayer.PriceEntry.ValueObjects;

    using MongoDB.Bson.Serialization.Attributes;

    [BsonDiscriminator(SeriesItemTypeCode.SingleValueWithReference)]
    [BsonIgnoreExtraElements]
    public class PriceItemSingleValueWithReference : BasePriceItem, ISingleValueWithReferenceValidationData
    {
        [BsonElement("price_value")]
        public decimal? Price { get; set; }

        [BsonElement("price_delta")]
        public decimal? PriceDelta { get; set; }

        [BsonElement("adjusted_price_delta")]
        public decimal? AdjustedPriceDelta { get; set; }

        [BsonElement("data_used")]
        public string? DataUsed { get; set; } = string.Empty;

        [BsonElement("assessment_method")]
        public string? AssessmentMethod { get; set; }

        [BsonElement("reference_price")]
        public ReferencePrice? ReferencePrice { get; set; }

        [BsonElement("premium_discount")]
        public decimal? PremiumDiscount { get; set; }

        [BsonIgnore]
        public decimal? ReferencePriceValue => ReferencePrice?.Price;

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
