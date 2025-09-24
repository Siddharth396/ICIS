namespace Infrastructure.Services.PeriodGenerator
{
    using System;

    public class RequestedInterval
    {
        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }
    }
}
