namespace Test.Infrastructure.Authoring.BusinessFlows.PriceEntryBusinessFlow.Processes
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    using Test.Infrastructure.Authoring.GraphQLClients.PriceEntry;
    using Test.Infrastructure.GraphQL;

    public class ToggleNonMarketAdjustmentProcess : IPriceEntryBusinessFlowProcess
    {
        private readonly string contentBlockId;

        private readonly int contentBlockVersion;

        private readonly DateTime assessedDateTime;

        private readonly bool enabled;

        public ToggleNonMarketAdjustmentProcess(string contentBlockId, int contentBlockVersion, DateTime assessedDateTime, bool enabled)
        {
            this.contentBlockId = contentBlockId;
            this.contentBlockVersion = contentBlockVersion;
            this.assessedDateTime = assessedDateTime;
            this.enabled = enabled;
        }

        public Task<HttpResponseMessage> Execute(HttpClient httpClient)
        {
            var graphQLClient = new ContentBlockGraphQLClient(new GraphQLClient(httpClient));
            return graphQLClient.ToggleNonMarketAdjustment(
                contentBlockId,
                contentBlockVersion,
                assessedDateTime,
                enabled);
        }
    }
}
