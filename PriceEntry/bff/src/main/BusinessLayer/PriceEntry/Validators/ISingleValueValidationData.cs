namespace BusinessLayer.PriceEntry.Validators
{
    public interface ISingleValueValidationData
    {
        decimal? AdjustedPriceDelta { get; }

        decimal? Price { get; }
    }
}
