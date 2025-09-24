namespace Test.Infrastructure.Authoring.GraphQLClients.PriceEntry
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    using Test.Infrastructure.GraphQL;

    public class ImpactedPricesGraphQLClient
    {
        private readonly GraphQLClient client;

        public ImpactedPricesGraphQLClient(GraphQLClient client)
        {
            this.client = client;
        }

        public Task<HttpResponseMessage> GetImpactedPrices(string seriesId, DateTime assessedDateTime)
        {
            return client.SendAsync(
                GraphQLQueryBuilder
                   .New()
                   .SetQuery(Queries.GetImpactedPrices)
                   .AddVariable("priceSeriesId", seriesId)
                   .AddVariable("assessedDateTime", assessedDateTime)
                   .Build());
        }
    }
}
