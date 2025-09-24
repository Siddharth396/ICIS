namespace Test.Infrastructure.Authoring.GraphQLClients.PriceEntry
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    using Test.Infrastructure.GraphQL;

    public class DataPackageGraphQLClient
    {
        private const string DummyReviewPageUrl = "/dummy/url";

        private readonly GraphQLClient client;

        public DataPackageGraphQLClient(GraphQLClient client)
        {
            this.client = client;
        }

        public Task<HttpResponseMessage> InitiateCorrectionForDataPackage(string contentBlockId, int version, DateTime assessedDateTime)
        {
            return client.SendAsync(
                GraphQLQueryBuilder.New()
                   .SetQuery(Mutations.InitiateCorrectionForDataPackage)
                   .AddVariable("contentBlockId", contentBlockId)
                   .AddVariable("version", version)
                   .AddVariable("assessedDateTime", assessedDateTime)
                   .AddVariable("reviewPageUrl", DummyReviewPageUrl)
                   .Build());
        }

        public Task<HttpResponseMessage> DataPackageTransitionToState(string contentBlockId, int version, DateTime assessedDateTime, string nextState, string operationType)
        {
            return client.SendAsync(
                GraphQLQueryBuilder.New()
                   .SetQuery(Mutations.DataPackageTransitionToState)
                   .AddVariable("contentBlockId", contentBlockId)
                   .AddVariable("version", version)
                   .AddVariable("assessedDateTime", assessedDateTime)
                   .AddVariable("nextState", nextState)
                   .AddVariable("operationType", operationType)
                   .AddVariable("reviewPageUrl", DummyReviewPageUrl)
                   .Build());
        }
    }
}