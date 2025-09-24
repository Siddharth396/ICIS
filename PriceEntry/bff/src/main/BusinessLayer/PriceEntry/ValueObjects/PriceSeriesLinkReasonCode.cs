namespace BusinessLayer.PriceEntry.ValueObjects
{
    using System;

    public record PriceSeriesLinkReasonCode
    {
        public static readonly PriceSeriesLinkReasonCode HasSubsequentAssessmentForSameFulfilmentPeriod = new("subs-assmt-same-ffmt");

        public static readonly PriceSeriesLinkReasonCode IsConversionOf = new("is-conversion-of");

        private PriceSeriesLinkReasonCode(string value)
        {
            Value = value;
        }

        public string Value { get; }

        public bool Matches(string? value)
        {
            return string.Compare(Value, value, StringComparison.Ordinal) == 0;
        }
    }
}
