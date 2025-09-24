namespace BusinessLayer.PriceEntry.ValueObjects
{
    using System;

    public record AssessmentMethod
    {
        public static readonly AssessmentMethod PremiumDiscount = new("Premium/Discount");

        public static readonly AssessmentMethod Assessed = new("Assessed");

        private AssessmentMethod(string value)
        {
            Value = value;
        }

        public string Value { get; }

        public static bool IsValid(string? value)
        {
            return PremiumDiscount.Matches(value) || Assessed.Matches(value);
        }

        public bool Matches(string? value)
        {
            return string.Compare(
                       Value,
                       value,
                       StringComparison.Ordinal)
                   == 0;
        }
    }
}
