namespace BusinessLayer.PriceEntry.Repositories.Models
{
    using BusinessLayer.PriceEntry.Validators;
    using BusinessLayer.PriceEntry.ValueObjects;

    using MongoDB.Bson.Serialization.Attributes;

    [BsonDiscriminator(SeriesItemTypeCode.SingleValue)]
    [BsonIgnoreExtraElements]
    public class PriceItemSingleValue : BasePriceItem, ISingleValueValidationData
    {
        [BsonElement("price_value")]
        public decimal? Price { get; set; }

        [BsonElement("price_delta")]
        public decimal? PriceDelta { get; set; }

        [BsonElement("adjusted_price_delta")]
        public decimal? AdjustedPriceDelta { get; set; }

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
