namespace BusinessLayer.PriceEntry.Services.Calculators.Periods
{
    using System;

    public record PeriodCalculatorOutputItem
    {
        public required DateOnly ReferenceDate { get; init; }

        public required string Code { get; init; }

        public required string PeriodCode { get; init; }

        public required string Label { get; init; }

        public required DateOnly FromDate { get; set; }

        public DateOnly? UntilDate { get; set; }
    }
}
