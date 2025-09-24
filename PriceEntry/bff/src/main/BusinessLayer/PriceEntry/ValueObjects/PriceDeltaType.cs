namespace BusinessLayer.PriceEntry.ValueObjects
{
    public record PriceDeltaType
    {
        public static readonly PriceDeltaType Regular = new("REGULAR");

        public static readonly PriceDeltaType NonMarketAdjustment = new("NONMKTADJ");

        private PriceDeltaType(string value)
        {
            Value = value;
        }

        public string Value { get; }
    }
}