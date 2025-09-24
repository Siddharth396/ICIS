namespace BusinessLayer.Tests.Services.Outage
{
    using BusinessLayer.Services.Common;
    using Infrastructure.SQLDB.Repositories;
    using Infrastructure.SQLDB.Models;
    using Moq;
    using Xunit;
    using BusinessLayer.Services.Outage;
    using Serilog;
    using Microsoft.Data.SqlClient;

    public class OutageServiceTests
    {
        [Fact]
        public async Task GetOutagesByCommoditiesAndRegions_ValidParameters_ReturnsSuccessResponse()
        {
            // Arrange
            var commodities = "commodity1,commodity2";
            var regions = "region1,region2";
            var expectedOutages = new List<Outage>
            {
                new()
                    {
                        OutageStart = "25 Aug 2024 (Market information)",
                        OutageEnd = "12 Nov 2024 (Market information)",
                        Country = "Country 1",
                        Company = "Company 1",
                        Site = "Site 1",
                        PlantNo = 1,
                        Cause = "Scheduled",
                        CapacityLoss = "100% (est. 48.2kt)",
                        TotalAnnualCapacity = 220.00m,
                        LastUpdated = "25 Jan 2024",
                        Comments = "Comments 1"
                    },
                    new()
                    {
                        OutageStart = "26 Sep 2024 (Market information)",
                        OutageEnd = "13 Dec 2024 (Market information)",
                        Country = "Country 2",
                        Company = "Company 2",
                        Site = "Site 2",
                        PlantNo = 1,
                        Cause = "Scheduled",
                        CapacityLoss = "100% (est. 11.0kt)",
                        TotalAnnualCapacity = 130.00m,
                        LastUpdated = "26 Jan 2024",
                        Comments = "--"
                    }
            };

            var mockRepository = new Mock<IGenericRepository<Outage>>();
            mockRepository.Setup(mockRepository => mockRepository.ExecuteReadOnlySql(It.IsAny<string>(),
                It.IsAny<List<SqlParameter>>(),
                It.IsAny<int?>(), It.IsAny<int?>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedOutages);

            var mockLogger = new Mock<ILogger>();

            var mockSqlQueryReader = new Mock<ISqlQueryReader>();
            mockSqlQueryReader
                .Setup(reader => reader.ReadQuery("TableTool1.sql"))
                .Returns("SELECT * FROM Outages");

            var outageService = new OutageService(mockRepository.Object, mockLogger.Object, mockSqlQueryReader.Object);

            // Act
            var response = await outageService.GetOutagesByCommoditiesAndRegions(commodities, regions, null, null);

            // Assert
            Assert.True(response.IsSuccess);
            Assert.Equal(expectedOutages, response.Data);
        }

        [Fact]
        public async Task GetOutagesByCommoditiesAndRegions_InvalidParameters_ReturnsFailureResponse()
        {
            // Arrange
            var commodities = "";
            var regions = "region1,region2";
            int? pageNo = 1;
            int? pageSize = 10;

            var mockRepository = new Mock<IGenericRepository<Outage>>();
            var mockLogger = new Mock<ILogger>();
            var mockSqlQueryReader = new Mock<ISqlQueryReader>();

            var outageService = new OutageService(mockRepository.Object, mockLogger.Object, mockSqlQueryReader.Object);

            // Act
            var response = await outageService.GetOutagesByCommoditiesAndRegions(commodities, regions, pageNo, pageSize);

            // Assert
            Assert.True(response.IsFailure);
            Assert.Equal("Incorrect request parameter. Commodities and Region cannot be null or empty.", response.Error.Message);
        }

        [Fact]
        public async Task GetOutagesByCommoditiesAndRegions_InvalidPaging_ReturnsFailureResponse()
        {
            // Arrange
            var commodities = "Product1,Product2";
            var regions = "region1";
            int? pageNo = 0;
            int? pageSize = 0;

            var mockRepository = new Mock<IGenericRepository<Outage>>();
            var mockLogger = new Mock<ILogger>();
            var mockSqlQueryReader = new Mock<ISqlQueryReader>();

            var outageService = new OutageService(mockRepository.Object, mockLogger.Object, mockSqlQueryReader.Object);

            // Act
            var response = await outageService.GetOutagesByCommoditiesAndRegions(commodities, regions, pageNo, pageSize);

            // Assert
            Assert.True(response.IsFailure);
            Assert.Equal("Incorrect request parameter.PageNo and PageSize both should be either null or greater than zero.", response.Error.Message);
        }

        [Fact]
        public async Task GetOutagesByCommoditiesAndRegions_FileNotFound_ReturnsFailureResponse()
        {
            // Arrange
            var commodities = "commodity1,commodity2";
            var regions = "region1,region2";
            int? pageNo = 1;
            int? pageSize = 10;

            var mockRepository = new Mock<IGenericRepository<Outage>>();
            var mockLogger = new Mock<ILogger>();

            var mockSqlQueryReader = new Mock<ISqlQueryReader>();
            mockSqlQueryReader
                .Setup(reader => reader.ReadQuery("TableTool1.sql"))
                .Throws(new FileNotFoundException("Script file 'TableTool1.sql' not found."));

            var outageService = new OutageService(mockRepository.Object, mockLogger.Object, mockSqlQueryReader.Object);

            // Act
            var response = await outageService.GetOutagesByCommoditiesAndRegions(commodities, regions, pageNo, pageSize);

            // Assert
            Assert.False(response.IsSuccess);
            Assert.Equal("Some error occured while executing script.", response.Error.Message);
        }
    }
}
