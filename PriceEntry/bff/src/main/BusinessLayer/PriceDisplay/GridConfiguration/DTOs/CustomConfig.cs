namespace BusinessLayer.PriceDisplay.GridConfiguration.DTOs
{
    using HotChocolate;

    [GraphQLName("PriceDisplayCustomConfig")]
    public class CustomConfig
    {
        public PriceDelta? PriceDelta { get; set; } = default!;
    }
}
