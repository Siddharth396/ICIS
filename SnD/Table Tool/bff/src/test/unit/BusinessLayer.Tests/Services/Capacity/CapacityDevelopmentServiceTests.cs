namespace BusinessLayer.Tests.Services.Capacity
{
    using BusinessLayer.Services.Capacity;
    using BusinessLayer.Services.Common;

    using Infrastructure.SQLDB.Models;
    using Infrastructure.SQLDB.Repositories;

    using Microsoft.Data.SqlClient;

    using Moq;

    using Serilog;

    using Xunit;

    public class CapacityDevelopmentServiceTests
    {
        [Fact]
        public async Task GetCapacityDevelopmentsByCommoditiesAndRegions_ValidParameters_ReturnsSuccessResponse()
        {
            // Arrange
            var commodities = "commodity1,commodity2";
            var regions = "region1,region2";
            var expectedCapacityDevelopments = new List<CapacityDevelopment>
            {
                new()
                {
                    Country = "USA",
                    Company = "Company A",
                    Site = "Site 1",
                    PlantNo = 1,
                    Type = "Expansion",
                    EstimatedStart = "Jan 2024",
                    NewAnnualCapacity = 1000.00m,
                    CapacityChange = 200.00m,
                    PercentChange = "25%",
                    LastUpdated = "01 Jan 2024"
                },
                new()
                {
                    Country = "Germany",
                    Company = "Company B",
                    Site = "Site 2",
                    PlantNo = 1,
                    Type = "Contraction",
                    EstimatedStart = "Feb 2024",
                    NewAnnualCapacity = 700.00m,
                    CapacityChange = -100.00m,
                    PercentChange = "-12.50%",
                    LastUpdated = "15 Feb 2024"
                },
                new()
                {
                    Country = "Japan",
                    Company = "Company C",
                    Site = "Site 3",
                    PlantNo = 2,
                    Type = "Expansion",
                    EstimatedStart = "Mar 2024",
                    NewAnnualCapacity = 200.00m,
                    CapacityChange = 100.00m,
                    PercentChange = "100%",
                    LastUpdated = "10 Mar 2024"
                }
            };

            var mockRepository = new Mock<IGenericRepository<CapacityDevelopment>>();
            mockRepository.Setup(mockRepository => mockRepository.ExecuteReadOnlySql(It.IsAny<string>(),
                It.IsAny<List<SqlParameter>>(),
                It.IsAny<int?>(), It.IsAny<int?>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedCapacityDevelopments);

            var mockLogger = new Mock<ILogger>();

            var mockSqlQueryReader = new Mock<ISqlQueryReader>();
            mockSqlQueryReader
                .Setup(reader => reader.ReadQuery("TableTool2.sql"))
                .Returns("SELECT * FROM CapacityDevelopment");

            var service = new CapacityDevelopmentService(mockRepository.Object, mockLogger.Object, mockSqlQueryReader.Object);

            // Act
            var response = await service.GetCapacityDevelopmentsByCommoditiesAndRegions(commodities, regions, null, null);

            // Assert
            Assert.True(response.IsSuccess);
            Assert.Equal(expectedCapacityDevelopments, response.Data);
        }

        [Fact]
        public async Task GetCapacityDevelopmentsByCommoditiesAndRegions_EmptyCommodities_ReturnsFailureResponse()
        {
            // Arrange
            var commodities = string.Empty;
            var regions = "region1,region2";

            var mockRepository = new Mock<IGenericRepository<CapacityDevelopment>>();
            var mockLogger = new Mock<ILogger>();
            var mockSqlQueryReader = new Mock<ISqlQueryReader>();

            var service = new CapacityDevelopmentService(mockRepository.Object, mockLogger.Object, mockSqlQueryReader.Object);

            // Act
            var response = await service.GetCapacityDevelopmentsByCommoditiesAndRegions(commodities, regions, null, null);

            // Assert
            Assert.True(response.IsFailure);
            Assert.Equal("Incorrect request parameter. Commodities and Region cannot be null or empty", response.Error.Message);
            Assert.Equal("400", response.Error.Code);
        }

        [Fact]
        public async Task GetCapacityDevelopmentsByCommoditiesAndRegion_InvalidPaging_ReturnsFailureResponse()
        {
            // Arrange
            var commodities = "Product1,Product2";
            var regions = "region1";
            int? pageNo = 0;
            int? pageSize = 0;

            var mockRepository = new Mock<IGenericRepository<CapacityDevelopment>>();
            var mockLogger = new Mock<ILogger>();
            var mockSqlQueryReader = new Mock<ISqlQueryReader>();

            var service = new CapacityDevelopmentService(mockRepository.Object, mockLogger.Object, mockSqlQueryReader.Object);

            // Act
            var response = await service.GetCapacityDevelopmentsByCommoditiesAndRegions(commodities, regions, pageNo, pageSize);

            // Assert
            Assert.True(response.IsFailure);
            Assert.Equal("Incorrect request parameter.PageNo and PageSize both should be either null or greater than zero.", response.Error.Message);
        }

        [Fact]
        public async Task GetCapacityDevelopmentsByCommoditiesAndRegions_FileNotFound_ReturnsFailureResponse()
        {
            // Arrange
            var commodities = "commodity1,commodity2";
            var regions = "region1,region2";
            int? pageNo = 1;
            int? pageSize = 10;

            var mockRepository = new Mock<IGenericRepository<CapacityDevelopment>>();
            var mockLogger = new Mock<ILogger>();

            var mockSqlQueryReader = new Mock<ISqlQueryReader>();
            mockSqlQueryReader
                .Setup(reader => reader.ReadQuery("TableTool2.sql"))
                .Throws(new FileNotFoundException("Script file 'TableTool2.sql' not found."));

            var service = new CapacityDevelopmentService(mockRepository.Object, mockLogger.Object, mockSqlQueryReader.Object);

            // Act
            var response = await service.GetCapacityDevelopmentsByCommoditiesAndRegions(commodities, regions, pageNo, pageSize);

            // Assert
            Assert.True(response.IsFailure);
            Assert.Equal("Some error occured while executing script.", response.Error.Message);
        }
    }
}
