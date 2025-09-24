namespace Test.Infrastructure.Authoring.BusinessFlows.PriceEntryBusinessFlow.PriceSeriesItemBuilders
{
    using BusinessLayer.PriceEntry.DTOs.Input;
    using BusinessLayer.PriceEntry.ValueObjects;

    public static class SingleValueWithReferencePriceSeriesItemBuilder
    {
        public static SeriesItem BuildDefaultValidSeriesItem()
        {
            return new SeriesItem
            {
                AssessmentMethod = AssessmentMethod.PremiumDiscount.Value,
                Price = 10.1M,
                ReferenceMarketName = "TTF",
                PremiumDiscount = 0.1M,
                DataUsed = "Transaction"
            };
        }

        public static SeriesItem BuildValidSeriesItemWithAssessedPrice(int price = 10)
        {
            return new SeriesItem
            {
                AssessmentMethod = AssessmentMethod.Assessed.Value,
                Price = price,
                DataUsed = "Transaction"
            };
        }

        public static SeriesItem BuildValidSeriesItemWithPremiumDiscount(string referenceMarketName)
        {
            return new SeriesItem
            {
                AssessmentMethod = AssessmentMethod.PremiumDiscount.Value,
                Price = 10,
                PremiumDiscount = 5,
                ReferenceMarketName = referenceMarketName,
                DataUsed = "Bid/offer"
            };
        }

        public static SeriesItem BuildInvalidSeriesItem()
        {
            return new SeriesItem
            {
                AssessmentMethod = AssessmentMethod.Assessed.Value,
                Price = 0
            };
        }
    }
}
