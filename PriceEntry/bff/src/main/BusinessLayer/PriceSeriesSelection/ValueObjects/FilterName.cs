namespace BusinessLayer.PriceSeriesSelection.ValueObjects
{
    public record FilterName
    {
        public static readonly FilterName Commodity = new("commodity");

        public static readonly FilterName Region = new("region");

        public static readonly FilterName PriceCategory = new("price-category");

        public static readonly FilterName PriceSettlementType = new("price-settlement-type");

        public static readonly FilterName Frequency = new("frequency");

        private FilterName(string value)
        {
            Value = value;
        }

        public string Value { get; }
    }
}
