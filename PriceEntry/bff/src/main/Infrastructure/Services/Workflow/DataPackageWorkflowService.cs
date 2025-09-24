namespace Infrastructure.Services.Workflow
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Net.Http.Json;
    using System.Threading.Tasks;

    using Infrastructure.Services.Workflow.Dtos;

    using Serilog;

    using Subscriber.Auth;

    public class DataPackageWorkflowService : IDataPackageWorkflowService
    {
        private readonly HttpClient httpClient;

        private readonly WorkflowSettings workflowSettings;

        private readonly IUserContext userContext;

        private readonly ILogger logger;

        public DataPackageWorkflowService(
            HttpClient httpClient,
            WorkflowSettings workflowSettings,
            IUserContext userContext,
            ILogger logger)
        {
            this.httpClient = httpClient;
            this.workflowSettings = workflowSettings;
            this.userContext = userContext;
            this.logger = logger.ForContext<DataPackageWorkflowService>();
        }

        public async Task<WorkflowId> StartWorkflow(
            WorkflowBusinessKey businessKey,
            WorkflowVersion workflowVersion,
            IReadOnlyDictionary<string, string> variables,
            OperationType operationType,
            DateTime publishOnDate,
            ReviewPageUrl reviewPageUrl)
        {
            if (reviewPageUrl == ReviewPageUrl.Empty)
            {
                throw new ArgumentException("Review page url is required to start a workflow.");
            }

            logger.Information($"Starting workflow for business key {businessKey.Value}.");
            logger.Information($"Workflow Version is {workflowVersion.Value}.");
            logger.Information($"Operation Type received {operationType}.");
            logger.Information($"PublishedOn Date is {publishOnDate}.");

            SetAuthorizationHeader();

            var httpResponse = await httpClient.PostAsJsonAsync(
                                   workflowSettings.GetStartWorkflowEndpointFullUrl(),
                                   new StartWorkflowRequest
                                   {
                                       ProcessDefinitionKey = workflowSettings.ProcessDefinitionKey,
                                       BusinessKey = businessKey.Value,
                                       Variables = GetStartWorkflowVariables(
                                           workflowVersion,
                                           variables,
                                           businessKey,
                                           operationType,
                                           publishOnDate,
                                           reviewPageUrl)
                                   });

            logger.Information($"Workflow response status code is {httpResponse.StatusCode}");

            if (!httpResponse.IsSuccessStatusCode)
            {
                return WorkflowId.None;
            }

            var response = await httpResponse.Content.ReadFromJsonAsync<StartWorkflowResponse>();

            logger.Information($"Workflow id from response is {response.Id}");

            return new WorkflowId(response.Id);
        }

        public WorkflowVersion GetWorkflowVersion(string commodityName)
        {
            return new WorkflowVersion(workflowSettings.Versions.GetVersion(commodityName));
        }

        public async Task<bool> PerformAction(WorkflowBusinessKey businessKey, string action)
        {
            logger.Information($"Transition data package for business key {businessKey.Value} with action {action}.");
            SetAuthorizationHeader();

            var httpResponse = await httpClient.PostAsJsonAsync(
                                   workflowSettings.GetStateTransitionEndpointFullUrl(),
                                   new TransitionToNextStateRequest
                                   {
                                       BusinessKey = businessKey.Value,
                                       Variables = new Dictionary<string, object> { ["nextAction"] = action }
                                   });

            logger.Information($"Transition data package for business key {businessKey.Value} with action {action} response status code is {httpResponse.StatusCode}");
            return httpResponse.IsSuccessStatusCode;
        }

        public async Task<List<WorkflowAction>> GetNextActions(WorkflowBusinessKey businessKey)
        {
            logger.Information($"Getting next actions for business key {businessKey.Value}.");
            SetAuthorizationHeader();

            var httpResponse = await httpClient.GetAsync(workflowSettings.GetNextActionsEndpointFullUrl(businessKey.Value));

            logger.Information($"Getting next actions for business key {businessKey.Value} response status code is {httpResponse.StatusCode}");

            if (!httpResponse.IsSuccessStatusCode || httpResponse.StatusCode == HttpStatusCode.NoContent)
            {
                return new List<WorkflowAction>();
            }

            var response = await httpResponse.Content.ReadFromJsonAsync<NextActionsResponse>();

            logger.Information($"Next actions count is {response.NextActions.Count}");

            return response.NextActions;
        }

        private Dictionary<string, object> GetStartWorkflowVariables(
            WorkflowVersion version,
            IReadOnlyDictionary<string, string> inputVariables,
            WorkflowBusinessKey businessKey,
            OperationType operationType,
            DateTime publishOnDate,
            ReviewPageUrl reviewPageUrl)
        {
            var variables = new Dictionary<string, object>
            {
                ["initialStatus"] = WorkflowStatus.Draft,
                ["workspaceId"] = workflowSettings.WorkspaceId,
                ["publishOnDate"] = publishOnDate,
                ["reviewUrlPath"] = reviewPageUrl.Value
            };

            // At some point, all the workflows will be advanced, so we would be able to remove this then
            // But for now we have to make sure that for LNG we have the simple workflow
            if (version == WorkflowVersion.Advance)
            {
                variables.Add("workflowType", operationType == OperationType.None ? "new" : operationType.Value);

                variables.Add("targetProcess", "pricing-advance");
            }

            foreach (var (key, value) in inputVariables)
            {
                variables.TryAdd(key, value);
            }

            return variables;
        }

        private void SetAuthorizationHeader()
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userContext.AccessToken);
        }
    }
}
