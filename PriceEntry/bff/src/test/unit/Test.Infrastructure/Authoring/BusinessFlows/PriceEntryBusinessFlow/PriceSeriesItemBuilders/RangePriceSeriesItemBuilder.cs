namespace Test.Infrastructure.Authoring.BusinessFlows.PriceEntryBusinessFlow.PriceSeriesItemBuilders
{
    using BusinessLayer.PriceEntry.DTOs.Input;

    public static class RangePriceSeriesItemBuilder
    {
        public static SeriesItem BuildDefaultValidSeriesItem(int low = 10, int high = 20)
        {
            return new SeriesItem { PriceLow = low, PriceHigh = high };
        }

        public static SeriesItem SetNonMarketAdjustmentValues(SeriesItem item)
        {
            item.AdjustedPriceHighDelta = 12.2m;
            item.AdjustedPriceLowDelta = 1.1m;
            return item;
        }
    }
}
