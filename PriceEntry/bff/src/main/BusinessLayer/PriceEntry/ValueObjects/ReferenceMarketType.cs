namespace BusinessLayer.PriceEntry.ValueObjects
{
    using System;

    public record ReferenceMarketType
    {
        public static readonly ReferenceMarketType LNG = new("lng");

        public static readonly ReferenceMarketType Gas = new("gas");

        private ReferenceMarketType(string value)
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
