namespace BusinessLayer.PriceDisplay.GridConfiguration.DTOs
{
    using HotChocolate;

    [GraphQLName("PriceDisplayPriceDelta")]
    public class PriceDelta
    {
        public string PriceField { get; set; } = default!;

        public string PriceDeltaField { get; set; } = default!;

        public string PrecisionField { get; set; } = default!;
    }
}
