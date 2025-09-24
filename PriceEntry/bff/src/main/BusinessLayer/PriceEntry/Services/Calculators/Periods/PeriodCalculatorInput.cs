namespace BusinessLayer.PriceEntry.Services.Calculators.Periods
{
    using System;
    using System.Collections.Generic;

    public record PeriodCalculatorInput
    {
        public required DateOnly ReferenceDate { get; init; }

        public required List<string> PeriodCodes { get; init; }
    }
}
