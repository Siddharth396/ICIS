namespace Test.Infrastructure.Stubs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using global::Infrastructure.Services.PeriodGenerator;

    /// <summary>
    /// The periods here are set based on the default NOW date defined in TestData.cs.
    /// </summary>
    public class PeriodGeneratorServiceStub : IPeriodGeneratorService
    {
        public bool KeepAbsolutePeriodsEmpty { get; set; } = false;

        private List<AbsolutePeriod> AbsolutePeriods { get; set; } = new()!;

        private List<Event> Events { get; set; } = new()!;

        public Task<PeriodGeneratorOutput> GeneratePeriods(DateOnly referenceDate, List<string> periodCodes)
        {
            if (!AbsolutePeriods.Any() && !KeepAbsolutePeriodsEmpty)
            {
                AbsolutePeriods = GetDefaultAbsolutePeriods();
            }

            return Task.FromResult(new PeriodGeneratorOutput
            {
                ReferenceDate = referenceDate,
                AbsolutePeriods = AbsolutePeriods
            });
        }

        public Task<PublicationScheduleOutput> GetPublicationSchedules(PublicationScheduleInput input)
        {
           return Task.FromResult(new PublicationScheduleOutput
            {
                Schedule = new Schedule { Id = input.ScheduleId, Name = string.Empty },
                Events = Events
            });
        }

        public void ReplaceAbsolutePeriods(List<AbsolutePeriod> absolutePeriods)
        {
            AbsolutePeriods = absolutePeriods;
        }

        public void SetEvents(List<Event> events)
        {
            Events = events;
        }

        public void ClearAbsolutePeriods()
        {
            AbsolutePeriods.Clear();
        }

        public void ClearEvents()
        {
            Events.Clear();
        }

        public List<Event> GetDefaultEvents()
        {
            return new List<Event>
            {
                new()
                {
                    EventTime = AddDaysAndTime(7, 8, 30)
                },
                new()
                {
                    EventTime = AddDaysAndTime(14, 8, 30)
                },
                new()
                {
                    EventTime = AddDaysAndTime(21, 8, 30),
                },
                new()
                {
                    EventTime = AddDaysAndTime(28, 8, 30),
                }
            };
        }

        private static List<AbsolutePeriod> GetDefaultAbsolutePeriods()
        {
            return new List<AbsolutePeriod>
            {
                new()
                {
                    Code = "1H2402",
                    PeriodCode = "HM2",
                    Label = "1H February 2024",
                    FromDate = new DateOnly(2024, 02, 01),
                    UntilDate = new DateOnly(2024, 02, 15)
                },
                new()
                {
                    Code = "2H2402",
                    PeriodCode = "HM3",
                    Label = "2H February 2024",
                    FromDate = new DateOnly(2024, 02, 16),
                    UntilDate = new DateOnly(2024, 02, 29)
                },
                new()
                {
                    Code = "1H2403",
                    PeriodCode = "HM4",
                    Label = "1H March 2024",
                    FromDate = new DateOnly(2024, 03, 01),
                    UntilDate = new DateOnly(2024, 03, 15)
                },
                new()
                {
                    Code = "2H2403",
                    PeriodCode = "HM5",
                    Label = "2H March 2024",
                    FromDate = new DateOnly(2024, 03, 15),
                    UntilDate = new DateOnly(2024, 03, 31)
                },
                new()
                {
                    Code = "1H2404",
                    PeriodCode = "HM6",
                    Label = "1H April 2024",
                    FromDate = new DateOnly(2024, 04, 01),
                    UntilDate = new DateOnly(2024, 04, 15)
                },
                new()
                {
                    Code = "MM2402",
                    PeriodCode = "MM1",
                    Label = "February 2024",
                    FromDate = new DateOnly(2024, 02, 01),
                    UntilDate = new DateOnly(2024, 02, 29)
                },
                new()
                {
                    Code = "MM2403",
                    PeriodCode = "MM2",
                    Label = "March 2024",
                    FromDate = new DateOnly(2024, 03, 01),
                    UntilDate = new DateOnly(2024, 03, 31)
                },
                new()
                {
                    Code = "M2402",
                    PeriodCode = "M1",
                    Label = "February 2024",
                    FromDate = new DateOnly(2024, 02, 01),
                    UntilDate = new DateOnly(2024, 02, 29)
                },
                new()
                {
                    Code = "M2401",
                    PeriodCode = "M",
                    Label = "Jan 2024",
                    FromDate = new DateOnly(2024, 01, 01),
                    UntilDate = new DateOnly(2024, 01, 31)
                },
                new()
                {
                    Code = "Q2401",
                    PeriodCode = "Q",
                    Label = "Q1 2024",
                    FromDate = new DateOnly(2024, 01, 01),
                    UntilDate = new DateOnly(2024, 03, 31)
                }
            };
        }

        private static DateTime AddDaysAndTime(int days, int hours, int mins)
        {
            return TestData.TestData.Now.AddDays(days).AddHours(hours).AddMinutes(mins);
        }
    }
}
