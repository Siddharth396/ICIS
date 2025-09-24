namespace BusinessLayer.PriceEntry.Validators
{
    public interface IRangeValidationData
    {
        decimal? AdjustedPriceHighDelta { get; }

        decimal? AdjustedPriceLowDelta { get; }

        decimal? PriceHigh { get; }

        decimal? PriceLow { get; }
    }
}
