namespace Authoring.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;

    using BusinessLayer.DataPackage.Models;
    using BusinessLayer.DataPackage.Services;

    using Camunda.Worker.Variables;
    using global::Infrastructure.Services.Workflow;

    using Icis.Workflow.Constants;
    using Icis.Workflow.Extensions;
    using Icis.Workflow.Model;

    using Microsoft.AspNetCore.Mvc;

    using Serilog;
    using Serilog.Context;

    [Route("v1/[controller]")]
    [ApiController]
    public class WorkflowController : ControllerBase
    {
        private readonly IDataPackageService dataPackageService;

        private readonly ILogger logger;

        public WorkflowController(
            IDataPackageService dataPackageService,
            ILogger logger)
        {
            this.dataPackageService = dataPackageService;
            this.logger = logger.ForContext<WorkflowController>();
        }

        [HttpPost]
        [Route("content-package/published")]
        [Obsolete("Configure workflow to use the 'data-package/published' endpoint, and remove this after.")]
        [ExcludeFromCodeCoverage]
        public async Task<APIEventResponse> ContentPackagePublished([FromBody] APIEventPayload input)
        {
            return await DataPackagePublished(input);
        }

        [HttpPost]
        [Route("data-package/published")]
        public async Task<APIEventResponse> DataPackagePublished([FromBody] APIEventPayload input)
        {
            using (LogContext.PushProperty("Flow", nameof(DataPackagePublished)))
            using (LogContext.PushProperty("BusinessKey", input.BusinessKey))
            {
                var status = GetStatusFromPayload(input);
                if (string.IsNullOrEmpty(status))
                {
                    logger.Warning("Status variable not found in the payload.");
                    return CreateFailResponse(WorkflowConstants.ResponseErrorCodes.StatusNotFound, "Status variable not found in the payload.", WorkflowConstants.ResponseErrorCodes.StatusUpdateError);
                }

                var localLogger = logger.ForContext("BusinessKey", input.BusinessKey)
                    .ForContext("Payload", input, true);

                localLogger.Information("Notification received about data package being published with status: {status}", status);

                var userId = input.Variables.GetActionPerformedByUser();

                var result = await dataPackageService.OnDataPackagePublished(new WorkflowBusinessKey(input.BusinessKey), userId, status);

                if (result == DataPackageStatusChangeResult.NotFound)
                {
                    localLogger.Error($"No data package found for workflow business key {input.BusinessKey}.");

                    return CreateFailResponse("DATA_PACKAGE_NOT_FOUND", "No data package found for the given business key.", WorkflowConstants.ResponseErrorCodes.PublishError);
                }

                if (result == DataPackageStatusChangeResult.InvalidStatus)
                {
                    localLogger.Error("Data package is not in the correct status to be published.");

                    return CreateFailResponse(
                        "DATA_PACKAGE_INVALID_STATUS",
                        "Data package is not in the correct status to be published.",
                        WorkflowConstants.ResponseErrorCodes.PublishError);
                }

                return new APIEventResponse
                {
                    ResultType = ResultType.COMPLETE
                };
            }
        }

        [HttpPost]
        [Route("data-package/status-update")]
        public async Task<APIEventResponse> DataPackageStatusUpdated([FromBody] APIEventPayload input)
        {
            using (LogContext.PushProperty("Flow", nameof(DataPackageStatusUpdated)))
            using (LogContext.PushProperty("BusinessKey", input.BusinessKey))
            {
                var status = GetStatusFromPayload(input);
                if (string.IsNullOrEmpty(status))
                {
                    logger.Warning("Status variable not found in the payload.");
                    return CreateFailResponse(WorkflowConstants.ResponseErrorCodes.StatusNotFound, "Status variable not found in the payload.", WorkflowConstants.ResponseErrorCodes.StatusUpdateError);
                }

                using (LogContext.PushProperty("ReceivedStatus", status))
                {
                    logger.Information("Notification received about data package status update with status: {status}", status);

                    var userId = input.Variables.GetActionPerformedByUser();

                    var result = await dataPackageService.OnDataPackageTransitionedToState(
                                     new WorkflowBusinessKey(input.BusinessKey),
                                     new WorkflowStatus(status),
                                     userId);

                    if (result == DataPackageStatusChangeResult.Success)
                    {
                        logger.Information("Data package status update with success");
                        return new APIEventResponse { ResultType = ResultType.COMPLETE };
                    }

                    logger.Information($"Data package status update failed, with error: {result.ToString()}");
                    return CreateFailResponse(result.ToString(), "Error in status update", WorkflowConstants.ResponseErrorCodes.StatusUpdateError);
                }
            }
        }

        private static APIEventResponse CreateFailResponse(string errorCode, string errorMessage, string responseErrorCode)
        {
            return new APIEventResponse
            {
                ResultType = ResultType.BPMNERROR,
                ErrorMessage = errorMessage,
                ErrorCode = responseErrorCode,
                Variables = new Dictionary<string, VariableBase>
                {
                    [WorkflowConstants.Variables.FailureReasonCode] = new StringVariable(errorCode),
                    [WorkflowConstants.Variables.FailureReasonMessage] = new StringVariable(errorMessage)
                }
            };
        }

        private static string GetStatusFromPayload(APIEventPayload input)
        {
            input.Variables.TryGetValue(VariableNames.Status, out var value);
            return value != null ? ((StringVariable)value!).Value : string.Empty;
        }

        private static class WorkflowConstants
        {
            public static class ResponseErrorCodes
            {
                public const string StatusNotFound = "STATUS_NOT_FOUND";

                public const string StatusUpdateError = "STATUS_UPDATE_ERROR";

                public const string PublishError = "PUBLISH_ERROR";
            }

            public static class Variables
            {
                public const string FailureReasonCode = "FailureReasonCode";

                public const string FailureReasonMessage = "FailureReasonMessage";
            }
        }
    }
}
