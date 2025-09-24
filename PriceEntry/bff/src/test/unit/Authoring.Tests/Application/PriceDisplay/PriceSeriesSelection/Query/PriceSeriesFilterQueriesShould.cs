namespace Authoring.Tests.Application.PriceDisplay.PriceSeriesSelection.Query
{
    using System.Threading.Tasks;

    using Authoring.Tests.Functional.Tests.Common.Infrastructure;

    using Snapshooter;
    using Snapshooter.Xunit;

    using Test.Infrastructure.Authoring;
    using Test.Infrastructure.Authoring.GraphQLClients.PriceDisplay;
    using Test.Infrastructure.Extensions;

    using Xunit;

    [Collection(WebApplicationCollection.Name)]
    public class PriceSeriesFilterQueriesShould : WebApplicationTestBase
    {
        private readonly PriceSeriesSelectionForDisplayGraphQLClient priceSeriesSelectionClient;

        public PriceSeriesFilterQueriesShould(AuthoringBffApplicationFactory factory)
            : base(factory)
        {
            priceSeriesSelectionClient = new PriceSeriesSelectionForDisplayGraphQLClient(GraphQLClient);
        }

        [Fact]
        public async Task Return_All_Distinct_Commodities()
        {
            var result = await priceSeriesSelectionClient.GetCommodities()
                .GetRawResponse();
            result.MatchSnapshot(SnapshotNameExtension.Create("AllCommodities"));
        }
    }
}
