namespace BusinessLayer.PriceEntry.ValueObjects
{
    using System.Collections.Generic;

    public static class DataUsed
    {
        private static readonly HashSet<string> ValidValues = new()
        {
            "Bid/offer",
            "Transaction",
            "Spread",
            "Fundamentals",
            "Interpolation/extrapolation"
        };

        public static bool IsValid(string? value)
        {
            return value != null && ValidValues.Contains(value);
        }
    }
}
