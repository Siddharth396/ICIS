namespace Authoring.Tests.Application.PriceDisplay.PriceSeriesSelection.Query
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Authoring.Tests.Functional.Tests.Common.Infrastructure;

    using Microsoft.Extensions.DependencyInjection;

    using Snapshooter.Xunit;

    using Test.Infrastructure.Authoring;
    using Test.Infrastructure.Authoring.GraphQLClients.PriceDisplay;
    using Test.Infrastructure.Extensions;
    using Test.Infrastructure.Stubs;
    using Test.Infrastructure.TestData;

    using Xunit;

    [Collection(WebApplicationCollection.Name)]
    public class PriceSeriesSelectionQueriesForDisplayShould : WebApplicationTestBase
    {
        private readonly PriceSeriesSelectionForDisplayGraphQLClient priceSeriesSelectionClient;
        private readonly TestClock testClock;

        public PriceSeriesSelectionQueriesForDisplayShould(AuthoringBffApplicationFactory factory)
            : base(factory)
        {
            priceSeriesSelectionClient = new PriceSeriesSelectionForDisplayGraphQLClient(GraphQLClient);
            testClock = factory.Services.GetRequiredService<TestClock>();
            testClock.Reset();
        }

        [Fact]
        public async Task Return_The_Price_Series_With_Filters()
        {
            List<Guid> commodities =
            [
                TestData.CommodityLng,
                TestData.MelamineOldWhichDoesNotExistGuid
            ];

            var result = await priceSeriesSelectionClient.GetPriceSeriesForDisplayWithFilters(
                             commodities, false)
                            .GetRawResponse();

            result.MatchSnapshot();
        }

        [Fact]
        public async Task Return_InActive_Price_Series_With_Filters()
        {
            List<Guid> commodities = [TestData.CommodityMelamine];

            var result = await priceSeriesSelectionClient.GetPriceSeriesForDisplayWithFilters(
                             commodities, true)
                            .GetRawResponse();

            result.MatchSnapshot();
        }

        [Fact]
        public async Task Return_PreLaunch_Price_Series_With_Filters()
        {
            testClock.SetUtcNow(TestData.PreLaunchMelamineSeriesDate);

            List<Guid> commodities = [TestData.CommodityMelamine];

            var result = await priceSeriesSelectionClient.GetPriceSeriesForDisplayWithFilters(
                             commodities, true)
                            .GetRawResponse();

            result.MatchSnapshot();
        }
    }
}
