namespace BusinessLayer.PriceEntry.Validators
{
    public interface ICharterRateSingleValueValidationData
    {
        decimal? AdjustedPriceDelta { get; }

        string? DataUsed { get; }

        decimal? Price { get; }
    }
}
