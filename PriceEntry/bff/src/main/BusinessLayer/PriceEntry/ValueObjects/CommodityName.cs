namespace BusinessLayer.PriceEntry.ValueObjects
{
    using System;

    public record CommodityName
    {
        public static readonly CommodityName LNG = new("lng");

        public static readonly CommodityName Styrene = new("styrene");

        public static readonly CommodityName Melamine = new("melamine");

        private CommodityName(string value)
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
