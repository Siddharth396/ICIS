namespace BusinessLayer.PriceEntry.ValueObjects
{
    using System;

    public record PriceDerivationType
    {
        public static readonly PriceDerivationType Average = new("AVERAGE");

        public static readonly PriceDerivationType Spread = new("SPREAD");

        public static readonly PriceDerivationType Converted = new("CONVERTED");

        public static readonly PriceDerivationType Index = new("INDEX");

        public static readonly PriceDerivationType Forecast = new("FORECAST");

        private PriceDerivationType(string value)
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
