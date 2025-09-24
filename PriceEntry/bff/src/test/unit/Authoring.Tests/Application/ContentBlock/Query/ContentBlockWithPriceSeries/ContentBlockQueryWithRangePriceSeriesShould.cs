namespace Authoring.Tests.Application.ContentBlock.Query.ContentBlockWithPriceSeries
{
    using System;
    using System.Threading.Tasks;

    using Authoring.Tests.Functional.Tests.Common.Infrastructure;

    using FluentAssertions;

    using global::BusinessLayer.PriceEntry.DTOs.Input;
    using global::BusinessLayer.PriceEntry.ValueObjects;

    using Snapshooter;
    using Snapshooter.Xunit;

    using Test.Infrastructure.Authoring;
    using Test.Infrastructure.Authoring.BusinessFlows.PriceEntryBusinessFlow;
    using Test.Infrastructure.Authoring.BusinessFlows.PriceEntryBusinessFlow.PriceSeriesItemBuilders;
    using Test.Infrastructure.Extensions;
    using Test.Infrastructure.Services;
    using Test.Infrastructure.Stubs;
    using Test.Infrastructure.TestData;

    using Xunit;

    [Collection(WebApplicationCollection.Name)]
    public class ContentBlockQueryWithRangePriceSeriesShould : ContentBlockQueryWithPriceSeriesTestBase
    {
        private readonly DataPackageWorkflowServiceStub dataPackageWorkflowServiceStub;

        private readonly CanvasApiServiceStub canvasApiServiceStub;

        public ContentBlockQueryWithRangePriceSeriesShould(AuthoringBffApplicationFactory factory)
            : base(factory)
        {
            dataPackageWorkflowServiceStub = GetService<DataPackageWorkflowServiceStub>();
            dataPackageWorkflowServiceStub.ClearRequests();

            canvasApiServiceStub = GetService<CanvasApiServiceStub>();
            canvasApiServiceStub.ClearRequests();
        }

        public static TheoryData<string, Func<PriceEntryBusinessFlow, DateTime, PriceEntryBusinessFlow>, string>
            WorkflowUntilStatus =>
            new()
            {
                {
                    "SEND_FOR_REVIEW_IN_PROGRESS",
                    (businessFlow, now) => businessFlow.InitiateSendForReview(now),
                    TestSeries.Melamine_Asia_SE_Spot
                },
                {
                    "READY_FOR_REVIEW",
                    (businessFlow, now) => businessFlow.InitiateSendForReview(now).AcknowledgeSendForReview(now),
                    TestSeries.Melamine_Asia_SE_Spot
                },
                {
                    "IN REVIEW IN PROGRESS",
                    (businessFlow, now) =>
                        businessFlow
                           .InitiateSendForReview(now)
                           .AcknowledgeSendForReview(now)
                           .InitiateStartReview(now),
                    TestSeries.Melamine_Asia_SE_Spot
                },
                {
                    "IN REVIEW",
                    (businessFlow, now) =>
                        businessFlow
                           .InitiateSendForReview(now)
                           .AcknowledgeSendForReview(now)
                           .InitiateStartReview(now)
                           .AcknowledgeStartReview(now),
                    TestSeries.Melamine_Asia_SE_Spot
                },
                {
                    "APPROVE IN PROGRESS",
                    (businessFlow, now) =>
                        businessFlow
                           .InitiateSendForReview(now)
                           .AcknowledgeSendForReview(now)
                           .InitiateStartReview(now)
                           .AcknowledgeStartReview(now)
                           .InitiateApproval(now),
                    TestSeries.Melamine_Asia_SE_Spot
                },
                {
                    "APPROVE",
                    (businessFlow, now) =>
                        businessFlow
                           .InitiateSendForReview(now)
                           .AcknowledgeSendForReview(now)
                           .InitiateStartReview(now)
                           .AcknowledgeStartReview(now)
                           .InitiateApproval(now)
                           .AcknowledgeApproval(now),
                    TestSeries.Melamine_Asia_SE_Spot
                },
                {
                    "PUBLISHED",
                    (businessFlow, now) =>
                        businessFlow
                           .InitiateSendForReview(now)
                           .AcknowledgeSendForReview(now)
                           .InitiateStartReview(now)
                           .AcknowledgeStartReview(now)
                           .InitiateApproval(now)
                           .AcknowledgeApproval(now)
                           .AcknowledgePublished(now),
                    TestSeries.Melamine_Asia_SE_Spot
                },
                {
                    "SEND BACK IN PROGRESS",
                    (businessFlow, now) =>
                        businessFlow
                           .InitiateSendForReview(now)
                           .AcknowledgeSendForReview(now)
                           .InitiateStartReview(now)
                           .AcknowledgeStartReview(now)
                           .InitiateSendBack(now),
                    TestSeries.Melamine_Asia_SE_Spot
                },
                {
                    "SEND BACK",
                    (businessFlow, now) =>
                        businessFlow
                           .InitiateSendForReview(now)
                           .AcknowledgeSendForReview(now)
                           .InitiateStartReview(now)
                           .AcknowledgeStartReview(now)
                           .InitiateSendBack(now)
                           .AcknowledgeSendBack(now),
                    TestSeries.Melamine_Asia_SE_Spot
                },
                {
                    "READY FOR REVIEW - PULL BACK IN PROGRESS",
                    (businessFlow, now) =>
                        businessFlow
                           .InitiateSendForReview(now)
                           .AcknowledgeSendForReview(now)
                           .InitiatePullBack(now),
                    TestSeries.Melamine_Asia_SE_Spot
                },
                {
                    "READY FOR REVIEW - PULL BACK",
                    (businessFlow, now) =>
                        businessFlow
                           .InitiateSendForReview(now)
                           .AcknowledgeSendForReview(now)
                           .InitiatePullBack(now)
                           .AcknowledgePullBack(now),
                    TestSeries.Melamine_Asia_SE_Spot
                },
                {
                    "APPROVE - PULLBACK IN PROGRESS",
                    (businessFlow, now) =>
                        businessFlow
                           .InitiateSendForReview(now)
                           .AcknowledgeSendForReview(now)
                           .InitiateStartReview(now)
                           .AcknowledgeStartReview(now)
                           .InitiateApproval(now)
                           .AcknowledgeApproval(now)
                           .InitiatePullBack(now),
                    TestSeries.Melamine_Asia_SE_Spot
                },
                {
                    "APPROVE - PULL BACK",
                    (businessFlow, now) => businessFlow
                       .InitiateSendForReview(now)
                       .AcknowledgeSendForReview(now)
                       .InitiateStartReview(now)
                       .AcknowledgeStartReview(now)
                       .InitiateApproval(now)
                       .AcknowledgeApproval(now)
                       .InitiatePullBack(now)
                       .AcknowledgePullBack(now),
                    TestSeries.Melamine_Asia_SE_Spot
                },
                {
                    "CORRECTION DRAFT",
                    (businessFlow, now) => businessFlow
                       .InitiateSendForReview(now)
                       .AcknowledgeSendForReview(now)
                       .InitiateStartReview(now)
                       .AcknowledgeStartReview(now)
                       .InitiateApproval(now)
                       .AcknowledgeApproval(now)
                       .AcknowledgePublished(now)
                       .InitiateCorrection(now)
                       .SavePriceSeriesItem(
                            TestSeries.Melamine_Asia_SE_Spot,
                            now,
                            SeriesItemTypeCode.RangeSeries,
                            new SeriesItem { PriceLow = 10, PriceHigh = 50 },
                            "correction"),
                    TestSeries.Melamine_Asia_SE_Spot
                },
                {
                    "CORRECTION READY FOR REVIEW",
                    (businessFlow, now) => businessFlow
                       .InitiateSendForReview(now)
                       .AcknowledgeSendForReview(now)
                       .InitiateStartReview(now)
                       .AcknowledgeStartReview(now)
                       .InitiateApproval(now)
                       .AcknowledgeApproval(now)
                       .AcknowledgePublished(now)
                       .InitiateCorrection(now)
                       .SavePriceSeriesItem(
                            TestSeries.Melamine_Asia_SE_Spot,
                            now,
                            SeriesItemTypeCode.RangeSeries,
                            new SeriesItem { PriceLow = 10, PriceHigh = 50 },
                            "correction")
                       .AdvancedCorrectionInitiateSendForReview(now)
                       .AdvancedCorrectionAcknowledgeSendForReview(now),
                    TestSeries.Melamine_Asia_SE_Spot
                },
                {
                    "CORRECTION IN REVIEW",
                    (businessFlow, now) => businessFlow
                       .InitiateSendForReview(now)
                       .AcknowledgeSendForReview(now)
                       .InitiateStartReview(now)
                       .AcknowledgeStartReview(now)
                       .InitiateApproval(now)
                       .AcknowledgeApproval(now)
                       .AcknowledgePublished(now)
                       .InitiateCorrection(now)
                       .SavePriceSeriesItem(
                            TestSeries.Melamine_Asia_SE_Spot,
                            now,
                            SeriesItemTypeCode.RangeSeries,
                            new SeriesItem { PriceLow = 10, PriceHigh = 50 },
                            "correction")
                       .AdvancedCorrectionInitiateSendForReview(now)
                       .AdvancedCorrectionAcknowledgeSendForReview(now)
                       .AdvancedCorrectionInitiateStartReview(now)
                       .AdvancedCorrectionAcknowledgeStartReview(now),
                    TestSeries.Melamine_Asia_SE_Spot
                },
                {
                    "CORRECTION PUBLISHED",
                    (businessFlow, now) => businessFlow
                       .InitiateSendForReview(now)
                       .AcknowledgeSendForReview(now)
                       .InitiateStartReview(now)
                       .AcknowledgeStartReview(now)
                       .InitiateApproval(now)
                       .AcknowledgeApproval(now)
                       .AcknowledgePublished(now)
                       .InitiateCorrection(now)
                       .SavePriceSeriesItem(
                            TestSeries.Melamine_Asia_SE_Spot,
                            now,
                            SeriesItemTypeCode.RangeSeries,
                            new SeriesItem { PriceLow = 10, PriceHigh = 50 },
                            "correction")
                       .AdvancedCorrectionInitiateSendForReview(now)
                       .AdvancedCorrectionAcknowledgeSendForReview(now)
                       .AdvancedCorrectionInitiateStartReview(now)
                       .AdvancedCorrectionAcknowledgeStartReview(now)
                       .AdvancedCorrectionInitiateApproval(now)
                       .AdvancedCorrectionAcknowledgeApproval(now)
                       .AdvancedCorrectionAcknowledgePublished(now),
                    TestSeries.Melamine_Asia_SE_Spot
                }
            };

        protected override string AlternativePriceSeriesId => TestSeries.Melamine_China_Spot_FOB;

        protected override string PriceSeriesId => TestSeries.Melamine_Asia_SE_Spot;

        protected override SeriesItemTypeCode SeriesItemTypeCode => SeriesItemTypeCode.RangeSeries;

        [Theory]
        [MemberData(nameof(WorkflowUntilStatus))]
        public async Task Return_Price_Series_With_The_Correct_Status(
            string scenario,
            Func<PriceEntryBusinessFlow, DateTime, PriceEntryBusinessFlow> configureWorkflowUntilStatus,
            string seriesId)
        {
            var assessedDateTime = Today;
            var priceSeriesIds = ContentBlockQueryHelper.CreatePriceSeriesIds([seriesId]);

            var businessFlow =
                new PriceEntryBusinessFlow(ContentBlockId, priceSeriesIds, HttpClient)
                   .SaveContentBlockDefinition()
                   .SavePriceSeriesItem(
                        seriesId,
                        assessedDateTime,
                        GetSeriesItemTypeCode(),
                        BuildValidSeriesItem());
            await configureWorkflowUntilStatus.Invoke(businessFlow, assessedDateTime).Execute();

            var contentBlockWithPriceSeriesResponseEditor = await ContentBlockClient
                                                               .GetContentBlockWithPriceSeriesOnly(
                                                                    ContentBlockId,
                                                                    Today,
                                                                    isReviewMode: false)
                                                               .GetRawResponse();

            contentBlockWithPriceSeriesResponseEditor.MatchSnapshotWithSeriesItemId(
                SnapshotNameExtension.Create(scenario, "editor"));

            var contentBlockWithPriceSeriesResponseCopyEditor = await ContentBlockClient
                                                                   .GetContentBlockWithPriceSeriesOnly(
                                                                        ContentBlockId,
                                                                        Today,
                                                                        isReviewMode: true)
                                                                   .GetRawResponse();

            contentBlockWithPriceSeriesResponseCopyEditor.MatchSnapshotWithSeriesItemId(
                SnapshotNameExtension.Create(scenario, "copy_editor"));

            dataPackageWorkflowServiceStub.Requests.MatchSnapshotForWorkflowRequests(
                SnapshotNameExtension.Create(scenario, "Workflow_Requests"));
            canvasApiServiceStub.Requests.MatchSnapshotForCanvasApiRequests(SnapshotNameExtension.Create(scenario, "Canvas_API_Requests"));
        }

        [Fact]
        public async Task Return_Price_Series_With_Default_Reference_Monthly_Period_Label_When_No_Previous_Price_Exists()
        {
            var assessedDateTime = Today;

            var priceSeriesIds = ContentBlockQueryHelper.CreatePriceSeriesIds([TestSeries.Melamine_Monthly_Contract_Europe]);

            var businessFlow =
                new PriceEntryBusinessFlow(ContentBlockId, priceSeriesIds, HttpClient)
                   .SaveContentBlockDefinition();

            await businessFlow.Execute();

            var nextDayResult = await ContentBlockClient
                                   .GetContentBlockWithPriceSeriesOnly(
                                        ContentBlockId,
                                        assessedDateTime.AddDays(1))
                                   .GetRawResponse();

            nextDayResult.MatchSnapshot();
        }

        [Fact]
        public async Task Return_Price_Series_With_Default_Reference_Quarterly_Period_Label_When_No_Previous_Price_Exists()
        {
            var assessedDateTime = Today;
            var priceSeriesIds = ContentBlockQueryHelper.CreatePriceSeriesIds([TestSeries.Melamine_Quarterly_Contract_Europe]);

            var businessFlow =
                new PriceEntryBusinessFlow(ContentBlockId, priceSeriesIds, HttpClient)
                   .SaveContentBlockDefinition();

            await businessFlow.Execute();

            var nextDayResult = await ContentBlockClient
                                   .GetContentBlockWithPriceSeriesOnly(
                                        ContentBlockId,
                                        assessedDateTime.AddDays(1))
                                   .GetRawResponse();

            nextDayResult.MatchSnapshot();
        }

        [Fact]
        public async Task Revert_Price_Series_When_Correction_Cancelled()
        {
            var assessedDateTime = Today;

            var priceSeriesIds = ContentBlockQueryHelper.CreatePriceSeriesIds([TestSeries.Melamine_Asia_SE_Spot]);

            var businessFlow =
                new PriceEntryBusinessFlow(ContentBlockId, priceSeriesIds, HttpClient)
                   .SaveContentBlockDefinition()
                   .SavePriceSeriesItem(
                        TestSeries.Melamine_Asia_SE_Spot,
                        assessedDateTime,
                        GetSeriesItemTypeCode(),
                        new SeriesItem { PriceLow = 10, PriceHigh = 20 })
                   .InitiateSendForReview(assessedDateTime)
                   .AcknowledgeSendForReview(assessedDateTime)
                   .InitiateStartReview(assessedDateTime)
                   .AcknowledgeStartReview(assessedDateTime)
                   .InitiateApproval(assessedDateTime)
                   .AcknowledgeApproval(assessedDateTime)
                   .AcknowledgePublished(assessedDateTime)
                   .InitiateCorrection(assessedDateTime)
                   .SavePriceSeriesItem(
                        TestSeries.Melamine_Asia_SE_Spot,
                        assessedDateTime,
                        GetSeriesItemTypeCode(),
                        new SeriesItem { PriceLow = 20, PriceHigh = 100 },
                        "correction")
                   .AdvancedCorrectionInitiateSendForReview(assessedDateTime)
                   .AdvancedCorrectionAcknowledgeSendForReview(assessedDateTime)
                   .InitiateCancel(assessedDateTime)
                   .AcknowledgeCancelled(assessedDateTime);

            await businessFlow.Execute();

            var contentBlockWithPriceSeriesResponseEditor = await ContentBlockClient
                                                               .GetContentBlockWithPriceSeriesOnly(
                                                                    ContentBlockId,
                                                                    Today,
                                                                    isReviewMode: false)
                                                               .GetRawResponse();

            contentBlockWithPriceSeriesResponseEditor.MatchSnapshotWithSeriesItemId(
                SnapshotNameExtension.Create("editor"));

            var contentBlockWithPriceSeriesResponseCopyEditor = await ContentBlockClient
                                                                   .GetContentBlockWithPriceSeriesOnly(
                                                                        ContentBlockId,
                                                                        Today,
                                                                        isReviewMode: true)
                                                                   .GetRawResponse();

            contentBlockWithPriceSeriesResponseCopyEditor.MatchSnapshotWithSeriesItemId(
                SnapshotNameExtension.Create("copy_editor"));

            dataPackageWorkflowServiceStub.Requests.MatchSnapshotForWorkflowRequests(
                SnapshotNameExtension.Create("Workflow_Requests"));
            canvasApiServiceStub.Requests.MatchSnapshotForCanvasApiRequests(SnapshotNameExtension.Create("Canvas_API_Requests"));
        }

        [Fact]
        public async Task InitiateDeltaCorrectionForNextDate()
        {
            var assessedDateTime = Today;
            var nextDay = Tomorrow;
            var contentBlockNextDay = "ContentBlockIdNextDay";
            var priceSeriesIds = ContentBlockQueryHelper.CreatePriceSeriesIds([TestSeries.Melamine_Asia_SE_Spot]);

            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, priceSeriesIds, HttpClient)
                   .SaveContentBlockDefinition()
                   .SavePriceSeriesItem(
                        TestSeries.Melamine_Asia_SE_Spot,
                        assessedDateTime,
                        GetSeriesItemTypeCode(),
                        new SeriesItem { PriceLow = 10, PriceHigh = 20 })
                   .InitiateSendForReview(assessedDateTime)
                   .AcknowledgeSendForReview(assessedDateTime)
                   .InitiateStartReview(assessedDateTime)
                   .AcknowledgeStartReview(assessedDateTime)
                   .InitiateApproval(assessedDateTime)
                   .AcknowledgeApproval(assessedDateTime)
                   .AcknowledgePublished(assessedDateTime);
            await businessFlow.Execute();

            var businessFlowForNextDay = new PriceEntryBusinessFlow(contentBlockNextDay, priceSeriesIds, HttpClient)
               .SaveContentBlockDefinition()
               .SavePriceSeriesItem(
                    TestSeries.Melamine_Asia_SE_Spot,
                    nextDay,
                    GetSeriesItemTypeCode(),
                    new SeriesItem { PriceLow = 30, PriceHigh = 40 })
               .InitiateSendForReview(nextDay)
               .AcknowledgeSendForReview(nextDay)
               .InitiateStartReview(nextDay)
               .AcknowledgeStartReview(nextDay)
               .InitiateApproval(nextDay)
               .AcknowledgeApproval(nextDay)
               .AcknowledgePublished(nextDay);
            await businessFlowForNextDay.Execute();

            var businessFlowCorrection = new PriceEntryBusinessFlow(ContentBlockId, priceSeriesIds, HttpClient)
               .InitiateCorrection(assessedDateTime)
               .SavePriceSeriesItem(
                    TestSeries.Melamine_Asia_SE_Spot,
                    assessedDateTime,
                    GetSeriesItemTypeCode(),
                    new SeriesItem { PriceLow = 20, PriceHigh = 100 },
                    "correction")
               .AdvancedCorrectionInitiateSendForReview(assessedDateTime)
               .AdvancedCorrectionAcknowledgeSendForReview(assessedDateTime)
               .AdvancedCorrectionInitiateStartReview(assessedDateTime)
               .AdvancedCorrectionAcknowledgeStartReview(assessedDateTime)
               .AdvancedCorrectionInitiateApproval(assessedDateTime)
               .AdvancedCorrectionAcknowledgeApproval(assessedDateTime)
               .AdvancedCorrectionAcknowledgePublished(assessedDateTime);
            await businessFlowCorrection.Execute();

            var contentBlockDeltaCorrectionForNextDate = await ContentBlockClient
                                                               .GetContentBlockWithPriceSeriesOnly(
                                                                    contentBlockNextDay,
                                                                    Tomorrow,
                                                                    isReviewMode: false)
                                                               .GetRawResponse();

            contentBlockDeltaCorrectionForNextDate.MatchSnapshotWithSeriesItemId(
               SnapshotNameExtension.Create("PriceRange"));
        }

        [Fact]
        public async Task Return_Non_Terminated_Series_Only()
        {
            var assessedDateTime = Today;
            var fourSeries = GetFourSeriesIds();
            fourSeries.Should().HaveCount(4);
            var priceSeriesService = GetService<PriceSeriesTestService>();
            await priceSeriesService.SetTerminationDate(fourSeries[0], assessedDateTime.AddDays(-1));
            await priceSeriesService.SetTerminationDate(fourSeries[1], assessedDateTime);
            await priceSeriesService.SetTerminationDate(fourSeries[2], assessedDateTime.AddDays(1));
            await priceSeriesService.SetTerminationDate(fourSeries[3], null);

            var priceSeriesIds = ContentBlockQueryHelper.CreatePriceSeriesIds(fourSeries);

            var businessFlow =
                new PriceEntryBusinessFlow(ContentBlockId, priceSeriesIds, HttpClient)
                   .SaveContentBlockDefinition();

            _ = await businessFlow.Execute();

            var result = await ContentBlockClient.GetContentBlockWithPriceSeriesOnly(ContentBlockId, assessedDateTime)
                            .GetRawResponse();

            foreach (var seriesId in fourSeries)
            {
                await priceSeriesService.SetTerminationDate(seriesId, null);
            }

            result.MatchSnapshot(SnapshotNameExtension.Create(SeriesItemTypeCode.Value));
        }

        [Fact]
        public async Task Return_Non_Terminated_Series_Only_When_Send_For_Review()
        {
            var assessedDateTime = Today;
            var fourSeries = GetFourSeriesIds();
            fourSeries.Should().HaveCount(4);
            var priceSeriesService = GetService<PriceSeriesTestService>();
            await priceSeriesService.SetTerminationDate(fourSeries[0], assessedDateTime.AddDays(-1));
            await priceSeriesService.SetTerminationDate(fourSeries[1], assessedDateTime);
            await priceSeriesService.SetTerminationDate(fourSeries[2], assessedDateTime.AddDays(1));
            await priceSeriesService.SetTerminationDate(fourSeries[3], null);

            var priceSeriesIds = ContentBlockQueryHelper.CreatePriceSeriesIds(fourSeries);

            var businessFlow =
                new PriceEntryBusinessFlow(ContentBlockId, priceSeriesIds, HttpClient)
                   .SaveContentBlockDefinition()
                   .SavePriceSeriesItem(
                        fourSeries[1],
                        assessedDateTime,
                        GetSeriesItemTypeCode(),
                        GetDefaultSeriesItem())
                   .SavePriceSeriesItem(
                        fourSeries[2],
                        assessedDateTime,
                        GetSeriesItemTypeCode(),
                        GetDefaultSeriesItem())
                   .SavePriceSeriesItem(
                        fourSeries[3],
                        assessedDateTime,
                        GetSeriesItemTypeCode(),
                        GetDefaultSeriesItem())
                   .InitiateSendForReview(assessedDateTime)
                   .AcknowledgeSendForReview(assessedDateTime);

            _ = await businessFlow.Execute();

            var result = await ContentBlockClient.GetContentBlockWithPriceSeriesOnly(ContentBlockId, assessedDateTime)
                            .GetRawResponse();

            foreach (var seriesId in fourSeries)
            {
                await priceSeriesService.SetTerminationDate(seriesId, null);
            }

            result.MatchSnapshotWithSeriesItemId(SnapshotNameExtension.Create(SeriesItemTypeCode.Value));
        }

        [Fact]
        public async Task Return_Validation_Errors_For_NonMarketAdjustment_Fields()
        {
            var firstSeriesItem = GetDefaultSeriesItem();
            firstSeriesItem.AdjustedPriceLowDelta = 10.0m;

            var secondSeriesItem = GetDefaultSeriesItem();
            secondSeriesItem.AdjustedPriceHighDelta = 20.0m;

            var businessFlow =
                new PriceEntryBusinessFlow(ContentBlockId, [PriceSeriesId, AlternativePriceSeriesId], HttpClient)
                   .SaveContentBlockDefinition()
                   .SaveRangePriceSeriesItem(PriceSeriesId, Today, firstSeriesItem)
                   .SaveRangePriceSeriesItem(AlternativePriceSeriesId, Today, secondSeriesItem);

            _ = await businessFlow.Execute();

            var result = await ContentBlockClient.GetContentBlockWithPriceSeriesValidationErrorsOnly(ContentBlockId, Today)
                            .GetRawResponse();

            result.MatchSnapshotWithSeriesItemId();
        }

        protected override SeriesItem BuildInvalidSeriesItem()
        {
            return new SeriesItem { PriceLow = 20, PriceHigh = 10 };
        }

        protected override SeriesItem BuildValidSeriesItem()
        {
            return new SeriesItem { PriceLow = 10, PriceHigh = 20 };
        }

        protected override SeriesItem GetDefaultSeriesItem()
        {
            return RangePriceSeriesItemBuilder.BuildDefaultValidSeriesItem();
        }

        protected override SeriesItemTypeCode GetSeriesItemTypeCode()
        {
            return SeriesItemTypeCode.RangeSeries;
        }

        protected override string[] GetFourSeriesIds()
        {
            // using Styrene series as we need 4 non-terminated series with the same publication schedule
            return [
                TestSeries.Styrene_Dagu_Spot_China_N,
                TestSeries.Styrene_Daqing_Spot_China_NE,
                TestSeries.Styrene_Guangzhou_Spot_China_S,
                TestSeries.Styrene_Fujian_Spot_China_S
            ];
        }

        protected override SeriesItem SetNonMarketAdjustmentValues(SeriesItem seriesItem)
        {
            seriesItem.AdjustedPriceLowDelta = 10.0m;
            seriesItem.AdjustedPriceHighDelta = 20.0m;

            return seriesItem;
        }
    }
}
