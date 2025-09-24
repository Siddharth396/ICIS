namespace Authoring.Tests.Application.Outage.Query
{
    using Authoring.Application.Outage.Query;

    using BusinessLayer.Services.Models;
    using BusinessLayer.Services.Outage;

    using global::Infrastructure.GraphQL;
    using global::Infrastructure.SQLDB.Models;
    using global::Infrastructure.SQLDB.Repositories;

    using HotChocolate.Resolvers;

    using Microsoft.Extensions.Configuration;

    using Moq;

    using Test.Infrastructure.GraphQL;

    using Xunit;

    [Collection("Authoring stub collection")]
    public class OutageQueriesShould
    {
        private readonly QueryExecutor execute;
        private readonly Mock<IErrorReporter> errorReporter;
        private readonly Mock<IResolverContext> resolverContext;
        private readonly Mock<IConfiguration> mockConfiguration;
        private readonly OutageQueries outageQueries;
        private readonly Mock<IGenericRepository<Outage>> mockGenericeRepository;
        private readonly Mock<IOutageService> outageService;

        public OutageQueriesShould(AuthoringStubFixture fixture)
        {
            execute = fixture.Executor;
            errorReporter = new Mock<IErrorReporter>();
            resolverContext = new Mock<IResolverContext>();
            mockConfiguration = new Mock<IConfiguration>();
            outageQueries = new OutageQueries(errorReporter.Object);
            mockGenericeRepository = new Mock<IGenericRepository<Outage>>();
            outageService = new Mock<IOutageService>();
        }

        [Fact]
        public async Task GetOutagesByCommoditiesAndRegions_ReturnsData_WhenServiceSucceeds()
        {
            // Arrange
            var expectedData = new List<Outage>
            {
                new ()
                {
                    OutageStart = "2024-01-10T08:00:00Z",
                    OutageEnd = "2024-01-10T12:00:00Z",
                    Country = "USA",
                    Company = "EnergyCorp",
                    Site = "SiteA",
                    PlantNo = 1,
                    Cause = "Maintenance",
                    CapacityLoss = "100% (est. 26.3kt)",
                    TotalAnnualCapacity = 120.00m,
                    LastUpdated = "2024-01-10T12:30:00Z",
                    Comments = "Scheduled maintenance"
                },
                new ()
                {
                    OutageStart = "2024-02-15T14:00:00Z",
                    OutageEnd = "2024-02-15T18:00:00Z",
                    Country = "Canada",
                    Company = "PowerGen",
                    Site = "SiteB",
                    PlantNo = 2,
                    Cause = "Unexpected failure",
                    CapacityLoss = "10% (est. 0.1kt)",
                    TotalAnnualCapacity = 60.00m,
                    LastUpdated = "2024-02-15T18:30:00Z",
                    Comments = "Flooding closure"
                }
            };

            outageService.Setup(x => x.GetOutagesByCommoditiesAndRegions(string.Empty, string.Empty, It.IsAny<int?>(), It.IsAny<int?>()))
                .ReturnsAsync(Response<IEnumerable<Outage>>.Success(expectedData));

            // Act
            var result = await outageQueries.GetOutagesByCommoditiesAndRegions(string.Empty, string.Empty, null, null, resolverContext.Object, outageService.Object);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedData, result);
        }

        [Fact]
        public async Task GetOutagesByCommoditiesAndRegions_ReportsError_WhenServiceFails()
        {
            // Arrange
            var error = new Error("400", "Incorrect request parameter. Commodities and Region cannot be null or empty");
            var expectedResult = Response<IEnumerable<Outage>>.Failure(error.Message, error.Code);

            outageService.Setup(x => x.GetOutagesByCommoditiesAndRegions(string.Empty, string.Empty, It.IsAny<int?>(), It.IsAny<int?>()))
                .ReturnsAsync(expectedResult);

            // Act
            var actualResult = await outageQueries.GetOutagesByCommoditiesAndRegions(string.Empty, string.Empty, null, null, resolverContext.Object, outageService.Object);

            // Assert
            Assert.Null(actualResult);
            errorReporter.Verify(x => x.ReportCustomError(resolverContext.Object, error.Code, error.Message), Times.Once);
        }
    }
}
