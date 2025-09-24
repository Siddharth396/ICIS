namespace BusinessLayer.PriceDisplay.GridConfiguration.DTOs
{
    using HotChocolate;

    [GraphQLName("PriceDisplaySort")]
    public class Sort
    {
        public string Name { get; set; } = default!;

        public string Order { get; set; } = default!;
    }
}
