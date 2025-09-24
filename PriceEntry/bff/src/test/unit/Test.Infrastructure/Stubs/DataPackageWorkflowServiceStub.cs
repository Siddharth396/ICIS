namespace Test.Infrastructure.Stubs
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using global::Infrastructure.Services.Workflow;
    using global::Infrastructure.Services.Workflow.Dtos;

    public class DataPackageWorkflowServiceStub : IDataPackageWorkflowService
    {
        private readonly WorkflowSettings workflowSettings;

        private readonly List<RequestRecord> requests = new();

        private bool failToCall;

        private bool failToStartWorkflow;

        public DataPackageWorkflowServiceStub(WorkflowSettings workflowSettings)
        {
            this.workflowSettings = workflowSettings;
        }

        public IReadOnlyList<RequestRecord> Requests => requests;

        public void FailToCallOnce()
        {
            failToCall = true;
        }

        public void ClearRequests()
        {
            requests.Clear();
        }

        public Task<List<WorkflowAction>> GetNextActions(WorkflowBusinessKey businessKey)
        {
            // We won't implement this method for now as this is what should be returned from Workflow
            return Task.FromResult(
                new List<WorkflowAction>() { new() { IsAllowed = true, Key = "DUMMY", Value = "Dummy" } });
        }

        public WorkflowVersion GetWorkflowVersion(string commodityName)
        {
            return new WorkflowVersion(workflowSettings.Versions.GetVersion(commodityName));
        }

        public Task<bool> PerformAction(WorkflowBusinessKey businessKey, string action)
        {
            requests.Add(new RequestRecord { Action = nameof(PerformAction), Params = new { BusinessKey = businessKey.Value, Action = action } });

            var result = !failToCall;

            failToCall = false;

            return Task.FromResult(result);
        }

        public void FailToStartWorkflowOnce()
        {
            failToStartWorkflow = true;
        }

        public Task<WorkflowId> StartWorkflow(
            WorkflowBusinessKey businessKey,
            WorkflowVersion workflowVersion,
            IReadOnlyDictionary<string, string> variables,
            OperationType operationType,
            DateTime publishOnDate,
            ReviewPageUrl reviewPageUrl)
        {
            requests.Add(
                new RequestRecord
                {
                    Action = nameof(StartWorkflow),
                    Params = new
                    {
                        BusinessKey = businessKey.Value,
                        WorkflowVersion = workflowVersion,
                        Variables = variables,
                        OperationType = operationType.Value,
                        PublishOnDate = publishOnDate
                    }
                });
            var shouldFailToStartWorkflow = failToStartWorkflow;

            failToStartWorkflow = false;

            return Task.FromResult(shouldFailToStartWorkflow ? WorkflowId.None : new WorkflowId(Guid.NewGuid().ToString()));
        }

        public class RequestRecord
        {
            public required string Action { get; set; }

            public required object Params { get; set; }
        }
    }
}
