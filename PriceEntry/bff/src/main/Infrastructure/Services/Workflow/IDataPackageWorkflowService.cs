namespace Infrastructure.Services.Workflow
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Infrastructure.Services.Workflow.Dtos;

    public interface IDataPackageWorkflowService
    {
        Task<WorkflowId> StartWorkflow(
            WorkflowBusinessKey businessKey,
            WorkflowVersion workflowVersion,
            IReadOnlyDictionary<string, string> variables,
            OperationType operationType,
            DateTime publishOnDate,
            ReviewPageUrl reviewPageUrl);

        Task<List<WorkflowAction>> GetNextActions(WorkflowBusinessKey businessKey);

        WorkflowVersion GetWorkflowVersion(string commodityName);

        Task<bool> PerformAction(WorkflowBusinessKey businessKey, string action);
    }
}
