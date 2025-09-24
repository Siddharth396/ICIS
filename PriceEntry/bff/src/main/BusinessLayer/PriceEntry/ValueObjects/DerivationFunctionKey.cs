namespace BusinessLayer.PriceEntry.ValueObjects
{
    public record DerivationFunctionKey(string Value)
    {
        public const string PeriodAvg = "period-avg";

        public const string RegionalAvg = "regional-avg";

        public static readonly DerivationFunctionKey PeriodAvgFunctionKey = new(PeriodAvg);

        public static readonly DerivationFunctionKey RegionalAvgFunctionKey = new(RegionalAvg);
    }
}
