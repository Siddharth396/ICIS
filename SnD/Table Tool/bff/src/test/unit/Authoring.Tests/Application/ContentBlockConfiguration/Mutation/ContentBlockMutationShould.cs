namespace Authoring.Tests.Application.ContentBlockConfiguration.Mutation
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
    using BusinessLayer.DTO;
    using BusinessLayer.Services.Models;
    using Authoring.Application.ContentBlock.Mutation;

    [Collection("Authoring stub collection")]


    public class ContentBlockMutationShould
    {


        private readonly QueryExecutor execute;
        private readonly Mock<IErrorReporter> errorReporter;
        private readonly Mock<IResolverContext> resolverContext;
        private readonly ContentBlockMutations contentMutations;
        private readonly Mock<SaveContentBlockRequest> saveContentBlockRequest;
        private readonly Mock<IContentBlockService> contentBlockService;
        private readonly Mock<IContentBlockValidationService> validationService;


        public ContentBlockMutationShould(AuthoringStubFixture fixture)
        {

            execute = fixture.Executor;
            errorReporter = new Mock<IErrorReporter>();
            resolverContext = new Mock<IResolverContext>();
            contentBlockService = new Mock<IContentBlockService>();
            saveContentBlockRequest = new Mock<SaveContentBlockRequest>();
            validationService = new Mock<IContentBlockValidationService>();
            contentMutations = new ContentBlockMutations(errorReporter.Object);
        }

        [Fact]

        public async Task ReturnValidResponse_When_SavingContentBlockConfigurationWithValidRequest()
        {
            // Arrange
            var validationResult = new ValidationResponse()
            {
                Status = true,
                StatusCode = string.Empty,
                ValidationMessage = string.Empty
            };

            var saveContentBlockResponse = new SaveContentBlockResponse()
            {
                ContentBlockId = "2322-09101-BSHD-249282",
                Version = "1.0",
            };

            validationService.Setup(v => v.ValidateContentBlockInput(It.IsAny<SaveContentBlockRequest>())).Returns(validationResult);

            contentBlockService.Setup(x => x.SaveContentBlockConfiguration(It.IsAny<SaveContentBlockRequest>())).ReturnsAsync(Response<SaveContentBlockResponse>.Success(saveContentBlockResponse));

            // Act
            var response = await contentMutations.SaveContentBlock(resolverContext.Object, saveContentBlockRequest.Object, contentBlockService.Object, validationService.Object);

            // Assert
            Assert.NotNull(response);
            Assert.Equal(saveContentBlockResponse.ContentBlockId, response.ContentBlockId);
            Assert.Equal(saveContentBlockResponse.Version, response.Version);
        }

        [Fact]

        public async Task ReturnNull_When_SavingContentBlockConfigurationWithFailedValidation()
        {
            // Arrange
            var validationResult = new ValidationResponse()
            {
                Status = false,
                StatusCode = "400",
                ValidationMessage = "Incorrect request parameter. Please provide all required parameters."
            };

            validationService.Setup(v => v.ValidateContentBlockInput(It.IsAny<SaveContentBlockRequest>())).Returns(validationResult);

            // Act
            var response = await contentMutations.SaveContentBlock(resolverContext.Object, saveContentBlockRequest.Object, contentBlockService.Object, validationService.Object);

            // Assert
            Assert.Null(response);
        }

        [Fact]
        public async Task ReturnNull_When_SavingContentBlockConfigurationFails()
        {
            // Arrange
            var validationResult = new ValidationResponse()
            {
                Status = true,
                StatusCode = string.Empty,
                ValidationMessage = string.Empty
            };

            var version = "1.0";
            var errorMessage = $"Could not save the SND Table Tool content block configuration with ContentBlockId : {version}.";
            var errorStatusCode = "500";

            validationService.Setup(v => v.ValidateContentBlockInput(It.IsAny<SaveContentBlockRequest>())).Returns(validationResult);
            contentBlockService.Setup(x => x.SaveContentBlockConfiguration(It.IsAny<SaveContentBlockRequest>())).ReturnsAsync(Response<SaveContentBlockResponse>.Failure(errorMessage, errorStatusCode));

            // Act
            var response = await contentMutations.SaveContentBlock(resolverContext.Object, saveContentBlockRequest.Object, contentBlockService.Object, validationService.Object);

            // Assert
            Assert.Null(response);
        }
    }
}
