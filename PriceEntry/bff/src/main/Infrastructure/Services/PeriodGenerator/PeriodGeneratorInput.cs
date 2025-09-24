namespace Infrastructure.Services.PeriodGenerator
{
    using System;
    using System.Collections.Generic;

    public record PeriodGeneratorInput
    {
        public required DateOnly ReferenceDate { get; set; }

        public required List<string> PeriodCodes { get; set; }
    }
}
