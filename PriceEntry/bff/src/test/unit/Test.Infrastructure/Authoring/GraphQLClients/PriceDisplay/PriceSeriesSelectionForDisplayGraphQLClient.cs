namespace Test.Infrastructure.Authoring.GraphQLClients.PriceDisplay
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;

    using Test.Infrastructure.GraphQL;

    public class PriceSeriesSelectionForDisplayGraphQLClient
    {
        private readonly GraphQLClient graphQLClient;

        public PriceSeriesSelectionForDisplayGraphQLClient(GraphQLClient graphQLClient)
        {
            this.graphQLClient = graphQLClient;
        }

        public Task<HttpResponseMessage> GetPriceSeriesForDisplayWithFilters(
            List<Guid> commodities,
            bool includeInactivePriceSeries)
        {
            return graphQLClient.SendAsync(
                GraphQLQueryBuilder.New()
                .SetQuery(QueriesForDisplay.GetAuthoringPriceSeriesForDisplayWithFilters)
                .AddVariable("commodities", commodities)
                .AddVariable("includeInactivePriceSeries",  includeInactivePriceSeries)
                .Build());
        }

        public Task<HttpResponseMessage> GetCommodities()
        {
            return graphQLClient.SendAsync(
                GraphQLQueryBuilder.New()
                .SetQuery(QueriesForDisplay.GetCommodities)
                .Build());
        }
    }
}
