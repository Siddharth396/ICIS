namespace Infrastructure.Services.PeriodGenerator
{
    using System;

    public class PublicationScheduleInput
    {
        public required string ScheduleId { get; set; }

        public required DateOnly StartDate { get; set; }

        public required DateOnly EndDate { get; set; }
    }
}
