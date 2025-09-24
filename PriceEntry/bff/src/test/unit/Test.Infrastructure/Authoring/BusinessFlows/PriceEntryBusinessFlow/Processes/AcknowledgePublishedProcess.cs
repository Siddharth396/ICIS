namespace Test.Infrastructure.Authoring.BusinessFlows.PriceEntryBusinessFlow.Processes
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    using global::Infrastructure.Services.Workflow;

    using Test.Infrastructure.Authoring.ApiClients;
    using Test.Infrastructure.Authoring.BusinessFlows.PriceEntryBusinessFlow;
    using Test.Infrastructure.Authoring.GraphQLClients.PriceEntry;
    using Test.Infrastructure.GraphQL;

    public class AcknowledgePublishedProcess : IPriceEntryBusinessFlowProcess
    {
        private readonly string contentBlockId;

        private readonly DateTime assessedDateTime;

        private readonly string status;

        public AcknowledgePublishedProcess(string contentBlockId, DateTime assessedDateTime, string status = "")
        {
            this.contentBlockId = contentBlockId ?? throw new ArgumentNullException(nameof(contentBlockId));
            this.assessedDateTime = assessedDateTime;
            this.status = status;
        }

        public async Task<HttpResponseMessage> Execute(HttpClient httpClient)
        {
            var graphQlClient = new ContentBlockGraphQLClient(new GraphQLClient(httpClient));

            var workflowBusinessKey = await graphQlClient.GetWorkflowBusinessKey(
                contentBlockId,
                assessedDateTime);

            var apiClient = new DataPackageWorkflowApiClient(httpClient);

            return await apiClient.DataPackagePublished(new WorkflowBusinessKey(workflowBusinessKey), status);
        }
    }
}
