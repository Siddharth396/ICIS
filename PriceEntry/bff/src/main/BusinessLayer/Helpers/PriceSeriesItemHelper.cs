namespace BusinessLayer.Helpers
{
    using System.Globalization;

    public static class PriceSeriesItemHelper
    {
        public static string? FormatPrice(this decimal? price)
        {
            var isWholeNumber = (price % 1 == 0);
            return isWholeNumber
                       ? price?.ToString("#,##0", NumberFormatInfo.InvariantInfo)
                       : price?.ToString("#,##0.00000", NumberFormatInfo.InvariantInfo);
        }
    }
}
