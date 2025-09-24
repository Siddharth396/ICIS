namespace Authoring.Tests.Application.Region.Query
{
    using Authoring.Application.Region.Query;

    using BusinessLayer.Services.Models;
    using BusinessLayer.Services.Region;

    using FluentAssertions;

    using global::Infrastructure.GraphQL;
    using global::Infrastructure.SQLDB.Models;
    using global::Infrastructure.SQLDB.Repositories;

    using HotChocolate.Resolvers;

    using Moq;

    using Test.Infrastructure.GraphQL;

    using Xunit;

    [Collection("Authoring stub collection")]
    public class RegionQueriesShould
    {
        private readonly QueryExecutor execute;
        private readonly RegionQueries regionQueries;
        private readonly Mock<IErrorReporter> errorReporter;
        private readonly Mock<IResolverContext> resolverContext;
        private readonly Mock<IRegionService> regionService;
        private readonly Mock<IGenericRepository<RegionCC>> mockGenericeRepository;

        public RegionQueriesShould(AuthoringStubFixture fixture)
        {
            execute = fixture.Executor;
            errorReporter = new Mock<IErrorReporter>();
            resolverContext = new Mock<IResolverContext>();
            regionService = new Mock<IRegionService>();
            regionQueries = new RegionQueries(errorReporter.Object);
            mockGenericeRepository = new Mock<IGenericRepository<RegionCC>>();
        }

        [Fact]
        public async Task GetRegions_Successfully()
        {
            // Arrange
            RegionCC region1 = new RegionCC();
            region1.Id = 1;
            region1.Code = "OdAsia";
            region1.Description = "Order_Asia";
            RegionCC region2 = new RegionCC();
            region2.Id = 2;
            region2.Code = "OdEurope";
            region2.Description = "Order_Europe";
            IEnumerable<RegionCC> regions = new List<RegionCC>() { region1, region2 };

            mockGenericeRepository.Setup(x => x.GetQueryable()).Returns(regions.AsQueryable());
            regionService.Setup(x => x.GetRegion()).ReturnsAsync(Response<IEnumerable<RegionCC>>.Success(regions));

            // Act
            var response = await regionQueries.GetRegions(resolverContext.Object, regionService.Object);

            // Assert
            Assert.NotNull(response);
            Assert.Equal(regions, response);
            response.Should().BeAssignableTo<IEnumerable<RegionCC>>();
        }
    }
}
