namespace Authoring.Tests.Application.ContentBlockConfiguration.Query
{
    using System.Threading.Tasks;

    using BusinessLayer.DTOs;
    using BusinessLayer.Services.TableTool;
    using BusinessLayer.Validation;

    using HotChocolate.Resolvers;

    using global::Infrastructure.GraphQL;

    using Moq;

    using Test.Infrastructure.GraphQL;

    using Xunit;
    using Authoring.Application.ContentBlock.Query;
    using BusinessLayer.DTO;
    using BusinessLayer.Services.Models;

    [Collection("Authoring stub collection")]

    public class ContentBlockQueriesShould
    {

        private readonly QueryExecutor execute;
        private readonly Mock<IErrorReporter> errorReporter;
        private readonly Mock<IResolverContext> resolverContext;
        private readonly ContentBlockQueries contentBlockQueries;
        private readonly Mock<ContentBlockRequest> contentBlockRequest;
        private readonly Mock<IContentBlockService> contentBlockService;
        private readonly Mock<IContentBlockValidationService> validationService;

        public ContentBlockQueriesShould(AuthoringStubFixture fixture)
        {
            execute = fixture.Executor;
            errorReporter = new Mock<IErrorReporter>();
            resolverContext = new Mock<IResolverContext>();
            contentBlockService = new Mock<IContentBlockService>();
            contentBlockRequest = new Mock<ContentBlockRequest>();
            validationService = new Mock<IContentBlockValidationService>();
            contentBlockQueries = new ContentBlockQueries(errorReporter.Object);
        }


        [Fact]

        public async Task ReturnValidResponse_When_FetchingContentBlockConfigurationWithValidContentBlockIdAndVersion()
        {
            // Arrange
            var contentBlockId = "2322-09101-BSHD-249282";
            var version = "1.0";

            var contentBlockResponse = new ContentBlockResponse()
            {
                ContentBlockId = contentBlockId,
                Version = version,
                Filter = "{\"region\": \"Asia Minor\", \"product\": \"Styrene\", \"TableType\": \"Outage\"}"
            };

            var validationResult = new ValidationResponse()
            {
                Status = true,
                StatusCode = string.Empty,
                ValidationMessage = string.Empty
            };

            validationService.Setup(v => v.ValidateContentBlockRequest(It.IsAny<ContentBlockRequest>())).Returns(validationResult);

            contentBlockService.Setup(x => x.GetContentBlockConfiguration(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(Response<ContentBlockResponse>.Success(contentBlockResponse));

            // Act
            var response = await contentBlockQueries.GetContentBlock(resolverContext.Object, contentBlockRequest.Object, contentBlockService.Object, validationService.Object);

            // Assert
            Assert.NotNull(response);
            Assert.Equal(contentBlockResponse.ContentBlockId, response.ContentBlockId);
            Assert.Equal(contentBlockResponse.Version, response.Version);
            Assert.Equal(contentBlockResponse.Filter, response.Filter);

        }


        [Fact]

        public async Task ReturnNull_When_FetchingContentBlockConfigurationWithFailedValidation()
        {
            // Arrange
            var validationResult = new ValidationResponse()
            {
                Status = false,
                StatusCode = "400",
                ValidationMessage = "Incorrect request parameter. Please provide all required parameters."
            };

            validationService.Setup(v => v.ValidateContentBlockRequest(It.IsAny<ContentBlockRequest>())).Returns(validationResult);

            // Act
            var response = await contentBlockQueries.GetContentBlock(resolverContext.Object, contentBlockRequest.Object, contentBlockService.Object, validationService.Object);

            // Assert
            Assert.Null(response);

        }


        [Fact]
        public async Task ReturnNull_When_FetchingContentBlockConfigurationWithInvalidContentBlockAndVersion()
        {
            // Arrange
            var InvalidContentBlockId = "2322-09101-BSHD-249282";
            var InvalidVersion = "1.0";
            var errorMessage = $"Could not find the SND Table Tool content block with ContentBlockId : {InvalidContentBlockId} and Version: {InvalidVersion}";
            var errorStatusCode = "404";

            var validationResult = new ValidationResponse()
            {
                Status = true,
                StatusCode = string.Empty,
                ValidationMessage = string.Empty
            };

            validationService.Setup(v => v.ValidateContentBlockRequest(It.IsAny<ContentBlockRequest>())).Returns(validationResult);

            contentBlockService.Setup(x => x.GetContentBlockConfiguration(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(Response<ContentBlockResponse>.Failure(errorMessage, errorStatusCode));

            // Act
            var response = await contentBlockQueries.GetContentBlock(resolverContext.Object, contentBlockRequest.Object, contentBlockService.Object, validationService.Object);

            // Assert
            Assert.Null(response);

        }
    }
}
