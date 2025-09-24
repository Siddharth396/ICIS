namespace Test.Infrastructure.Authoring.BusinessFlows.PriceEntryBusinessFlow.Processes
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    using Test.Infrastructure.Authoring.BusinessFlows.PriceEntryBusinessFlow;
    using Test.Infrastructure.Authoring.GraphQLClients.PriceEntry;
    using Test.Infrastructure.GraphQL;

    public class TransitionToStateProcess : IPriceEntryBusinessFlowProcess
    {
        private readonly DateTime assessedDateTime;

        private readonly string contentBlockId;

        private readonly int contentBlockVersion;

        private readonly string nextState;

        private readonly string operationType;

        public TransitionToStateProcess(
            string contentBlockId,
            int contentBlockVersion,
            DateTime assessedDateTime,
            string nextState,
            string operationType)
        {
            this.contentBlockId = contentBlockId ?? throw new ArgumentNullException(nameof(contentBlockId));
            this.contentBlockVersion = contentBlockVersion;
            this.assessedDateTime = assessedDateTime;
            this.nextState = nextState;
            this.operationType = operationType;
        }

        public Task<HttpResponseMessage> Execute(HttpClient httpClient)
        {
            var graphQLClient = new DataPackageGraphQLClient(new GraphQLClient(httpClient));

            return graphQLClient.DataPackageTransitionToState(
                contentBlockId,
                contentBlockVersion,
                assessedDateTime,
                nextState,
                operationType);
        }
    }
}
