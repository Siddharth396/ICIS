namespace Authoring.Tests.Application.Capacity.Query
{
    using Authoring.Application.Capacity.Query;

    using BusinessLayer.Services.Capacity;
    using BusinessLayer.Services.Models;

    using global::Infrastructure.GraphQL;
    using global::Infrastructure.SQLDB.Models;
    using global::Infrastructure.SQLDB.Repositories;

    using HotChocolate.Resolvers;

    using Microsoft.Extensions.Configuration;

    using Moq;

    using Test.Infrastructure.GraphQL;

    using Xunit;

    [Collection("Authoring stub collection")]
    public class CapacityQueriesShould
    {
        private readonly QueryExecutor execute;
        private readonly Mock<IErrorReporter> errorReporter;
        private readonly Mock<IResolverContext> resolverContext;
        private readonly Mock<IConfiguration> mockConfiguration;
        private readonly CapacityQueries capacityQueries;
        private readonly Mock<IGenericRepository<CapacityDevelopment>> mockGenericeRepository;
        private readonly Mock<ICapacityDevelopmentService> capacityDevelopmentService;

        public CapacityQueriesShould(AuthoringStubFixture fixture)
        {
            execute = fixture.Executor;
            errorReporter = new Mock<IErrorReporter>();
            resolverContext = new Mock<IResolverContext>();
            mockConfiguration = new Mock<IConfiguration>();
            capacityQueries = new CapacityQueries(errorReporter.Object);
            mockGenericeRepository = new Mock<IGenericRepository<CapacityDevelopment>>();
            capacityDevelopmentService = new Mock<ICapacityDevelopmentService>();
        }

        [Fact]
        public async Task GetCapacityDevelopmentsByCommoditiesAndRegions_ReturnsData_WhenServiceSucceeds()
        {
            // Arrange
            var expectedData = new List<CapacityDevelopment>
            {
                new ()
                {
                    Country = "USA",
                    Company = "TechCorp",
                    Site = "SiteA",
                    PlantNo = 1,
                    Type = "Expansion",
                    EstimatedStart = "Jan 2024",
                    NewAnnualCapacity = 5000.00m,
                    CapacityChange = 1000.00m,
                    PercentChange = "20%",
                    LastUpdated = "01 Jan 2024"
                },
                new ()
                {
                    Country = "Germany",
                    Company = "AutoMakers",
                    Site = "SiteB",
                    PlantNo = 1,
                    Type = "Contraction",
                    EstimatedStart = "Feb 2024",
                    NewAnnualCapacity = 4000.00m,
                    CapacityChange = -500.00m,
                    PercentChange = "-12.5%",
                    LastUpdated = "01 Feb 2024"
                }
            };

            capacityDevelopmentService.Setup(x => x.GetCapacityDevelopmentsByCommoditiesAndRegions(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<int?>()))
                .ReturnsAsync(Response<IEnumerable<CapacityDevelopment>>.Success(expectedData));

            // Act
            var actualData = await capacityQueries.GetCapacityDevelopmentsByCommoditiesAndRegions("Product1", "Region1,Region2", 1, 10, resolverContext.Object, capacityDevelopmentService.Object);

            // Assert
            Assert.NotNull(actualData);
            Assert.Equal(expectedData, actualData);
        }

        [Fact]
        public async Task GetCapacityDevelopmentsByCommoditiesAndRegions_ReportsError_WhenServiceFails()
        {
            // Arrange
            var error = new Error("400", "Incorrect request parameter. Commodities and Region cannot be null or empty");
            var expectedResult = Response<IEnumerable<CapacityDevelopment>>.Failure(error.Message, error.Code);

            capacityDevelopmentService.Setup(x => x.GetCapacityDevelopmentsByCommoditiesAndRegions(string.Empty, string.Empty, It.IsAny<int?>(), It.IsAny<int?>()))
                .ReturnsAsync(expectedResult);

            // Act
            var actualResult = await capacityQueries.GetCapacityDevelopmentsByCommoditiesAndRegions(string.Empty, string.Empty, null, null, resolverContext.Object, capacityDevelopmentService.Object);

            // Assert
            Assert.Null(actualResult);
            errorReporter.Verify(x => x.ReportCustomError(resolverContext.Object, error.Code, error.Message), Times.Once);
        }
    }
}
