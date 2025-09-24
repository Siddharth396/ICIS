namespace BusinessLayer.PriceEntry.ValueObjects
{
    using System;
    using System.Collections.Generic;

    public record PeriodTypeCode
    {
        public static readonly PeriodTypeCode HalfMonth = new("HM");

        public static readonly PeriodTypeCode MidMonth = new("MM");

        public static readonly PeriodTypeCode CalendarMonth = new("M");

        public static readonly PeriodTypeCode Quarterly = new("Q");

        private static readonly HashSet<string> AllowedReferencePeriodTypeCodes =
        [
           CalendarMonth.Value, Quarterly.Value
        ];

        private PeriodTypeCode(string value)
        {
            Value = value;
        }

        public string Value { get; }

        public bool Matches(string? value)
        {
            return string.Compare(
                       Value,
                       value,
                       StringComparison.Ordinal)
                   == 0;
        }

        public static bool IsAllowedReference(string? value)
        {
            return value != null && AllowedReferencePeriodTypeCodes.Contains(value);
        }
    }
}
