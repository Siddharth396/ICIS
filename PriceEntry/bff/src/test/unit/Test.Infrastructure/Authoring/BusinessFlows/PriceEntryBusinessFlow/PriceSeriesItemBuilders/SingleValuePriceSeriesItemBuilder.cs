namespace Test.Infrastructure.Authoring.BusinessFlows.PriceEntryBusinessFlow.PriceSeriesItemBuilders
{
    using BusinessLayer.PriceEntry.DTOs.Input;

    public static class SingleValuePriceSeriesItemBuilder
    {
        public static SeriesItem BuildDefaultValidSeriesItem(int price = 10)
        {
            return new SeriesItem { Price = price };
        }

        public static SeriesItem SetNonMarketAdjustmentValues(SeriesItem item)
        {
            item.AdjustedPriceDelta = 1.2m;
            return item;
        }
    }
}
