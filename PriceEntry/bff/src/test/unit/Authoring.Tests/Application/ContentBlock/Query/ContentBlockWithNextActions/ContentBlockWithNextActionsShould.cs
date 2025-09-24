namespace Authoring.Tests.Application.ContentBlock.Query.ContentBlockWithNextActions
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Authoring.Tests.Functional.Tests.Common.Infrastructure;

    using global::BusinessLayer.PriceEntry.DTOs.Input;

    using Snapshooter;
    using Snapshooter.Xunit;

    using Test.Infrastructure.Authoring;
    using Test.Infrastructure.Authoring.BusinessFlows.PriceEntryBusinessFlow;
    using Test.Infrastructure.Authoring.BusinessFlows.PriceEntryBusinessFlow.PriceSeriesItemBuilders;
    using Test.Infrastructure.Authoring.GraphQLClients.PriceEntry;
    using Test.Infrastructure.Extensions;
    using Test.Infrastructure.TestData;

    using Xunit;

    [Collection(WebApplicationCollection.Name)]
    public class ContentBlockWithNextActionsShould : WebApplicationTestBase
    {
        private const string ContentBlockId = "contentBlockId";

        private static readonly DateTime AssessedDateTime = TestData.Now;

        private readonly ContentBlockGraphQLClient contentBlockClient;

        public ContentBlockWithNextActionsShould(AuthoringBffApplicationFactory factory)
            : base(factory)
        {
            contentBlockClient = new ContentBlockGraphQLClient(GraphQLClient);
        }

        public static TheoryData<string, Func<PriceEntryBusinessFlow, DateTime, PriceEntryBusinessFlow>>
            SimpleWorkflowUntilStatus =>
            new()
            {
                { "IN_DRAFT", (businessFlow, now) => businessFlow },
                { "PUBLISH_IN_PROGRESS", (businessFlow, now) => businessFlow.InitiatePublish(now) },
                { "PUBLISHED", (businessFlow, now) => businessFlow.InitiatePublish(now).AcknowledgePublished(now) },
                {
                    "CORRECTION_INITIATED",
                    (businessFlow, now) =>
                        businessFlow.InitiatePublish(now).AcknowledgePublished(now).InitiateCorrection(now)
                }
            };

        public static TheoryData<string, Func<PriceEntryBusinessFlow, DateTime, PriceEntryBusinessFlow>>
            AdvancedWorkflowUntilStatus =>
            new()
            {
                {
                    "PUBLISHED",
                    (businessFlow, now) =>
                        businessFlow
                           .SaveContentBlockDefinition()
                           .SaveRangePriceSeriesItem(
                                TestSeries.Melamine_Asia_SE_Spot,
                                AssessedDateTime,
                                RangePriceSeriesItemBuilder.BuildDefaultValidSeriesItem())
                           .InitiateSendForReview(AssessedDateTime)
                           .AcknowledgeSendForReview(AssessedDateTime)
                           .InitiateStartReview(AssessedDateTime)
                           .AcknowledgeStartReview(AssessedDateTime)
                           .InitiateApproval(AssessedDateTime)
                           .AcknowledgeApproval(AssessedDateTime)
                           .AcknowledgePublished(AssessedDateTime)
                },
                {
                    "CORRECTION_PUBLISHED",
                    (businessFlow, now) =>
                        businessFlow
                           .SaveContentBlockDefinition()
                           .SaveRangePriceSeriesItem(
                                TestSeries.Melamine_Asia_SE_Spot,
                                AssessedDateTime,
                                RangePriceSeriesItemBuilder.BuildDefaultValidSeriesItem())
                           .InitiateSendForReview(AssessedDateTime)
                           .AcknowledgeSendForReview(AssessedDateTime)
                           .InitiateStartReview(AssessedDateTime)
                           .AcknowledgeStartReview(AssessedDateTime)
                           .InitiateApproval(AssessedDateTime)
                           .AcknowledgeApproval(AssessedDateTime)
                           .AcknowledgePublished(AssessedDateTime)
                           .InitiateCorrection(now)
                           .SaveRangePriceSeriesItem(
                                TestSeries.Melamine_Asia_SE_Spot,
                                AssessedDateTime,
                                new SeriesItem { PriceLow = 10, PriceHigh = 50 },
                                "correction")
                           .AdvancedCorrectionInitiateSendForReview(now)
                           .AdvancedCorrectionAcknowledgeSendForReview(now)
                           .AdvancedCorrectionInitiateStartReview(now)
                           .AdvancedCorrectionAcknowledgeStartReview(now)
                           .AdvancedCorrectionInitiateApproval(now)
                           .AdvancedCorrectionAcknowledgeApproval(now)
                           .AdvancedCorrectionAcknowledgePublished(now)
                }
            };

        [Theory]
        [MemberData(nameof(SimpleWorkflowUntilStatus))]
        public async Task Return_Next_Actions_For_Simple_Workflow_In_Scenario(
            string scenario,
            Func<PriceEntryBusinessFlow, DateTime, PriceEntryBusinessFlow> configureWorkflowUntilStatus)
        {
            var businessFlow =
                new PriceEntryBusinessFlow(ContentBlockId, new List<string> { TestSeries.LNG_China_HM1 }, HttpClient)
                   .SaveContentBlockDefinition()
                   .SaveSingleValueWithReferencePriceSeriesItem(
                        TestSeries.LNG_China_HM1,
                        AssessedDateTime,
                        SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice());

            _ = await configureWorkflowUntilStatus.Invoke(businessFlow, AssessedDateTime).Execute();

            var result = await contentBlockClient
                            .GetContentBlockWithNextActionsOnly(ContentBlockId, AssessedDateTime)
                            .GetRawResponse();

            result.MatchSnapshot(SnapshotNameExtension.Create(scenario));
        }

        [Fact]
        public async Task Return_Publish_Action_For_Simple_Workflow_When_All_PriceSeries_Have_Status_ReadyToStart()
        {
            var businessFlow = new PriceEntryBusinessFlow(
                ContentBlockId,
                new List<string> { TestSeries.LNG_China_HM1 },
                HttpClient).SaveContentBlockDefinition();

            _ = await businessFlow.Execute();

            var editorMode = await contentBlockClient
                                .GetContentBlockWithNextActionsOnly(
                                     ContentBlockId,
                                     AssessedDateTime,
                                     isReviewMode: false)
                                .GetRawResponse();

            editorMode.MatchSnapshot(SnapshotNameExtension.Create("editor"));

            var copyEditorMode = await contentBlockClient
                                    .GetContentBlockWithNextActionsOnly(
                                         ContentBlockId,
                                         AssessedDateTime,
                                         isReviewMode: true)
                                    .GetRawResponse();

            copyEditorMode.MatchSnapshot(SnapshotNameExtension.Create("copy_editor"));
        }

        [Fact]
        public async Task
            Return_SendForReview_Action_For_Advance_Workflow_When_Any_PriceSeries_Has_Status_InProgress()
        {
            var businessFlow =
                new PriceEntryBusinessFlow(
                        ContentBlockId,
                        new List<string> { TestSeries.Melamine_Asia_SE_Spot },
                        HttpClient)
                   .SaveContentBlockDefinition()
                   .SaveRangePriceSeriesItem(
                        TestSeries.Melamine_Asia_SE_Spot,
                        AssessedDateTime,
                        RangePriceSeriesItemBuilder.BuildDefaultValidSeriesItem());

            _ = await businessFlow.Execute();

            var editorMode = await contentBlockClient
                                .GetContentBlockWithNextActionsOnly(
                                     ContentBlockId,
                                     AssessedDateTime,
                                     isReviewMode: false)
                                .GetRawResponse();

            editorMode.MatchSnapshot(SnapshotNameExtension.Create("editor"));

            var copyEditorMode = await contentBlockClient
                                    .GetContentBlockWithNextActionsOnly(
                                         ContentBlockId,
                                         AssessedDateTime,
                                         isReviewMode: true)
                                    .GetRawResponse();

            copyEditorMode.MatchSnapshot(SnapshotNameExtension.Create("copy_editor"));
        }

        [Fact]
        public async Task Return_SendForReview_Action_For_Advance_Workflow_When_All_PriceSeries_Have_Status_ReadyToStart()
        {
            var businessFlow = new PriceEntryBusinessFlow(
                ContentBlockId,
                new List<string> { TestSeries.Melamine_Asia_SE_Spot },
                HttpClient).SaveContentBlockDefinition();

            _ = await businessFlow.Execute();

            var editorMode = await contentBlockClient
                                .GetContentBlockWithNextActionsOnly(
                                     ContentBlockId,
                                     AssessedDateTime,
                                     isReviewMode: false)
                                .GetRawResponse();

            editorMode.MatchSnapshot(SnapshotNameExtension.Create("editor"));

            var copyEditorMode = await contentBlockClient
                                    .GetContentBlockWithNextActionsOnly(
                                         ContentBlockId,
                                         AssessedDateTime,
                                         isReviewMode: true)
                                    .GetRawResponse();

            copyEditorMode.MatchSnapshot(SnapshotNameExtension.Create("copy_editor"));
        }

        [Fact]
        public async Task Return_Action_From_Workflow_For_Advance_Workflow_When_Not_In_First_State()
        {
            var businessFlow =
                new PriceEntryBusinessFlow(
                        ContentBlockId,
                        new List<string> { TestSeries.Melamine_Asia_SE_Spot },
                        HttpClient)
                   .SaveContentBlockDefinition()
                   .SaveRangePriceSeriesItem(
                        TestSeries.Melamine_Asia_SE_Spot,
                        AssessedDateTime,
                        RangePriceSeriesItemBuilder.BuildDefaultValidSeriesItem())
                   .InitiateSendForReview(AssessedDateTime)
                   .AcknowledgeSendForReview(AssessedDateTime);

            _ = await businessFlow.Execute();

            var editorMode = await contentBlockClient
                                .GetContentBlockWithNextActionsOnly(
                                     ContentBlockId,
                                     AssessedDateTime,
                                     isReviewMode: false)
                                .GetRawResponse();

            editorMode.MatchSnapshot(SnapshotNameExtension.Create("editor"));

            var copyEditorMode = await contentBlockClient
                                    .GetContentBlockWithNextActionsOnly(
                                         ContentBlockId,
                                         AssessedDateTime,
                                         isReviewMode: true)
                                    .GetRawResponse();

            copyEditorMode.MatchSnapshot(SnapshotNameExtension.Create("copy_editor"));
        }

        [Fact]
        public async Task Return_Nothing_When_Content_Block_Has_No_Price_Series_Defined()
        {
            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, new List<string>(), HttpClient)
               .SaveContentBlockDefinition();

            _ = await businessFlow.Execute();

            var editorMode = await contentBlockClient
                                .GetContentBlockWithNextActionsOnly(
                                     ContentBlockId,
                                     AssessedDateTime,
                                     isReviewMode: false)
                                .GetRawResponse();

            editorMode.MatchSnapshot(SnapshotNameExtension.Create("editor"));

            var copyEditorMode = await contentBlockClient
                                    .GetContentBlockWithNextActionsOnly(
                                         ContentBlockId,
                                         AssessedDateTime,
                                         isReviewMode: true)
                                    .GetRawResponse();

            copyEditorMode.MatchSnapshot(SnapshotNameExtension.Create("copy_editor"));
        }

        [Theory]
        [MemberData(nameof(AdvancedWorkflowUntilStatus))]
        public async Task Return_Next_Actions_For_Advance_Workflow_In_Scenario(
            string scenario,
            Func<PriceEntryBusinessFlow, DateTime, PriceEntryBusinessFlow> configureWorkflowUntilStatus)
        {
            var businessFlow = new PriceEntryBusinessFlow(
                ContentBlockId,
                new List<string> { TestSeries.Melamine_Asia_SE_Spot },
                HttpClient);

            _ = await configureWorkflowUntilStatus.Invoke(businessFlow, AssessedDateTime).Execute();

            var result = await contentBlockClient
                            .GetContentBlockWithNextActionsOnly(ContentBlockId, AssessedDateTime)
                            .GetRawResponse();

            result.MatchSnapshot(SnapshotNameExtension.Create(scenario));
        }

        [Fact]
        public async Task Return_Next_Actions_After_Starting_NonMarketAdjustment_When_All_PriceSeries_Have_Status_ReadyToStart()
        {
            var businessFlow = new PriceEntryBusinessFlow(
                ContentBlockId,
                new List<string> { TestSeries.Melamine_Asia_SE_Spot },
                HttpClient)
                .SaveContentBlockDefinition()
                .StartNonMarketAdjustment(AssessedDateTime);

            _ = await businessFlow.Execute();

            var editorMode = await contentBlockClient
                                .GetContentBlockWithNextActionsOnly(
                                     ContentBlockId,
                                     AssessedDateTime,
                                     isReviewMode: false)
                                .GetRawResponse();

            editorMode.MatchSnapshot(SnapshotNameExtension.Create("editor"));

            var copyEditorMode = await contentBlockClient
                                    .GetContentBlockWithNextActionsOnly(
                                         ContentBlockId,
                                         AssessedDateTime,
                                         isReviewMode: true)
                                    .GetRawResponse();

            copyEditorMode.MatchSnapshot(SnapshotNameExtension.Create("copy_editor"));
        }

        [Fact]
        public async Task Return_Next_Actions_After_Cancelling_NonMarketAdjustment_When_All_PriceSeries_Have_Status_ReadyToStart()
        {
            var businessFlow = new PriceEntryBusinessFlow(
                ContentBlockId,
                new List<string> { TestSeries.Melamine_Asia_SE_Spot },
                HttpClient)
                .SaveContentBlockDefinition()
                .StartNonMarketAdjustment(AssessedDateTime)
                .CancelNonMarketAdjustment(AssessedDateTime);

            _ = await businessFlow.Execute();

            var editorMode = await contentBlockClient
                                .GetContentBlockWithNextActionsOnly(
                                     ContentBlockId,
                                     AssessedDateTime,
                                     isReviewMode: false)
                                .GetRawResponse();

            editorMode.MatchSnapshot(SnapshotNameExtension.Create("editor"));

            var copyEditorMode = await contentBlockClient
                                    .GetContentBlockWithNextActionsOnly(
                                         ContentBlockId,
                                         AssessedDateTime,
                                         isReviewMode: true)
                                    .GetRawResponse();

            copyEditorMode.MatchSnapshot(SnapshotNameExtension.Create("copy_editor"));
        }
    }
}
