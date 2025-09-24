namespace Authoring.Tests.Infrastructure.Services
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;

    using FluentAssertions;

    using global::Infrastructure.Services.CanvasApi;

    using NSubstitute;

    using Serilog;

    using Test.Infrastructure.Mocks;

    using Xunit;

    public class CanvasApiServiceShould
    {
        private readonly CanvasApiSettings apiSettings;

        private readonly ILogger logger;

        public CanvasApiServiceShould()
        {
            apiSettings = new CanvasApiSettings
            {
                ContentPackagesEndpoint = "/content-packages",
                ReviewUrlPath = "/dummy",
                MaxRetries = 3,
                Timeout = TimeSpan.FromSeconds(30),
                BaseUrl = "http://test.com",
                VersionEndpoint = "/version",
                WorkspaceId = "pricing",
                Enabled = true
            };
            logger = Substitute.For<ILogger>();
        }

        [Fact]
        public async Task Return_As_Expected_When_Http_Response_Is_Success()
        {
            // Arrange
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK);

            var httpClient = GetHttpClientWithMockedResponse(responseMessage);

            var periodGeneratorService = new CanvasApiService(httpClient, apiSettings, logger);

            // Act
            var result = await periodGeneratorService.SendContentPackage(null);

            // Assert
            result.Should().BeTrue();
        }

        [Theory]
        [InlineData(HttpStatusCode.BadRequest)]
        [InlineData(HttpStatusCode.InternalServerError)]
        [InlineData(HttpStatusCode.NotFound)]
        public async Task Return_Empty_Periods_When_Http_Response_Is_Not_Success(HttpStatusCode httpStatusCode)
        {
            // Arrange
            var responseMessage = new HttpResponseMessage(httpStatusCode);

            var httpClient = GetHttpClientWithMockedResponse(responseMessage);

            var periodGeneratorService = new CanvasApiService(httpClient, apiSettings, logger);

            // Act
            var result = await periodGeneratorService.SendContentPackage(null);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task Not_Make_Requests_To_Canvas_When_Disabled()
        {
            // Arrange
            var handlerMock = new HttpMessageHandlerMock();
            var httpClient = new HttpClient(handlerMock);
            apiSettings.Enabled = false;

            var periodGeneratorService = new CanvasApiService(httpClient, apiSettings, logger);

            // Act
            var result = await periodGeneratorService.SendContentPackage(null);

            // Assert
            result.Should().BeTrue();
            handlerMock.Requests.Should().BeEmpty();
        }

        private HttpClient GetHttpClientWithMockedResponse(HttpResponseMessage responseMessage)
        {
            return HttpClientMock.GetHttpClientWithResponse(apiSettings.BaseUrl, responseMessage);
        }
    }
}
