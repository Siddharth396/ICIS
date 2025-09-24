namespace Test.Infrastructure.Authoring.BusinessFlows.PriceEntryBusinessFlow.Processes
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    using Test.Infrastructure.Authoring.BusinessFlows.PriceEntryBusinessFlow;
    using Test.Infrastructure.Authoring.GraphQLClients.PriceEntry;
    using Test.Infrastructure.GraphQL;

    public class InitiateCorrectionProcess : IPriceEntryBusinessFlowProcess
    {
        private readonly string contentBlockId;

        private readonly int contentBlockVersion;

        private readonly DateTime assessedDateTime;

        public InitiateCorrectionProcess(string contentBlockId, int contentBlockVersion, DateTime assessedDateTime)
        {
            this.contentBlockId = contentBlockId;
            this.contentBlockVersion = contentBlockVersion;
            this.assessedDateTime = assessedDateTime;
        }

        public Task<HttpResponseMessage> Execute(HttpClient httpClient)
        {
            var graphQLClient = new DataPackageGraphQLClient(new GraphQLClient(httpClient));
            return graphQLClient.InitiateCorrectionForDataPackage(
                contentBlockId,
                contentBlockVersion,
                assessedDateTime);
        }
    }
}
