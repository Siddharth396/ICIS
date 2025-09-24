namespace BusinessLayer.PriceDisplay.PriceSeries.ValueObjects
{
    using System;

    public record PriceDeltaType
    {
        public static readonly PriceDeltaType Regular = new("REGULAR");

        public static readonly PriceDeltaType NonMarketAdjustment = new("NONMKTADJ");

        private PriceDeltaType(string value)
        {
            Value = value;
        }

        public string Value { get; }

        public bool Matches(string? value)
        {
            return string.Compare(
                       Value,
                       value,
                       StringComparison.OrdinalIgnoreCase)
                   == 0;
        }
    }
}
