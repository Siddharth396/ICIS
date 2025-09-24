namespace BusinessLayer.Tests.Services.ContentBlock
{

    using Infrastructure.SQLDB.Models;
    using Infrastructure.SQLDB.Repositories;
    using Moq;
    using Serilog;
    using BusinessLayer.Services.TableTool;
    using Xunit;
    using BusinessLayer.DTO;
    using BusinessLayer.Validation;

    public class ContentBlockServiceTests
    {
        private readonly Mock<IGenericRepository<TableConfiguration>> mockGenericRepository;
        private readonly Mock<ILogger> mockLogger;
        private readonly ContentBlockService contentBlockService;
        private readonly Mock<ContentBlockValidationService> mockContentBlockValidationService;

        public ContentBlockServiceTests()
        {

            mockGenericRepository = new Mock<IGenericRepository<TableConfiguration>>();
            mockLogger = new Mock<ILogger>();
            mockContentBlockValidationService = new Mock<ContentBlockValidationService>();
            contentBlockService = new ContentBlockService(mockGenericRepository.Object, mockLogger.Object);
        }

        [Fact]
        public async Task Should_ReturnValidIncrementVersion_When_SavingContentBlockConfiguration()
        {
            var contentBlockInput = new SaveContentBlockRequest
            {
                ContentBlockId = "123-999",
                Filter = "{\"region\": \"Asia Minor\", \"product\": \"Styrene\", \"TableType\": \"Outage\"}",
            };

            var majorVersion = 0;
            var minorVersion = 0;


            mockGenericRepository.Setup(repo => repo.InsertAsync(It.IsAny<TableConfiguration>()))
                .Returns(Task.FromResult(1));

            // Act
            var response = await contentBlockService.SaveContentBlockConfiguration(contentBlockInput);

            // Assert
            Assert.NotNull(response);
            Assert.True(response.IsSuccess);
            Assert.Equal($"{majorVersion}.{minorVersion + 1}", response.Data.Version);
        }

        [Fact]
        public async Task Should_ReturnValidContentBlockId_When_SavingContentBlockConfiguration()
        {
            var contentBlockInput = new SaveContentBlockRequest
            {
                ContentBlockId = "123-777",
                Filter = "{\"region\": \"Asia Minor\", \"product\": \"Styrene\", \"TableType\": \"Outage\"}",
            };



            mockGenericRepository.Setup(repo => repo.InsertAsync(It.IsAny<TableConfiguration>()))
               .Returns(Task.FromResult(1));

            // Act
            var response = await contentBlockService.SaveContentBlockConfiguration(contentBlockInput);

            // Assert
            Assert.NotNull(response);
            Assert.True(response.IsSuccess);
            Assert.Equal(contentBlockInput.ContentBlockId, response.Data.ContentBlockId);

        }

        [Fact]
        public async Task Should_ReturnError_When_SavingContentBlockConfigurationWithInvalidRequest()
        {
            var contentBlockInput = new SaveContentBlockRequest
            {
                ContentBlockId = string.Empty,
                Filter = string.Empty,
            };

            var errorMessage = $"Could not save the SND Table Tool content block configuration with ContentBlockId : {contentBlockInput.ContentBlockId}.";


            mockGenericRepository.Setup(repo => repo.InsertAsync(It.IsAny<TableConfiguration>()))
               .Returns(Task.FromResult(0));

            // Act
            var response = await contentBlockService.SaveContentBlockConfiguration(contentBlockInput);

            // Assert
            Assert.Null(response.Data);
            Assert.True(response.IsFailure);
            Assert.Equal(errorMessage, response.Error.Message);


        }


        [Fact]

        public async Task Should_ReturnError_When_FetchingConfigurationWithInvalidContentBlockId()
        {
            var InvalidcontentBlockId = "123-777090";
            var validVersion = "1.0";
            var errorMessage = $"Could not find the SND Table Tool content block with ContentBlockId : {InvalidcontentBlockId} and Version: {validVersion}";

            var expectedData = new List<TableConfiguration>()
            { new TableConfiguration()
            { Id = 1, ContentBlockId = "123-777", MajorVersion = 1, MinorVersion = 0,
                Filter = "{\"region\": \"Asia Minor\", \"product\": \"Styrene\", \"TableType\": \"Outage\"}",  CreatedOn = DateTime.Now } };

            mockGenericRepository.Setup(repo => repo.GetQueryable())
                .Returns(expectedData.AsQueryable());


            var response = await contentBlockService.GetContentBlockConfiguration(InvalidcontentBlockId, validVersion);

            Assert.Null(response.Data);
            Assert.True(response.IsFailure);
            Assert.Equal(errorMessage, response.Error.Message);

        }


        [Fact]

        public async Task Should_ReturnError_When_FetchingConfigurationWithInvalidVersion()
        {
            var validcontentBlockId = "123-777";
            var InvalidVersion = "1.1";
            var errorMessage = $"Could not find the SND Table Tool content block with ContentBlockId : {validcontentBlockId} and Version: {InvalidVersion}";

            var expectedData = new List<TableConfiguration>()
            { new TableConfiguration()
            { Id = 1, ContentBlockId = "123-777", MajorVersion = 1, MinorVersion = 0,
                Filter = "{\"region\": \"Asia Minor\", \"product\": \"Styrene\", \"TableType\": \"Outage\"}",  CreatedOn = DateTime.Now } };

            mockGenericRepository.Setup(repo => repo.GetQueryable())
                .Returns(expectedData.AsQueryable());


            var response = await contentBlockService.GetContentBlockConfiguration(validcontentBlockId, InvalidVersion);

            Assert.Null(response.Data);
            Assert.True(response.IsFailure);
            Assert.Equal(errorMessage, response.Error.Message);

        }


        [Fact]

        public async Task Should_ReturnValidResponse_When_FetchingConfigurationWithValidContentBlockIdAndVersion()
        {
            var validcontentBlockId = "123-777";
            var validVersion = "1.1";


            var expectedData = new List<TableConfiguration>()
            { new TableConfiguration()
            { Id = 1, ContentBlockId = "123-777", MajorVersion = 1, MinorVersion = 1,
                Filter = "{\"region\": \"Asia Minor\", \"product\": \"Styrene\", \"TableType\": \"Outage\"}",  CreatedOn = DateTime.Now } };

            mockGenericRepository.Setup(repo => repo.GetQueryable())
                .Returns(expectedData.AsQueryable());


            var response = await contentBlockService.GetContentBlockConfiguration(validcontentBlockId, validVersion);

            Assert.True(response.IsSuccess);
            Assert.Equal(validcontentBlockId, response.Data.ContentBlockId);
            Assert.Equal(validVersion, response.Data.Version);
            Assert.Equal(expectedData.FirstOrDefault().Filter, response.Data.Filter);
        }
    }
}
