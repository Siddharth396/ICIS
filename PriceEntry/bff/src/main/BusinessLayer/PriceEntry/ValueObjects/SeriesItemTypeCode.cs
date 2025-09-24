namespace BusinessLayer.PriceEntry.ValueObjects
{
    public record SeriesItemTypeCode(string Value)
    {
        public const string SingleValueWithReference = "pi-single-with-ref";

        public const string Range = "pi-range";

        public const string SingleValue = "pi-single";

        public const string CharterRateSingleValue = "cri-single";

        public const string ShippingCostSingleValue = "sci-single";

        public static readonly SeriesItemTypeCode SingleValueWithReferenceSeries = new(SingleValueWithReference);

        public static readonly SeriesItemTypeCode RangeSeries = new(Range);

        public static readonly SeriesItemTypeCode SingleValueSeries = new(SingleValue);

        public static readonly SeriesItemTypeCode CharterRateSingleValueSeries = new(CharterRateSingleValue);
    }
}
