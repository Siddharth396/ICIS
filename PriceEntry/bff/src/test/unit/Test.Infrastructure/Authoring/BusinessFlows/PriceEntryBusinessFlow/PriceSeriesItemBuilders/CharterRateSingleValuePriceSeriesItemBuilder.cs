namespace Test.Infrastructure.Authoring.BusinessFlows.PriceEntryBusinessFlow.PriceSeriesItemBuilders
{
    using BusinessLayer.PriceEntry.DTOs.Input;

    public static class CharterRateSingleValuePriceSeriesItemBuilder
    {
        public static SeriesItem BuildDefaultValidSeriesItem()
        {
            return new SeriesItem
            {
                Price = 10,
                DataUsed = "Interpolation/extrapolation"
            };
        }

        public static SeriesItem BuildInvalidSeriesItem()
        {
            return new SeriesItem
            {
                Price = 0
            };
        }
    }
}
