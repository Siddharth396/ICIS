namespace Authoring.Tests.Infrastructure.Services
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Text.Json;
    using System.Threading.Tasks;

    using FluentAssertions;

    using global::Infrastructure.Services.PeriodGenerator;
    using NSubstitute;

    using Serilog;

    using Test.Infrastructure.Mocks;

    using Xunit;

    public class PeriodGeneratorServiceShould
    {
        private readonly PeriodGeneratorSettings periodGeneratorSettings;

        private readonly ILogger logger;

        public PeriodGeneratorServiceShould()
        {
            periodGeneratorSettings = new PeriodGeneratorSettings
            {
                PeriodsEndpoint = "/periods",
                SchedulesEndpoint = "/schedules",
                MaxRetries = 3,
                Timeout = TimeSpan.FromSeconds(30),
                BaseUrl = "http://test.com",
                VersionEndpoint = "/version"
            };
            logger = Substitute.For<ILogger>();
        }

        [Fact]
        public async Task Return_As_Expected_When_Http_Response_Is_Success()
        {
            // Arrange
            var referenceDate = DateOnly.MinValue;
            var fulfilmentPeriodCodes = new List<string> { "test" };
            var expected = new PeriodGeneratorOutput
            {
                ReferenceDate = referenceDate,
                AbsolutePeriods = new List<AbsolutePeriod>
                {
                    new()
                    {
                        Code = "test",
                        PeriodCode = "test",
                        Label = "test",
                        FromDate = referenceDate,
                        UntilDate = referenceDate
                    }
                }
            };
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(
                    JsonSerializer.Serialize(expected),
                    Encoding.UTF8,
                    "application/json")
            };

            var httpClient = GetHttpClientWithMockedResponse(responseMessage);

            var periodGeneratorService = new PeriodGeneratorService(httpClient, periodGeneratorSettings, logger);

            // Act
            var result = await periodGeneratorService.GeneratePeriods(referenceDate, fulfilmentPeriodCodes);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task Return_As_Expected_When_Calling_Publication_Schedules_And_Http_Response_Is_Success()
        {
            // Arrange
            var input = new PublicationScheduleInput
            {
                ScheduleId = "ScheduleId",
                StartDate = DateOnly.MinValue,
                EndDate = DateOnly.MaxValue
            };

            var expected = new PublicationScheduleOutput
            {
                Schedule = new Schedule { Id = "ScheduleId", Name = string.Empty },
                Events = new List<Event>
                {
                    new()
                    {
                        EventTime = DateTime.UtcNow,
                    },
                }
            };
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(
                    JsonSerializer.Serialize(expected),
                    Encoding.UTF8,
                    "application/json")
            };

            var httpClient = GetHttpClientWithMockedResponse(responseMessage);

            var periodGeneratorService = new PeriodGeneratorService(httpClient, periodGeneratorSettings, logger);

            // Act
            var result = await periodGeneratorService.GetPublicationSchedules(input);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [InlineData(HttpStatusCode.BadRequest)]
        [InlineData(HttpStatusCode.InternalServerError)]
        [InlineData(HttpStatusCode.NotFound)]
        public async Task Return_Empty_Periods_When_Http_Response_Is_Not_Success(HttpStatusCode httpStatusCode)
        {
            // Arrange
            var referenceDate = DateOnly.MinValue;
            var fulfilmentPeriodCodes = new List<string> { "test" };

            var responseMessage = new HttpResponseMessage(httpStatusCode);

            var httpClient = GetHttpClientWithMockedResponse(responseMessage);

            var periodGeneratorService = new PeriodGeneratorService(httpClient, periodGeneratorSettings, logger);

            // Act
            var result = await periodGeneratorService.GeneratePeriods(referenceDate, fulfilmentPeriodCodes);

            // Assert
            result.Should().NotBeNull();
            result.ReferenceDate.Should().Be(referenceDate);
            result.AbsolutePeriods.Should().BeEmpty();
        }

        [Theory]
        [InlineData(HttpStatusCode.BadRequest)]
        [InlineData(HttpStatusCode.InternalServerError)]
        [InlineData(HttpStatusCode.NotFound)]
        public async Task Return_Empty_Events_When_Calling_Publication_Schedules_And_Http_Response_Is_Not_Success(HttpStatusCode httpStatusCode)
        {
            // Arrange
            var input = new PublicationScheduleInput
            {
                ScheduleId = "ScheduleId",
                StartDate = DateOnly.MinValue,
                EndDate = DateOnly.MaxValue
            };

            var responseMessage = new HttpResponseMessage(httpStatusCode);

            var httpClient = GetHttpClientWithMockedResponse(responseMessage);

            var periodGeneratorService = new PeriodGeneratorService(httpClient, periodGeneratorSettings, logger);

            // Act
            var result = await periodGeneratorService.GetPublicationSchedules(input);

            // Assert
            result.Should().NotBeNull();
            result.Events.Should().BeEmpty();
        }

        private HttpClient GetHttpClientWithMockedResponse(HttpResponseMessage responseMessage)
        {
            return HttpClientMock.GetHttpClientWithResponse(periodGeneratorSettings.BaseUrl, responseMessage);
        }
    }
}
