namespace Infrastructure.Services.PeriodGenerator
{
    using System;
    using System.Collections.Generic;

    public class PeriodGeneratorOutput
    {
        public required DateOnly ReferenceDate { get; set; }

        public required List<AbsolutePeriod> AbsolutePeriods { get; set; }
    }
}
