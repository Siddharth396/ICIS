namespace BusinessLayer.PriceEntry.ValueObjects
{
    using System.Collections.Generic;

    public record ReferencePeriodCode
    {
        public const string Monthly = "M";

        public const string Quarterly = "Q";

        private static readonly HashSet<string> AllowedPeriodCodes =
        [
            Monthly, Quarterly
        ];

        public static bool IsAllowed(string? value)
        {
            return value != null && AllowedPeriodCodes.Contains(value);
        }
    }
}
