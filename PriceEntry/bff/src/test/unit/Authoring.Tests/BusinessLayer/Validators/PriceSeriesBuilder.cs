namespace Authoring.Tests.BusinessLayer.Validators
{
    using System;

    using global::BusinessLayer.PriceEntry.DTOs.Output;
    using global::BusinessLayer.PriceEntry.ValueObjects;

    public static class PriceSeriesBuilder
    {
        public static class Range
        {
            public static PriceSeries New()
            {
                return new PriceSeries
                {
                    SeriesItemTypeCode = SeriesItemTypeCode.Range,
                    PriceLow = 10,
                    PriceHigh = 20
                };
            }
        }

        public static class SingleValueWithReference
        {
            public static PriceSeries NewAssessed()
            {
                return new PriceSeries
                {
                    SeriesItemTypeCode = SeriesItemTypeCode.SingleValueWithReference,
                    AssessmentMethod = AssessmentMethod.Assessed.Value,
                    DataUsed = "Bid/offer",
                    Price = 10
                };
            }

            public static PriceSeries NewPremiumDiscount()
            {
                return new PriceSeries
                {
                    SeriesItemTypeCode = SeriesItemTypeCode.SingleValueWithReference,
                    AssessmentMethod = AssessmentMethod.PremiumDiscount.Value,
                    DataUsed = "Bid/offer",
                    PremiumDiscount = 10,
                    ReferencePrice = new ReferencePrice()
                    {
                        Datetime = DateTime.Now,
                        Market = "Market",
                        PeriodLabel = "Period",
                        Price = 20,
                        SeriesName = "Series"
                    }
                };
            }
        }

        public static class CharterRateSingleValue
        {
            public static PriceSeries New()
            {
                return new PriceSeries
                {
                    SeriesItemTypeCode = SeriesItemTypeCode.CharterRateSingleValue,
                    Price = 10,
                    DataUsed = "Interpolation/extrapolation"
                };
            }
        }
    }
}
