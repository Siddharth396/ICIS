namespace Authoring.Application.DataPackage.Mutation
{
    using System;
    using System.Threading.Tasks;

    using Authoring.Application.ContentBlock.Query;

    using BusinessLayer.DataPackage.DTOs.Output;
    using BusinessLayer.DataPackage.Models;
    using BusinessLayer.DataPackage.Services;
    using BusinessLayer.PriceEntry.ValueObjects;

    using global::Infrastructure.GraphQL;
    using global::Infrastructure.Services.Workflow;

    using HotChocolate;
    using HotChocolate.Types;

    using Serilog;
    using Serilog.Context;

    using OperationType = HotChocolate.Language.OperationType;
    using Version = BusinessLayer.ContentBlock.ValueObjects.Version;
    using WorkflowOperationType = global::Infrastructure.Services.Workflow.OperationType;

    [AddToGraphQLSchema]
    [ExtendObjectType(OperationType.Mutation)]
    public class DataPackageMutations
    {
        [GraphQLName("initiateCorrectionForDataPackage")]
        public async Task<bool> InitiateCorrectionForDataPackage(
            [Service] IDataPackageService dataPackageService,
            [Service] ILogger logger,
            [GraphQLNonNullType] string contentBlockId,
            [GraphQLNonNullType] int version,
            [GraphQLName(Constants.ContentBlock.AssessedDateTimeParameter)][GraphQLNonNullType] DateTime assessedDateTime,
            string reviewPageUrl)
        {
            using (LogContext.PushProperty("Flow", nameof(InitiateCorrectionForDataPackage)))
            {
                var localLogger = logger.ForContext<DataPackageMutations>();

                localLogger.Debug("START: Initiate Correction");

                var reviewPage = ReviewPageUrl.Create(reviewPageUrl);
                var response = await dataPackageService.InitiateCorrectionForDataPackage(
                                   new DataPackageKey(contentBlockId, Version.From(version), assessedDateTime),
                                   reviewPage);

                localLogger.Debug("END: Initiate Correction");

                return response == DataPackageStatusChangeResult.Success;
            }
        }

        [GraphQLName("dataPackageTransitionToState")]
        public async Task<DataPackageStateTransitionResponse> DataPackageTransitionToState(
            [Service] IDataPackageService dataPackageService,
            [Service] ILogger logger,
            [GraphQLNonNullType] string contentBlockId,
            [GraphQLNonNullType] int version,
            [GraphQLName(Constants.ContentBlock.AssessedDateTimeParameter)][GraphQLNonNullType] DateTime assessedDateTime,
            [GraphQLNonNullType] string nextState,
            string operationType = "",
            string reviewPageUrl = "")
        {
            using (LogContext.PushProperty("Flow", nameof(DataPackageTransitionToState)))
            {
                var localLogger = logger.ForContext<DataPackageMutations>();

                localLogger.Debug("START: State Transition");
                localLogger.Debug("Next State received: {nextState}", nextState);
                localLogger.Debug("Operation type received: {operationType}", operationType);
                DataPackageStateTransitionResponse response;

                var reviewPage = string.IsNullOrWhiteSpace(reviewPageUrl)
                                     ? ReviewPageUrl.Empty
                                     : ReviewPageUrl.Create(reviewPageUrl);

                var dataPackageKey = new DataPackageKey(contentBlockId, Version.From(version), assessedDateTime);

                if (WorkflowOperationType.Correction.Matches(operationType))
                {
                    response = await dataPackageService.TriggerStateTransitionWithActionAfterWorkflowStarted(
                                   dataPackageKey,
                                   nextState,
                                   WorkflowOperationType.Correction);
                }
                else
                {
                    response = await dataPackageService.TransitionToState(
                                   dataPackageKey,
                                   nextState,
                                   WorkflowOperationType.None,
                                   reviewPage);
                }

                localLogger.Debug("END: State Transition");

                return response;
            }
        }
    }
}
