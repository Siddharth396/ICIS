namespace Test.Infrastructure.Authoring.GraphQLClients.PriceEntry
{
    using System.Net.Http;
    using System.Threading.Tasks;

    using BusinessLayer.PriceEntry.DTOs.Input;

    using Test.Infrastructure.GraphQL;

    public class PriceEntryGraphQLClient
    {
        private readonly GraphQLClient client;

        public PriceEntryGraphQLClient(GraphQLClient client)
        {
            this.client = client;
        }

        public Task<HttpResponseMessage> SavePriceSeriesItem(PriceItemInput priceItemInput)
        {
            return client.SendAsync(
                GraphQLQueryBuilder.New()
                   .SetQuery(Mutations.SavePriceSeriesItem)
                   .AddVariable("priceItemInput", priceItemInput)
                   .Build());
        }
    }
}
