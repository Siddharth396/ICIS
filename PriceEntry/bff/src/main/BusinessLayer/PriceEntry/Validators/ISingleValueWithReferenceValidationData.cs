namespace BusinessLayer.PriceEntry.Validators
{
    public interface ISingleValueWithReferenceValidationData
    {
        decimal? AdjustedPriceDelta { get; }

        string? AssessmentMethod { get; }

        string? DataUsed { get; }

        decimal? PremiumDiscount { get; }

        decimal? Price { get; }

        decimal? ReferencePriceValue { get; }
    }
}
