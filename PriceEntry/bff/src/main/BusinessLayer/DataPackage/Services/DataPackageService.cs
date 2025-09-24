namespace BusinessLayer.DataPackage.Services
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading.Tasks;

    using BusinessLayer.Commentary.Services;
    using BusinessLayer.ContentBlock.DTOs.Output;
    using BusinessLayer.ContentBlock.Services;
    using BusinessLayer.DataPackage.DTOs.Output;
    using BusinessLayer.DataPackage.Handler;
    using BusinessLayer.DataPackage.Models;
    using BusinessLayer.DataPackage.Repositories.Models;
    using BusinessLayer.PriceEntry.Services;
    using BusinessLayer.PriceEntry.Services.Factories;
    using BusinessLayer.PriceEntry.ValueObjects;
    using BusinessLayer.PriceSeriesSelection.Repositories.Models;
    using BusinessLayer.PriceSeriesSelection.Services;

    using Infrastructure.Attributes.BusinessAnnotations;
    using Infrastructure.EventDispatcher;
    using Infrastructure.MongoDB.Models;
    using Infrastructure.Services.AuditInfoService;
    using Infrastructure.Services.Workflow;
    using Infrastructure.Services.Workflow.Dtos;

    using Microsoft.Extensions.Internal;

    using Serilog;

    using Version = BusinessLayer.ContentBlock.ValueObjects.Version;

    [ApplicationService]
    public class DataPackageService : IDataPackageService
    {
        private readonly IAuditInfoService auditInfoService;

        private readonly IContentBlockService contentBlockService;

        private readonly IEventDispatcher eventDispatcher;

        private readonly WorkflowSettings workflowSettings;

        private readonly IHalfMonthPriceSeriesItemService halfMonthPriceSeriesItemService;

        private readonly ILogger logger;

        private readonly IPriceSeriesService priceSeriesService;

        private readonly ISeriesItemTypeServiceFactory seriesItemTypeServiceFactory;

        private readonly IDataPackageWorkflowService workflowService;

        private readonly IPublicationScheduleService publicationScheduleService;

        private readonly ISystemClock clock;

        private readonly IDataPackageMetadataDomainService dataPackageMetadataDomainService;

        private readonly IDataPackageDomainService dataPackageDomainService;

        private readonly IPriceSeriesItemsDomainService priceSeriesItemsDomainService;

        private readonly IContentBlockDomainService contentBlockDomainService;

        private readonly ICommentaryDomainService commentaryDomainService;

        public DataPackageService(
            ISeriesItemTypeServiceFactory seriesItemTypeServiceFactory,
            IContentBlockService contentBlockService,
            IDataPackageWorkflowService workflowService,
            IAuditInfoService auditInfoService,
            IPriceSeriesService priceSeriesService,
            IHalfMonthPriceSeriesItemService halfMonthPriceSeriesItemService,
            IEventDispatcher eventDispatcher,
            ILogger logger,
            WorkflowSettings workflowSettings,
            IPublicationScheduleService publicationScheduleService,
            ISystemClock clock,
            IDataPackageMetadataDomainService dataPackageMetadataDomainService,
            IDataPackageDomainService dataPackageDomainService,
            IPriceSeriesItemsDomainService priceSeriesItemsDomainService,
            IContentBlockDomainService contentBlockDomainService,
            ICommentaryDomainService commentaryDomainService)
        {
            this.seriesItemTypeServiceFactory = seriesItemTypeServiceFactory;
            this.contentBlockService = contentBlockService;
            this.workflowService = workflowService;
            this.auditInfoService = auditInfoService;
            this.priceSeriesService = priceSeriesService;
            this.halfMonthPriceSeriesItemService = halfMonthPriceSeriesItemService;
            this.eventDispatcher = eventDispatcher;
            this.workflowSettings = workflowSettings;
            this.logger = logger.ForContext<DataPackageService>();
            this.publicationScheduleService = publicationScheduleService;
            this.clock = clock;
            this.dataPackageMetadataDomainService = dataPackageMetadataDomainService;
            this.dataPackageDomainService = dataPackageDomainService;
            this.priceSeriesItemsDomainService = priceSeriesItemsDomainService;
            this.contentBlockDomainService = contentBlockDomainService;
            this.commentaryDomainService = commentaryDomainService;
        }

        public async Task<List<WorkflowAction>> GetNextActions(
            ContentBlockDefinition contentBlockDefinition,
            DateTime assessedDateTime,
            bool isReviewMode)
        {
            if (contentBlockDefinition.GetPriceSeriesIds().Count == 0)
            {
                return [];
            }

            var actions = await GetNextActionsInternal();

            foreach (var action in actions)
            {
                action.IsAllowed = isReviewMode
                                       ? UserActions.IsCopyEditorAction(action.Key)
                                       : UserActions.IsEditorAction(action.Key);
            }

            return actions;

            async Task<List<WorkflowAction>> GetNextActionsInternal()
            {
                var dataPackageKey = new DataPackageKey(
                    contentBlockDefinition.ContentBlockId,
                    Version.From(contentBlockDefinition.Version),
                    assessedDateTime);
                var dataPackage = await dataPackageDomainService.GetDataPackage(dataPackageKey);

                var localLogger = logger.ForContext("Get Next Actions For data package", dataPackage?.Id);

                var priceSeriesIds = contentBlockDefinition.GetPriceSeriesIds();

                if (dataPackage == null || WorkflowStatus.Draft.Matches(dataPackage.Status))
                {
                    // first action depends on the workflow version of the commodity
                    // this is because we don't start a workflow before the first action
                    var priceSeries = await priceSeriesService.GetPriceSeriesById(priceSeriesIds.First());
                    var workflowVersion = workflowService.GetWorkflowVersion(priceSeries.Commodity.Name.English!);

                    var nmaAction = await dataPackageMetadataDomainService.IsNonMarketAdjustmentEnabled(dataPackageKey)
                                        ? new WorkflowAction
                                        {
                                            IsAllowed = true,
                                            Key = UserActions.CancelNma,
                                            Value = "Cancel non-market adjustment"
                                        }
                                        : new WorkflowAction
                                        {
                                            IsAllowed = true,
                                            Key = UserActions.StartNma,
                                            Value = "Non-market adjustment"
                                        };

                    if (workflowVersion == WorkflowVersion.Simple)
                    {
                        return
                        [
                            nmaAction, new WorkflowAction { IsAllowed = true, Key = UserActions.Publish, Value = "Publish" }
                        ];
                    }

                    return
                    [
                        nmaAction,
                        new WorkflowAction { IsAllowed = true, Key = UserActions.SendForReview, Value = "Send for review" }
                    ];
                }

                if (WorkflowStatus.IsPublishedOrCorrectionPublishedStatusMatch(dataPackage.Status))
                {
                    var priceSeries = await priceSeriesService.GetPriceSeriesById(priceSeriesIds.First());
                    var workflowVersion = workflowService.GetWorkflowVersion(priceSeries.Commodity.Name.English!);

                    localLogger.Debug("Workflow version is {workflowVersion} and Workflow Correction Toggle value is {workflowCorrectionToggle}", workflowVersion, workflowSettings.WorkflowCorrectionToggle);

                    if (workflowVersion == WorkflowVersion.Advance)
                    {
                        return workflowSettings.WorkflowCorrectionToggle
                            ? [new WorkflowAction { IsAllowed = true, Key = UserActions.InitiateCorrection, Value = "Correction needed" }]
                            : GetRepublishAction();
                    }

                    return GetRepublishAction();
                }

                return await workflowService.GetNextActions(
                           new WorkflowBusinessKey(dataPackage.WorkflowData!.BusinessKey));
            }
        }

        public async Task<WorkflowBusinessKey?> GetWorkflowBusinessKey(DataPackageKey dataPackageKey)
        {
            var dataPackage = await dataPackageDomainService.GetDataPackage(dataPackageKey);
            return dataPackage?.WorkflowData != null
                       ? new WorkflowBusinessKey(dataPackage.WorkflowData.BusinessKey)
                       : null;
        }

        public async Task<DataPackageStatusChangeResult> InitiateCorrectionForDataPackage(
            DataPackageKey dataPackageKey,
            ReviewPageUrl reviewPageUrl)
        {
            var contentBlock = await contentBlockDomainService.GetContentBlock(
                                   dataPackageKey.ContentBlockId,
                                   dataPackageKey.ContentBlockVersion);

            if (contentBlock == null)
            {
                logger.ForContext("Scenario", "NotFound").Debug("Content block not found");

                return DataPackageStatusChangeResult.NotFound;
            }

            var dataPackage = await dataPackageDomainService.GetDataPackage(dataPackageKey);
            if (dataPackage == null)
            {
                logger.ForContext("Scenario", "NotFound").Warning("Data package not found");

                return DataPackageStatusChangeResult.NotFound;
            }

            var priceSeries = await priceSeriesService.GetPriceSeriesById(contentBlock.GetPriceSeriesIds().First());
            var workflowVersion = workflowService.GetWorkflowVersion(priceSeries.Commodity.Name.English!);

            logger.ForContext("Scenario", "Workflow Version Found").Debug("Workflow Version value When InitiateCorrectionForDataPackage is {workflowVersion}", workflowVersion.Value);
            logger.Debug("Workflow Correction Toggle set to {workflowCorrectionToggle}", workflowSettings.WorkflowCorrectionToggle);

            if (workflowVersion == WorkflowVersion.Advance && workflowSettings.WorkflowCorrectionToggle)
            {
                return await InitiateAdvancedCorrectionWorkflow(contentBlock, dataPackage, reviewPageUrl);
            }
            else
            {
                if (!WorkflowStatus.IsPublishedOrCorrectionPublishedStatusMatch(dataPackage.Status))
                {
                    logger
                       .ForContext("Scenario", "InvalidStatus")
                       .ForContext("CurrentStatus", dataPackage.Status)
                       .ForContext("DesiredStatus", WorkflowStatus.Published.Value)
                       .Warning($"Can not initiate correction as the status is {dataPackage.Status}");

                    return DataPackageStatusChangeResult.InvalidStatus;
                }

                var auditInfo = auditInfoService.GetAuditInfoForCurrentUser();
                await UpdateDataPackageOnStatusChange(
                    dataPackage,
                    WorkflowStatus.Draft,
                    auditInfo);

                return DataPackageStatusChangeResult.Success;
            }
        }

        public Task<DataPackageStatusChangeResult> OnDataPackagePublished(
            WorkflowBusinessKey businessKey,
            string userId,
            string status)
        {
            return OnDataPackageTransitionedToState(businessKey, new WorkflowStatus(status), userId);
        }

        public async Task<DataPackageStatusChangeResult> OnDataPackageTransitionedToState(
            WorkflowBusinessKey businessKey,
            WorkflowStatus status,
            string userId)
        {
            var localLogger = logger.ForContext("Transition To State acknowledged for", businessKey.ToString());
            localLogger.Information($"Status received in acknowledgement: {status}");

            var dataPackage = await dataPackageDomainService.GetDataPackage(businessKey);
            if (dataPackage == null)
            {
                return DataPackageStatusChangeResult.NotFound;
            }

            var auditInfo = auditInfoService.GetAuditInfoForUser(userId);

            return await UpdateDataPackageOnStatusChange(
                       dataPackage,
                       status,
                       auditInfo);
        }

        public async Task<DataPackageStateTransitionResponse> TransitionToState(
            DataPackageKey dataPackageKey,
            string nextState,
            OperationType operationType,
            ReviewPageUrl reviewPageUrl)
        {
            switch (nextState)
            {
                case UserActions.Publish:
                    // Publish is for simple workflow only
                    return await StartWorkflowAndPerformFirstTransition(
                               dataPackageKey,
                               UserActions.Publish,
                               operationType,
                               reviewPageUrl);
                case UserActions.SendForReview:
                    // SendForReview is the first state for advance workflow
                    return await StartWorkflowAndPerformFirstTransition(
                               dataPackageKey,
                               UserActions.SendForReview,
                               operationType,
                               reviewPageUrl);
            }

            return await TriggerStateTransitionWithActionAfterWorkflowStarted(dataPackageKey, nextState, operationType);
        }

        [ExcludeFromCodeCoverage]
        public async Task<DataPackageStateTransitionResponse> TriggerStateTransitionWithActionAfterWorkflowStarted(
            DataPackageKey dataPackageKey,
            string action,
            OperationType operationType)
        {
            var dataPackage = await dataPackageDomainService.GetDataPackage(dataPackageKey);

            if (dataPackage is null)
            {
                return DataPackageStateTransitionResponse.Error(ErrorCodes.DataPackageNotFound);
            }

            if (action != UserActions.Cancel)
            {
                var validationResult = await ValidateBeforeStateTransition(dataPackageKey, operationType);

                if (!validationResult.IsSuccess)
                {
                    return validationResult;
                }
            }

            var isSuccess = await workflowService.PerformAction(
                                new WorkflowBusinessKey(dataPackage.WorkflowData!.BusinessKey),
                                action);
            if (!isSuccess)
            {
                return DataPackageStateTransitionResponse.Error(ErrorCodes.WorkflowTransitionFailed);
            }

            return DataPackageStateTransitionResponse.Success;
        }

        private async Task<DataPackageStatusChangeResult> InitiateAdvancedCorrectionWorkflow(
            BusinessLayer.ContentBlock.Repositories.Models.ContentBlock contentBlock,
            DataPackage dataPackage,
            ReviewPageUrl reviewPageUrl)
        {
            var localLogger = logger.ForContext("Scenario: Advanced Correction Workflow", dataPackage.Id);
            localLogger.Debug("Initiating advance correction workflow for data package", dataPackage.Id);

            var (workflowBusinessKey, workflowId) = await StartWorkflow(
                                                        contentBlock,
                                                        dataPackage,
                                                        OperationType.Correction,
                                                        reviewPageUrl);
            if (workflowId == WorkflowId.None)
            {
                localLogger
                   .ForContext("Scenario", "Correction Workflow failed")
                   .Warning("Advance correction workflow is failed to initialize for {dataPackageId}", dataPackage.Id);

                return DataPackageStatusChangeResult.CorrectionFailed;
            }

            await dataPackageDomainService.CorrectionStarted(
                dataPackage.GetDataPackageId(),
                workflowId,
                workflowBusinessKey);

            return DataPackageStatusChangeResult.Success;
        }

        private async Task<DataPackageStatusChangeResult> UpdateDataPackageOnStatusChange(
            DataPackage dataPackage,
            WorkflowStatus status,
            AuditInfo auditInfo)
        {
            var localLogger = logger.ForContext("Updating data package on status change", dataPackage.Id);

            if (status == WorkflowStatus.Cancelled)
            {
                localLogger.Debug("Correction cancelled, removing pending changes to revert back to original");
                await dataPackageDomainService.CorrectionCancelled(dataPackage.GetDataPackageId());
            }
            else if (status == WorkflowStatus.CorrectionPublished)
            {
                localLogger.Debug("Correction published, overwrite data package with pending changes");
                await dataPackageDomainService.CorrectionPublished(dataPackage.GetDataPackageId(), auditInfo);
            }
            else if (WorkflowStatus.IsCorrectionPrePublishStatus(status))
            {
                localLogger.Debug("Correction status change, transition data package to status {status}", status);
                await dataPackageDomainService.StatusChangedDuringCorrection(dataPackage, status, auditInfo);
            }
            else
            {
                localLogger.Debug("Standard status change, transition data package to status {status}", status);

                await dataPackageDomainService.StatusChanged(dataPackage, status, auditInfo);
            }

            // Fetching data package again to ensure that the latest data package is sent to the event dispatcher
            var dataPackageUpdated = await dataPackageDomainService.GetDataPackage(dataPackage.GetDataPackageId());
            await eventDispatcher.DispatchAsync(
                new DataPackageUpdatedEvent { UserId = auditInfo.User, DataPackage = dataPackageUpdated!, Status = status });

            return DataPackageStatusChangeResult.Success;
        }

        private async Task<bool> AreAllReferencePriceSeriesPublishedOrInSameContentBlock(
            PriceSeriesGrid priceSeriesGrid,
            DateTime assessedDateTime,
            List<string> otherGridsPriceSeriesIds)
        {
            var priceItemService = seriesItemTypeServiceFactory.GetPriceItemService(
                SeriesItemTypeCodeFactory.GetSeriesItemTypeCode(priceSeriesGrid.SeriesItemTypeCode!));

            return await priceItemService.AreAllReferencePriceSeriesPublishedOrInSameContentBlock(
                       priceSeriesGrid.PriceSeriesIds!,
                       assessedDateTime,
                       otherGridsPriceSeriesIds);
        }

        private async Task<bool> AreInputsPublishedOrInSameContentBlock(
            DateTime assessedDateTime,
            List<string> priceSeriesIds,
            List<string> otherGridsPriceSeriesIds)
        {
            var priceSeriesList = await priceSeriesService.GetPriceSeriesByIds(priceSeriesIds);

            var priceSeriesWithDerivationInput = priceSeriesList
                                                .Where(ps => ps.DerivationInputs != null && ps.DerivationInputs.Any())
                                                .ToList();

            foreach (var priceSeries in priceSeriesWithDerivationInput)
            {
                var priceSeriesItems = await priceSeriesItemsDomainService.GetPriceSeriesItems(
                                           priceSeries.DerivationInputs!.Select(x => x.SeriesId).ToList(),
                                           assessedDateTime);

                if (priceSeries.DerivationInputs!.First().DerivationFunctionKey == DerivationFunctionKey.PeriodAvg)
                {
                    var (halfMonthPeriodOne, halfMonthPeriodTwo) =
                        await halfMonthPriceSeriesItemService.GetHalfMonthPriceSeriesItems(
                            priceSeriesItems,
                            priceSeries,
                            assessedDateTime);

                    var isFirstInTheSameContentBlock = otherGridsPriceSeriesIds.Contains(halfMonthPeriodOne?.SeriesId ?? string.Empty);
                    var isSecondInSameContentBlock = otherGridsPriceSeriesIds.Contains(halfMonthPeriodTwo?.SeriesId ?? string.Empty);

                    var areDerivationInputsInSameContentBlock = isFirstInTheSameContentBlock && isSecondInSameContentBlock;

                    if (areDerivationInputsInSameContentBlock)
                    {
                        continue;
                    }

                    if (!isFirstInTheSameContentBlock && !WorkflowStatus.IsPublishedOrCorrectionPublishedStatusMatch(halfMonthPeriodOne?.Status))
                    {
                        return false;
                    }

                    if (!isSecondInSameContentBlock && !WorkflowStatus.IsPublishedOrCorrectionPublishedStatusMatch(halfMonthPeriodTwo?.Status))
                    {
                        return false;
                    }
                }
                else
                {
                    if (priceSeriesItems.All(x => otherGridsPriceSeriesIds.Contains(x.SeriesId)))
                    {
                        continue;
                    }

                    if (priceSeriesItems.Any(x => x.Status != WorkflowStatus.Published.Value && x.Status != WorkflowStatus.CorrectionPublished.Value))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private async Task<(WorkflowBusinessKey WorkflowBusinessKey, WorkflowId WorkflowId)> StartWorkflow(
            BusinessLayer.ContentBlock.Repositories.Models.ContentBlock contentBlock,
            DataPackage dataPackage,
            OperationType operationType,
            ReviewPageUrl reviewPageUrl)
        {
            var firstSeries = await priceSeriesService.GetPriceSeriesById(contentBlock.GetPriceSeriesIds().First());

            var publishOnDate = await GetPublishOnDate(firstSeries, dataPackage.AssessedDateTime);

            var variables = new Dictionary<string, string>
            {
                ["content"] = contentBlock.Title ?? string.Empty,
                ["contentType"] = "Pricing",
                ["location"] = firstSeries.Location.Region.Name.English!,
                ["location_id"] = firstSeries.Location.Region.Guid.ToString()!,
                ["market"] = firstSeries.Commodity.Name.English!,
                ["commodity"] = firstSeries.Commodity.Name.English!,
                ["commodity_id"] = firstSeries.Commodity.Guid.ToString()
            };

            var workflowBusinessKey = new WorkflowBusinessKey(dataPackage.Id);
            var workflowVersion = workflowService.GetWorkflowVersion(firstSeries.Commodity.Name.English!);
            var workflowId = await workflowService.StartWorkflow(
                workflowBusinessKey,
                workflowVersion,
                variables,
                operationType,
                publishOnDate,
                reviewPageUrl);
            return (workflowBusinessKey, workflowId);
        }

        private async Task<DateTime> GetPublishOnDate(PriceSeries priceSeries, DateTime assessedDateTime)
        {
            var nextPublicationDate = await publicationScheduleService.GetNextPublicationDate(
                priceSeries,
                assessedDateTime);

            var scheduledPublishDate = nextPublicationDate?.ScheduledPublishDate;

            if (scheduledPublishDate.HasValue &&
                DateOnly.FromDateTime(assessedDateTime) == DateOnly.FromDateTime(scheduledPublishDate.Value))
            {
                return scheduledPublishDate.Value;
            }

            return clock.UtcNow.Date;
        }

        private async Task<DataPackageStateTransitionResponse> StartWorkflowAndPerformFirstTransition(
            DataPackageKey dataPackageKey,
            string actionToPerform,
            OperationType operationType,
            ReviewPageUrl reviewPageUrl)
        {
            var validationResult = await ValidateBeforeStateTransition(dataPackageKey, operationType);

            if (!validationResult.IsSuccess)
            {
                return validationResult;
            }

            var contentBlock = await contentBlockDomainService.GetContentBlock(
                                   dataPackageKey.ContentBlockId,
                                   dataPackageKey.ContentBlockVersion);

            var priceSeriesIds = contentBlock!.GetPriceSeriesIds();

            await priceSeriesItemsDomainService.UpdatePeriods(
             priceSeriesIds,
             dataPackageKey.AssessedDateTime);

            var dataPackage = await dataPackageDomainService.BuildDataPackage(dataPackageKey, contentBlock!);

            var (workflowBusinessKey, workflowId) = await StartWorkflow(
                                                        contentBlock!,
                                                        dataPackage,
                                                        OperationType.None,
                                                        reviewPageUrl);

            if (workflowId == WorkflowId.None)
            {
                return DataPackageStateTransitionResponse.Error(ErrorCodes.FailedToInitializeWorkflow);
            }

            dataPackage.WorkflowData = new WorkflowData
            {
                Id = workflowId.Value,
                BusinessKey = workflowBusinessKey.Value
            };

            var isSuccess = await workflowService.PerformAction(workflowBusinessKey, actionToPerform);
            if (isSuccess)
            {
                await dataPackageDomainService.Save(dataPackage);
                return DataPackageStateTransitionResponse.Success;
            }

            return DataPackageStateTransitionResponse.Error(ErrorCodes.WorkflowTransitionFailed);
        }

        [ExcludeFromCodeCoverage(Justification = "All error use cases have to be covered later. Success case is already covered.")]
        private async Task<DataPackageStateTransitionResponse> ValidateBeforeStateTransition(
            DataPackageKey dataPackageKey,
            OperationType operationType)
        {
            var contentBlock = await contentBlockService.GetContentBlock(
                                   dataPackageKey.ContentBlockId,
                                   dataPackageKey.ContentBlockVersion,
                                   dataPackageKey.AssessedDateTime,
                                   false);
            var contentBlockValidationResult = ValidateContentBlock(contentBlock);

            if (!contentBlockValidationResult.Success)
            {
                return DataPackageStateTransitionResponse.Error(contentBlockValidationResult.ErrorCode!);
            }

            var validationResult = await ValidateDataPackage(contentBlock!, dataPackageKey.AssessedDateTime);

            if (!validationResult.Valid)
            {
                return DataPackageStateTransitionResponse.Error(validationResult.ErrorCode!);
            }

            if (operationType == OperationType.Correction)
            {
                var correctionValidations = await ValidateCorrection(dataPackageKey, contentBlock);

                if (!correctionValidations.Success)
                {
                    return DataPackageStateTransitionResponse.Error(correctionValidations.ErrorCode!);
                }
            }

            return DataPackageStateTransitionResponse.Success;
        }

        private (bool Success, string? ErrorCode) ValidateContentBlock(ContentBlockDefinition? contentBlock)
        {
            if (contentBlock == null)
            {
                return (false, ErrorCodes.ContentBlockNotFound);
            }

            if (contentBlock.GetPriceSeriesIds().Count == 0)
            {
                return (false, ErrorCodes.InputPriceSeriesIdsEmpty);
            }

            return (true, null);
        }

        private async Task<(bool Valid, string? ErrorCode)> ValidateDataPackage(
            ContentBlockDefinition contentBlock,
            DateTime assessedDateTime)
        {
            foreach (var priceSeriesGrid in contentBlock.PriceSeriesGrids!)
            {
                var otherGridsPriceSeriesIds = contentBlock.PriceSeriesGrids!
                                               .Where(x => x.Id != priceSeriesGrid.Id)
                                               .SelectMany(x => x.PriceSeriesIds!).ToList();

                var validationResults = await ValidatePriceSeriesItems(priceSeriesGrid, assessedDateTime);

                if (!validationResults.Valid)
                {
                    return (false, ErrorCodes.NotAllPriceSeriesItemsAreValid);
                }

                if (!await AreInputsPublishedOrInSameContentBlock(assessedDateTime, priceSeriesGrid.PriceSeriesIds!, otherGridsPriceSeriesIds))
                {
                    return (false, ErrorCodes.NotAllInputsArePublished);
                }

                if (!await AreAllReferencePriceSeriesPublishedOrInSameContentBlock(priceSeriesGrid, assessedDateTime, otherGridsPriceSeriesIds))
                {
                    return (false, ErrorCodes.NotAllReferencePriceSeriesArePublished);
                }
            }

            return (true, null);
        }

        private async Task<(bool Valid, List<string> PriceSeriesItemIds)> ValidatePriceSeriesItems(
            PriceSeriesGrid priceSeriesGrid,
            DateTime assessedDateTime)
        {
            var priceItemService = seriesItemTypeServiceFactory.GetPriceItemService(
                SeriesItemTypeCodeFactory.GetSeriesItemTypeCode(priceSeriesGrid.SeriesItemTypeCode!));

            var filteredPriceSeries = await priceSeriesService.GetActivePriceSeriesByIds(
               priceSeriesGrid.PriceSeriesIds!,
               assessedDateTime);

            var validationResults =
                await priceItemService.ValidatePriceSeriesItems(filteredPriceSeries.Select(x => x.Id).ToList(), assessedDateTime);

            return (validationResults.Valid, validationResults.PriceSeriesItemIds);
        }

        private async Task<(bool Success, string? ErrorCode)> ValidateCorrection(
            DataPackageKey dataPackageKey,
            ContentBlockDefinition? contentBlock)
        {
            var priceSeriesItems = await priceSeriesItemsDomainService.GetPriceSeriesItems(contentBlock!.GetPriceSeriesIds(), dataPackageKey.AssessedDateTime);
            var priceSeriesItemsHavePendingChanges = priceSeriesItems.Any(x => x.PendingChanges != null);

            var commentary = await commentaryDomainService.GetCommentary(contentBlock!.ContentBlockId, dataPackageKey.AssessedDateTime);
            var commentaryHasPendingChanges = commentary?.PendingChanges != null;

            if (priceSeriesItemsHavePendingChanges || commentaryHasPendingChanges)
            {
                return (true, null);
            }

            return (false, ErrorCodes.CorrectionContainsNoChanges);
        }

        private List<WorkflowAction> GetRepublishAction()
        {
            return workflowSettings.ShowRepublishButtonToggle
                       ? [new WorkflowAction { IsAllowed = true, Key = UserActions.InitiateCorrection, Value = "Re-publish" }]
                       : [];
        }

        private static class ErrorCodes
        {
            public const string ContentBlockNotFound = "CONTENT_BLOCK_NOT_FOUND";

            public const string DataPackageNotFound = "DATA_PACKAGE_NOT_FOUND";

            public const string FailedToInitializeWorkflow = "FAILED_TO_INITIALIZE_WORKFLOW";

            public const string InputPriceSeriesIdsEmpty = "INPUT_PRICE_SERIES_IDS_EMPTY";

            public const string NotAllInputsArePublished = "NOT_ALL_INPUTS_ARE_PUBLISHED";

            public const string NotAllPriceSeriesItemsAreValid = "NOT_ALL_PRICE_SERIES_ITEMS_ARE_VALID";

            public const string NotAllReferencePriceSeriesArePublished = "NOT_ALL_REFERENCE_PRICE_SERIES_ARE_PUBLISHED";

            public const string WorkflowTransitionFailed = "FAILED_TO_TRANSITION_TO_NEXT_STATUS";

            public const string CorrectionContainsNoChanges = "CORRECTION_CONTAINS_NO_CHANGES";
        }
    }
}