namespace BusinessLayer.PriceEntry.Services.Factories
{
    using System;
    using System.Collections.Generic;

    using BusinessLayer.PriceEntry.ValueObjects;

    public static class SeriesItemTypeCodeFactory
    {
        private static readonly Dictionary<string, SeriesItemTypeCode> SeriesItemTypeCodes = new()
        {
            { SeriesItemTypeCode.SingleValueWithReference, SeriesItemTypeCode.SingleValueWithReferenceSeries },
            { SeriesItemTypeCode.Range, SeriesItemTypeCode.RangeSeries },
            { SeriesItemTypeCode.SingleValue, SeriesItemTypeCode.SingleValueSeries },
            { SeriesItemTypeCode.CharterRateSingleValue, SeriesItemTypeCode.CharterRateSingleValueSeries }
        };

        public static SeriesItemTypeCode GetSeriesItemTypeCode(string seriesItemTypeCode)
        {
            if (!SeriesItemTypeCodes.ContainsKey(seriesItemTypeCode))
            {
                throw new ArgumentException($"Series item type code {seriesItemTypeCode} is not supported");
            }

            return SeriesItemTypeCodes[seriesItemTypeCode];
        }
    }
}
