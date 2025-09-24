namespace Infrastructure.Services.PeriodGenerator
{
    using System;

    public class AbsolutePeriod
    {
        public required string Code { get; init; }

        public required string Label { get; set; }

        public required DateOnly UntilDate { get; set; }

        public required DateOnly FromDate { get; set; }

        public required string PeriodCode { get; set; }
    }
}
