namespace Test.Infrastructure.Authoring.BusinessFlows.PriceEntryBusinessFlow.Processes
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    using global::Infrastructure.Services.Workflow;

    using Test.Infrastructure.Authoring.BusinessFlows.PriceEntryBusinessFlow;
    using Test.Infrastructure.Authoring.GraphQLClients.PriceEntry;
    using Test.Infrastructure.GraphQL;

    /// <summary>
    /// To be used only for "simple" workflow.
    /// </summary>
    public class InitiatePublishProcess : IPriceEntryBusinessFlowProcess
    {
        private readonly string contentBlockId;

        private readonly int contentBlockVersion;

        private readonly DateTime assessedDateTime;

        public InitiatePublishProcess(string contentBlockId, int contentBlockVersion, DateTime assessedDateTime)
        {
            this.contentBlockId = contentBlockId ?? throw new ArgumentNullException(nameof(contentBlockId));
            this.contentBlockVersion = contentBlockVersion;
            this.assessedDateTime = assessedDateTime;
        }

        public Task<HttpResponseMessage> Execute(HttpClient httpClient)
        {
            var graphQLClient = new DataPackageGraphQLClient(new GraphQLClient(httpClient));

            return graphQLClient.DataPackageTransitionToState(
                contentBlockId,
                contentBlockVersion,
                assessedDateTime,
                UserActions.Publish,
                string.Empty);
        }
    }
}
