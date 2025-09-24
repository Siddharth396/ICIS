namespace Test.Infrastructure.Authoring.GraphQLClients.PriceEntry
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;

    using Test.Infrastructure.GraphQL;

    public class PriceSeriesSelectionGraphQLClient
    {
        private readonly GraphQLClient client;

        public PriceSeriesSelectionGraphQLClient(GraphQLClient client)
        {
            this.client = client;
        }

        public Task<HttpResponseMessage> GetPriceSeriesWithFilters(
            Guid commodityId,
            Guid priceCategoryId,
            Guid regionId,
            Guid priceSettlementTypeId,
            Guid itemFrequencyId)
        {
            return client.SendAsync(
                GraphQLQueryBuilder.New()
                   .SetQuery(Queries.GetPriceSeriesWithFilters)
                   .AddVariable("commodityId", commodityId)
                   .AddVariable("priceCategoryId", priceCategoryId)
                   .AddVariable("regionId", regionId)
                   .AddVariable("priceSettlementTypeId", priceSettlementTypeId)
                   .AddVariable("itemFrequencyId", itemFrequencyId)
                   .Build());
        }

        public Task<HttpResponseMessage> GetFilters(IEnumerable<string>? selectedPriceSeriesIds)
        {
            var setQuery = GraphQLQueryBuilder.New().SetQuery(Queries.GetFilters);

            if (selectedPriceSeriesIds != null)
            {
                setQuery.AddVariable("selectedPriceSeriesIds", selectedPriceSeriesIds);
            }

            return client.SendAsync(
                setQuery.Build());
        }
    }
}
