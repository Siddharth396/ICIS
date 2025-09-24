namespace BusinessLayer.PriceEntry.ValueObjects
{
    using System;

    public record PriceCategoryCode
    {
        public static readonly PriceCategoryCode Derived = new("DERIVED");

        public static readonly PriceCategoryCode Assessed = new("ASSESSED");

        public static readonly PriceCategoryCode MarketReported = new("MKTRPT");

        private PriceCategoryCode(string value)
        {
            Value = value;
        }

        public string Value { get; }

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
