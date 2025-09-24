namespace Test.Infrastructure.Authoring.ApiClients
{
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Text.Json;
    using System.Threading.Tasks;

    using Camunda.Worker.Variables;

    using global::Infrastructure.Services.Workflow;

    using Icis.Workflow.Constants;
    using Icis.Workflow.Extensions;
    using Icis.Workflow.Model;

    public class DataPackageWorkflowApiClient
    {
        private readonly HttpClient httpClient;

        public DataPackageWorkflowApiClient(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public Task<HttpResponseMessage> DataPackagePublished(WorkflowBusinessKey businessKey, string status)
        {
            var input = new APIEventPayload
            {
                BusinessKey = businessKey.Value,
                ProcessInstanceId = "some_process_instance_id",
                TaskId = "some_task_id",
                WorkspaceId = "pricing"
            };

            input.Variables.Add(VariableNames.ActionBy, new StringVariable("dummy_user_from_workflow"));
            input.Variables.Add(VariableNames.Status, new StringVariable(status));
            return httpClient.PostAsJsonAsync("/v1/workflow/data-package/published", input, new JsonSerializerOptions().GetJsonSerializerOptions());
        }

        public Task<HttpResponseMessage> DataPackageStatusUpdated(WorkflowBusinessKey businessKey, string status)
        {
            var input = new APIEventPayload
            {
                BusinessKey = businessKey.Value,
                ProcessInstanceId = "some_process_instance_id",
                TaskId = "some_task_id",
                WorkspaceId = "pricing"
            };

            input.Variables.Add(VariableNames.ActionBy, new StringVariable("dummy_user_from_workflow"));
            input.Variables.Add(VariableNames.Status, new StringVariable(status));

            return httpClient.PostAsJsonAsync("/v1/workflow/data-package/status-update", input, new JsonSerializerOptions().GetJsonSerializerOptions());
        }
    }
}
