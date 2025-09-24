namespace BusinessLayer.PriceEntry.DTOs.Input
{
    public class SeriesItem
    {
        public decimal? AdjustedPriceDelta { get; set; }

        public decimal? AdjustedPriceHighDelta { get; set; }

        public decimal? AdjustedPriceLowDelta { get; set; }

        public string? AssessmentMethod { get; set; }

        public string? DataUsed { get; set; }

        public decimal? PremiumDiscount { get; set; }

        public decimal? Price { get; set; }

        public decimal? PriceHigh { get; set; }

        public decimal? PriceLow { get; set; }

        public string? ReferenceMarketName { get; set; }
    }
}
