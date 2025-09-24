namespace Infrastructure.Services.PeriodGenerator
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.WebUtilities;
    using Serilog;

    public class PeriodGeneratorService : IPeriodGeneratorService
    {
        private readonly HttpClient httpClient;

        private readonly PeriodGeneratorSettings periodGeneratorSettings;

        private readonly ILogger logger;

        public PeriodGeneratorService(
            HttpClient httpClient,
            PeriodGeneratorSettings periodGeneratorSettings,
            ILogger logger)
        {
            this.httpClient = httpClient;
            this.periodGeneratorSettings = periodGeneratorSettings;
            this.logger = logger.ForContext<PeriodGeneratorService>();
        }

        public async Task<PeriodGeneratorOutput> GeneratePeriods(
            DateOnly referenceDate,
            List<string> periodCodes)
        {
            logger.Debug("Preparing to make request to Period Generator Service");

            var response = await httpClient.PostAsJsonAsync(
                               periodGeneratorSettings.GetPeriodsEndpointFullUrl(),
                               new PeriodGeneratorInput
                               {
                                   ReferenceDate = referenceDate,
                                   PeriodCodes = periodCodes
                               });

            logger.Debug($"Request to Period Generator Service Done, status code is {response.StatusCode}");

            if (response.IsSuccessStatusCode)
            {
                return (await response.Content.ReadFromJsonAsync<PeriodGeneratorOutput>())!;
            }

            logger.ForContext("Scenario", "PeriodGeneratorServiceFailedRequestV2")
               .ForContext("StatusCode", response.StatusCode)
               .Error("Failed to generate periods for reference date {referenceDate} and periodCodes {periodCodes}", referenceDate, periodCodes);

            return new PeriodGeneratorOutput
            {
                ReferenceDate = referenceDate,
                AbsolutePeriods = []
            };
        }

        public async Task<PublicationScheduleOutput> GetPublicationSchedules(PublicationScheduleInput input)
        {
            logger.Information($"Preparing to make request to calendar service for ScheduleId {input.ScheduleId}");

            var queryParams = new Dictionary<string, string?>
            {
                { "startDate", input.StartDate.ToString("yyyy-MM-dd", DateTimeFormatInfo.InvariantInfo) },
                { "endDate", input.EndDate.ToString("yyyy-MM-dd", DateTimeFormatInfo.InvariantInfo) }
            };

            var baseUrl = periodGeneratorSettings.GetSchedulesEndpointFullUrl(input.ScheduleId);

            var response = await httpClient.GetAsync(QueryHelpers.AddQueryString(baseUrl, queryParams));

            logger.Information($"Calendar service responded with status code {response.StatusCode}");

            if (response.IsSuccessStatusCode)
            {
                return (await response.Content.ReadFromJsonAsync<PublicationScheduleOutput>())!;
            }

            logger.ForContext("Scenario", "CalendarServiceFailedRequest")
              .ForContext("StatusCode", response.StatusCode)
              .Error($"Failed to get publication schedules for ScheduleId {input.ScheduleId}");

            return new PublicationScheduleOutput
            {
                Schedule = new Schedule { Id = input.ScheduleId, Name = string.Empty },
                Events = new List<Event>()
            };
        }
    }
}
