namespace Test.Infrastructure.Authoring.BusinessFlows.PriceEntryBusinessFlow
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;

    using BusinessLayer.PriceEntry.DTOs.Input;
    using BusinessLayer.PriceEntry.ValueObjects;

    using global::Infrastructure.Services.Workflow;

    using Test.Infrastructure.Authoring.BusinessFlows.PriceEntryBusinessFlow.Processes;

    public class PriceEntryBusinessFlow
    {
        private const int ContentBlockVersion = 1;

        private readonly HttpClient httpClient;

        private readonly List<IPriceEntryBusinessFlowProcess> processes = new();

        private readonly string contentBlockId;

        private readonly ICollection<ICollection<string>>? priceSeriesIds;

        public PriceEntryBusinessFlow(string contentBlockId, ICollection<string>? seriesIds, HttpClient httpClient)
            : this(contentBlockId, [seriesIds ?? []], httpClient)
        {
        }

        public PriceEntryBusinessFlow(
            string contentBlockId,
            ICollection<ICollection<string>>? priceSeriesIds,
            HttpClient httpClient)
        {
            this.contentBlockId = contentBlockId ?? throw new ArgumentNullException(nameof(contentBlockId));
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            this.priceSeriesIds = priceSeriesIds;
        }

        public PriceEntryBusinessFlow SaveContentBlockDefinition()
        {
            processes.Add(new ContentBlockDefinitionProcess(
                contentBlockId,
                priceSeriesIds?.Select(x => x.ToArray()).ToArray()));

            return this;
        }

        public PriceEntryBusinessFlow SaveRangePriceSeriesItem(
            string seriesId,
            DateTime assessedDateTime,
            SeriesItem seriesItem,
            string operationType = "")
        {
            return SavePriceSeriesItem(seriesId, assessedDateTime, SeriesItemTypeCode.RangeSeries, seriesItem, operationType);
        }

        public PriceEntryBusinessFlow SaveSingleValuePriceSeriesItem(
            string seriesId,
            DateTime assessedDateTime,
            SeriesItem seriesItem,
            string operationType = "")
        {
            return SavePriceSeriesItem(seriesId, assessedDateTime, SeriesItemTypeCode.SingleValueSeries, seriesItem, operationType);
        }

        public PriceEntryBusinessFlow SaveSingleValueWithReferencePriceSeriesItem(
            string seriesId,
            DateTime assessedDateTime,
            SeriesItem seriesItem,
            string operationType = "")
        {
            return SavePriceSeriesItem(seriesId, assessedDateTime, SeriesItemTypeCode.SingleValueWithReferenceSeries, seriesItem, operationType);
        }

        public PriceEntryBusinessFlow SaveCharterRateSingleValuePriceSeriesItem(
            string seriesId,
            DateTime assessedDateTime,
            SeriesItem seriesItem)
        {
            return SavePriceSeriesItem(seriesId, assessedDateTime, SeriesItemTypeCode.CharterRateSingleValueSeries, seriesItem);
        }

        public PriceEntryBusinessFlow SavePriceSeriesItem(string seriesId, DateTime assessedDateTime, SeriesItemTypeCode seriesItemTypeCode, SeriesItem seriesItem, string operationType = "")
        {
            var priceItemInput = new PriceItemInput
            {
                SeriesId = seriesId,
                AssessedDateTime = assessedDateTime,
                SeriesItemTypeCode = seriesItemTypeCode.Value,
                SeriesItem = seriesItem,
                OperationType = operationType
            };
            processes.Add(new SavePriceSeriesItemProcess(priceItemInput));

            return this;
        }

        public PriceEntryBusinessFlow SaveCommentary(DateTime assessedDateTime, string? commentaryVersion = null, string operationType = "")
        {
            processes.Add(new SaveCommentaryProcess(contentBlockId, assessedDateTime, commentaryVersion, operationType));

            return this;
        }

        public PriceEntryBusinessFlow InitiatePublish(DateTime assessedDateTime)
        {
            processes.Add(new InitiatePublishProcess(
                contentBlockId,
                ContentBlockVersion,
                assessedDateTime));

            return this;
        }

        public PriceEntryBusinessFlow AcknowledgePublished(DateTime assessedDateTime)
        {
            processes.Add(new AcknowledgePublishedProcess(contentBlockId, assessedDateTime, WorkflowStatus.Published.Value));

            return this;
        }

        public PriceEntryBusinessFlow InitiateCorrection(DateTime assessedDateTime)
        {
            processes.Add(new InitiateCorrectionProcess(contentBlockId, ContentBlockVersion, assessedDateTime));

            return this;
        }

        public PriceEntryBusinessFlow AddSeriesIdToGrid(int gridIndex, string seriesId)
        {
            if (priceSeriesIds == null)
            {
                throw new InvalidOperationException("PriceSeriesIds collection is null.");
            }

            priceSeriesIds.ElementAt(gridIndex).Add(seriesId);

            return this;
        }

        public PriceEntryBusinessFlow InitiateSendForReview(DateTime assessedDateTime)
        {
            return TransitionToState(assessedDateTime, UserActions.SendForReview);
        }

        public PriceEntryBusinessFlow AcknowledgeSendForReview(DateTime assessedDateTime)
        {
            return AcknowledgeStateTransitioned(assessedDateTime, WorkflowStatus.ReadyForReview.Value);
        }

        public PriceEntryBusinessFlow InitiateStartReview(DateTime assessedDateTime)
        {
            return TransitionToState(assessedDateTime, UserActions.StartReview);
        }

        public PriceEntryBusinessFlow AcknowledgeStartReview(DateTime assessedDateTime)
        {
            return AcknowledgeStateTransitioned(assessedDateTime, WorkflowStatus.InReview.Value);
        }

        public PriceEntryBusinessFlow InitiateApproval(DateTime assessedDateTime)
        {
            return TransitionToState(assessedDateTime, UserActions.Approve);
        }

        public PriceEntryBusinessFlow AcknowledgeApproval(DateTime assessedDateTime)
        {
            return AcknowledgeStateTransitioned(assessedDateTime, WorkflowStatus.ReadyToPublish.Value);
        }

        public PriceEntryBusinessFlow InitiateSendBack(DateTime assessedDateTime)
        {
            return TransitionToState(assessedDateTime, UserActions.SendBack);
        }

        public PriceEntryBusinessFlow AcknowledgeSendBack(DateTime assessedDateTime)
        {
            return AcknowledgeStateTransitioned(assessedDateTime, WorkflowStatus.ReturnedToAuthor.Value);
        }

        public PriceEntryBusinessFlow InitiatePullBack(DateTime assessedDateTime)
        {
            return TransitionToState(assessedDateTime, UserActions.PullBack);
        }

        public PriceEntryBusinessFlow AcknowledgePullBack(DateTime assessedDateTime)
        {
            return AcknowledgeStateTransitioned(assessedDateTime, WorkflowStatus.Draft.Value);
        }

        public PriceEntryBusinessFlow InitiateCancel(DateTime assessedDateTime)
        {
            return TransitionToState(assessedDateTime, UserActions.Cancel);
        }

        public PriceEntryBusinessFlow AcknowledgeCancelled(DateTime assessedDateTime)
        {
            return AcknowledgeStateTransitioned(assessedDateTime, WorkflowStatus.Cancelled.Value);
        }

        public PriceEntryBusinessFlow AdvancedCorrectionInitiateSendForReview(DateTime assessedDateTime)
        {
            return AdvancedCorrectionTransitionToState(assessedDateTime, UserActions.SendForReview);
        }

        public PriceEntryBusinessFlow AdvancedCorrectionAcknowledgeSendForReview(DateTime assessedDateTime)
        {
            return AcknowledgeStateTransitioned(assessedDateTime, WorkflowStatus.CorrectionReadyForReview.Value);
        }

        public PriceEntryBusinessFlow AdvancedCorrectionInitiateStartReview(DateTime assessedDateTime)
        {
            return AdvancedCorrectionTransitionToState(assessedDateTime, UserActions.StartReview);
        }

        public PriceEntryBusinessFlow AdvancedCorrectionAcknowledgeStartReview(DateTime assessedDateTime)
        {
            return AcknowledgeStateTransitioned(assessedDateTime, WorkflowStatus.CorrectionInReview.Value);
        }

        public PriceEntryBusinessFlow AdvancedCorrectionInitiateApproval(DateTime assessedDateTime)
        {
            return AdvancedCorrectionTransitionToState(assessedDateTime, UserActions.Approve);
        }

        public PriceEntryBusinessFlow AdvancedCorrectionAcknowledgeApproval(DateTime assessedDateTime)
        {
            return AcknowledgeStateTransitioned(assessedDateTime, WorkflowStatus.CorrectionReadyToPublish.Value);
        }

        public PriceEntryBusinessFlow AdvancedCorrectionAcknowledgePublished(DateTime assessedDateTime)
        {
            processes.Add(new AcknowledgePublishedProcess(contentBlockId, assessedDateTime, WorkflowStatus.CorrectionPublished.Value));

            return this;
        }

        public PriceEntryBusinessFlow StartNonMarketAdjustment(DateTime assessedDateTime)
        {
            processes.Add(new ToggleNonMarketAdjustmentProcess(contentBlockId, ContentBlockVersion, assessedDateTime, true));
            return this;
        }

        public PriceEntryBusinessFlow CancelNonMarketAdjustment(DateTime assessedDateTime)
        {
            processes.Add(new ToggleNonMarketAdjustmentProcess(contentBlockId, ContentBlockVersion, assessedDateTime, false));
            return this;
        }

        public PriceEntryBusinessFlow PublishWithAdvanceWorkflow(DateTime assessedDateTime)
        {
            return this
               .InitiateSendForReview(assessedDateTime)
               .AcknowledgeSendForReview(assessedDateTime)
               .InitiateStartReview(assessedDateTime)
               .AcknowledgeStartReview(assessedDateTime)
               .InitiateApproval(assessedDateTime)
               .AcknowledgeApproval(assessedDateTime)
               .AcknowledgePublished(assessedDateTime);
        }

        public PriceEntryBusinessFlow PublishWithAdvanceCorrectionWorkflow(DateTime assessedDateTime)
        {
            return this
                .AdvancedCorrectionInitiateSendForReview(assessedDateTime)
                .AdvancedCorrectionAcknowledgeSendForReview(assessedDateTime)
                .AdvancedCorrectionInitiateStartReview(assessedDateTime)
                .AdvancedCorrectionAcknowledgeStartReview(assessedDateTime)
                .AdvancedCorrectionInitiateApproval(assessedDateTime)
                .AdvancedCorrectionAcknowledgeApproval(assessedDateTime)
                .AdvancedCorrectionAcknowledgePublished(assessedDateTime);
        }

        public async Task<HttpResponseMessage> Execute()
        {
            if (processes.Count == 0)
            {
                throw new InvalidOperationException("No processes have been added to the business flow.");
            }

            HttpResponseMessage? response = null;

            foreach (var process in processes)
            {
                response = await process.Execute(httpClient);
            }

            processes.Clear();

            return response!;
        }

        private PriceEntryBusinessFlow TransitionToState(DateTime assessedDateTime, string action)
        {
            processes.Add(new TransitionToStateProcess(contentBlockId, ContentBlockVersion, assessedDateTime, action, string.Empty));
            return this;
        }

        private PriceEntryBusinessFlow AdvancedCorrectionTransitionToState(DateTime assessedDateTime, string action)
        {
            processes.Add(new TransitionToStateProcess(contentBlockId, ContentBlockVersion, assessedDateTime, action, OperationType.Correction.Value));
            return this;
        }

        private PriceEntryBusinessFlow AcknowledgeStateTransitioned(DateTime assessedDateTime, string nextState)
        {
            processes.Add(new AcknowledgeStateTransitioned(contentBlockId, ContentBlockVersion, assessedDateTime, nextState));
            return this;
        }
    }
}