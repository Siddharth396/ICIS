namespace BusinessLayer.PriceEntry.Repositories.Models
{
    using System.Diagnostics.CodeAnalysis;

    using BusinessLayer.PriceEntry.ValueObjects;
    using MongoDB.Bson.Serialization.Attributes;

    [ExcludeFromCodeCoverage(Justification = "Not supporting ShippingCostSingleValue series item type code")]
    [BsonDiscriminator(SeriesItemTypeCode.ShippingCostSingleValue)]
    [BsonIgnoreExtraElements]
    public class ShippingCostPriceItemSingleValue : BasePriceItem
    {
        [BsonElement("price_value")]
        public decimal? Price { get; set; }

        [BsonElement("price_delta")]
        public decimal? PriceDelta { get; set; }

        public override decimal? GetPriceValue()
        {
            return Price;
        }

        protected override void CancelNonMarketAdjustment()
        {
            throw new System.NotImplementedException();
        }
    }
}
