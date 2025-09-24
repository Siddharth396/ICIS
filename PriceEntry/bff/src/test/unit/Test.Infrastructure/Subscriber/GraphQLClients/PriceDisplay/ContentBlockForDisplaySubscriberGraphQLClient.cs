namespace Test.Infrastructure.Subscriber.GraphQLClients.PriceDisplay
{
    using System.Net.Http;
    using System.Threading.Tasks;

    using Test.Infrastructure.GraphQL;

    public class ContentBlockForDisplaySubscriberGraphQLClient
    {
        private readonly GraphQLClient client;

        public ContentBlockForDisplaySubscriberGraphQLClient(GraphQLClient client)
        {
            this.client = client;
        }

        public Task<HttpResponseMessage> GetContentBlockForDisplay(
            string contentBlockId,
            int? version = null)
        {
            var builder = GraphQLQueryBuilder.New()
               .SetQuery(QueriesForDisplay.GetSubscriberContentBlockForDisplay)
               .AddVariable("contentBlockId", contentBlockId);

            if (version.HasValue)
            {
                builder.AddVariable("version", version);
            }

            var query = builder.Build();
            return client.SendAsync(query);
        }

        public Task<HttpResponseMessage> GetContentBlockWithGridConfigurationOnly(string contentBlockId, int? version = null)
        {
            var builder = GraphQLQueryBuilder.New()
               .SetQuery(QueriesForDisplay.GetSubscriberContentBlockWithGridConfigurationOnly)
               .AddVariable("contentBlockId", contentBlockId);

            if (version.HasValue)
            {
                builder.AddVariable("version", version);
            }

            var query = builder.Build();
            return client.SendAsync(query);
        }

        public Task<HttpResponseMessage> GetContentBlockForDisplayWithPriceSeriesOnly(string contentBlockId, long assessedDateTime)
        {
            var builder = GraphQLQueryBuilder.New()
               .SetQuery(QueriesForDisplay.GetSubscriberContentBlockForDisplayWithPriceSeriesOnly)
               .AddVariable("contentBlockId", contentBlockId)
               .AddVariable("assessedDateTime", assessedDateTime);

            var query = builder.Build();
            return client.SendAsync(query);
        }
    }
}
