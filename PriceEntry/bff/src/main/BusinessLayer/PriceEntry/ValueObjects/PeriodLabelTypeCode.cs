namespace BusinessLayer.PriceEntry.ValueObjects
{
    using System;

    public record PeriodLabelTypeCode
    {
        public static readonly PeriodLabelTypeCode ReferenceTime = new("plt-ref-time");

        public static readonly PeriodLabelTypeCode RelativeFulfilmentTime = new("plt-ffmt-time");

        public static readonly PeriodLabelTypeCode None = new("plt-none");

        private PeriodLabelTypeCode(string value)
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
    }
}
