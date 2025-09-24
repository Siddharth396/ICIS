namespace BusinessLayer.DataPackage.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using BusinessLayer.ContentBlock.DTOs.Output;
    using BusinessLayer.DataPackage.DTOs.Output;
    using BusinessLayer.DataPackage.Models;
    using BusinessLayer.PriceEntry.ValueObjects;

    using Infrastructure.Services.Workflow;
    using Infrastructure.Services.Workflow.Dtos;

    public interface IDataPackageService
    {
        Task<WorkflowBusinessKey?> GetWorkflowBusinessKey(DataPackageKey dataPackageKey);

        Task<DataPackageStatusChangeResult> InitiateCorrectionForDataPackage(
            DataPackageKey dataPackageKey,
            ReviewPageUrl reviewPageUrl);

        Task<DataPackageStatusChangeResult> OnDataPackagePublished(WorkflowBusinessKey businessKey, string userId, string status);

        Task<List<WorkflowAction>> GetNextActions(
            ContentBlockDefinition contentBlockDefinition,
            DateTime assessedDateTime,
            bool isReviewMode);

        Task<DataPackageStateTransitionResponse> TransitionToState(
            DataPackageKey dataPackageKey,
            string nextState,
            OperationType operationType,
            ReviewPageUrl reviewPageUrl);

        Task<DataPackageStateTransitionResponse> TriggerStateTransitionWithActionAfterWorkflowStarted(
            DataPackageKey dataPackageKey,
            string action,
            OperationType operationType);

        Task<DataPackageStatusChangeResult> OnDataPackageTransitionedToState(
            WorkflowBusinessKey businessKey,
            WorkflowStatus status,
            string userId);
    }
}