namespace BusinessLayer.PriceEntry.ValueObjects
{
    using System.Collections.Generic;

    public record RelativeFulfilmentPeriodCode
    {
        public const string MonthPlusOne = "M1";

        public const string FifteenFortyFiveDays = "15-45D";

        public const string TwentyFortyDays = "20-40D";

        public static RelativeFulfilmentPeriodCode None = new(string.Empty);

        private static readonly HashSet<string> AllowedPeriodCodes =
        [
            "HM0", "HM1", "HM2", "HM3", "HM4", "HM5", "HM6",
            "MM1", "MM2",
            "M-1", "M-2", "M0", "M1", "M2", "M3", "M4", "M5", "M6", "M7"
        ];

        private RelativeFulfilmentPeriodCode(string code)
        {
            Value = code;
        }

        public string Value { get; }

        public static bool IsAllowed(string? value)
        {
            return value != null && AllowedPeriodCodes.Contains(value);
        }
    }
}
