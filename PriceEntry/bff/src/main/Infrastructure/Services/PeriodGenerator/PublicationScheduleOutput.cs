namespace Infrastructure.Services.PeriodGenerator
{
    using System.Collections.Generic;

    public class PublicationScheduleOutput
    {
        public required Schedule Schedule { get; set; }

        public IEnumerable<Event>? Events { get; set; }
    }
}
