namespace BusinessLayer.PriceEntry.Services
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using BusinessLayer.ContentBlock.DTOs.Output;
    using BusinessLayer.PriceSeriesSelection.Repositories.Models;

    using Infrastructure.Services.PeriodGenerator;

    using Serilog;

    public class PublicationScheduleService : IPublicationScheduleService
    {
        private readonly IPeriodGeneratorService periodGeneratorService;

        private readonly ILogger logger;

        public PublicationScheduleService(
            ILogger logger,
            IPeriodGeneratorService periodGeneratorService)
        {
            this.logger = logger.ForContext<PublicationScheduleService>();
            this.periodGeneratorService = periodGeneratorService;
        }

        public async Task<NextPublicationDate?> GetNextPublicationDate(PriceSeries priceSeries, DateTime assessedDateTime)
        {
            if (priceSeries.PublicationSchedules is null || priceSeries.PublicationSchedules.Count == 0)
            {
                logger.Information($"No publication schedules found for series {priceSeries.Id}");

                return null;
            }

            var publicationSchedule = priceSeries.PublicationSchedules.First();

            var assessedDateTimeDateOnly = DateOnly.FromDateTime(assessedDateTime);

            var publicationScheduleInput = new PublicationScheduleInput
            {
                ScheduleId = publicationSchedule.ScheduleId,
                StartDate = assessedDateTimeDateOnly,
                EndDate = assessedDateTimeDateOnly
            };

            var schedules = await periodGeneratorService.GetPublicationSchedules(publicationScheduleInput);

            if (schedules.Events == null || !schedules.Events.Any())
            {
                logger.Information($"No publication schedule events found for series {priceSeries.Id}");

                return new NextPublicationDate(null, schedules.Schedule.Id);
            }

            return new NextPublicationDate(schedules.Events.First().EventTime, schedules.Schedule.Id);
        }
    }
}
