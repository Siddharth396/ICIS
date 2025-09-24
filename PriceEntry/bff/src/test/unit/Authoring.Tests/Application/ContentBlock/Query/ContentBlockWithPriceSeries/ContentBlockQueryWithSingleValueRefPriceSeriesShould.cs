namespace Authoring.Tests.Application.ContentBlock.Query.ContentBlockWithPriceSeries
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Authoring.Tests.Functional.Tests.Common.Infrastructure;

    using FluentAssertions;

    using global::BusinessLayer.PriceEntry.DTOs.Input;
    using global::BusinessLayer.PriceEntry.ValueObjects;
    using global::Infrastructure.Services.PeriodGenerator;

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
    public class ContentBlockQueryWithSingleValueRefPriceSeriesShould : ContentBlockQueryWithPriceSeriesTestBase
    {
        private readonly DataPackageWorkflowServiceStub dataPackageWorkflowServiceStub;

        private readonly CanvasApiServiceStub canvasApiServiceStub;

        public ContentBlockQueryWithSingleValueRefPriceSeriesShould(AuthoringBffApplicationFactory factory)
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
                    "RE-PUBLISH",
                    (businessFlow, now) => businessFlow
                       .InitiateSendForReview(now)
                       .AcknowledgeSendForReview(now)
                       .InitiateStartReview(now)
                       .AcknowledgeStartReview(now)
                       .InitiateApproval(now)
                       .AcknowledgeApproval(now)
                       .AcknowledgePublished(now)
                       .InitiateCorrection(now),
                    TestSeries.LNG_China_HM1
                },
                {
                    "RE-PUBLISH SEND FOR REVIEW IN PROGRESS",
                    (businessFlow, now) => businessFlow
                       .InitiateSendForReview(now)
                       .AcknowledgeSendForReview(now)
                       .InitiateStartReview(now)
                       .AcknowledgeStartReview(now)
                       .InitiateApproval(now)
                       .AcknowledgeApproval(now)
                       .AcknowledgePublished(now)
                       .InitiateCorrection(now)
                       .InitiateSendForReview(now),
                    TestSeries.LNG_China_HM1
                },
            };

        protected override string PriceSeriesId => TestSeries.LNG_Dubai_MM1;

        protected override string AlternativePriceSeriesId => TestSeries.LNG_China_HM2;

        protected override SeriesItemTypeCode SeriesItemTypeCode => SeriesItemTypeCode.SingleValueWithReferenceSeries;

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
        public async Task Return_Non_Terminated_Series_Only_When_Published()
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
                   .SaveSingleValueWithReferencePriceSeriesItem(fourSeries[1], assessedDateTime, SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice())
                   .SaveSingleValueWithReferencePriceSeriesItem(fourSeries[2], assessedDateTime, SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice())
                   .SaveSingleValueWithReferencePriceSeriesItem(fourSeries[3], assessedDateTime, SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice())
                   .InitiatePublish(assessedDateTime)
                   .AcknowledgePublished(assessedDateTime);

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
        public async Task Return_Price_Series_With_Persisted_AssessmentMethod()
        {
            var assessedDateTime = Today;
            var priceSeriesIds = ContentBlockQueryHelper.CreatePriceSeriesIds([PriceSeriesId]);

            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, priceSeriesIds, HttpClient)
               .SaveContentBlockDefinition()
               .SaveSingleValueWithReferencePriceSeriesItem(PriceSeriesId, assessedDateTime, SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice())
               .InitiatePublish(assessedDateTime)
               .AcknowledgePublished(assessedDateTime);
            await businessFlow.Execute();

            var response = await ContentBlockClient
                              .GetContentBlockWithPriceSeriesOnly(ContentBlockId, Tomorrow)
                              .GetRawResponse();

            response.MatchSnapshot();
        }

        [Fact]
        public async Task Return_Price_Series_With_Persisted_DataUsed()
        {
            var assessedDateTime = Today;
            var priceSeriesIds = ContentBlockQueryHelper.CreatePriceSeriesIds([PriceSeriesId]);

            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, priceSeriesIds, HttpClient)
               .SaveContentBlockDefinition()
               .SaveSingleValueWithReferencePriceSeriesItem(PriceSeriesId, assessedDateTime, SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice())
               .InitiatePublish(assessedDateTime)
               .AcknowledgePublished(assessedDateTime);

            await businessFlow.Execute();

            var response = await ContentBlockClient
                              .GetContentBlockWithPriceSeriesOnly(ContentBlockId, Tomorrow)
                              .GetRawResponse();

            response.MatchSnapshot();
        }

        [Fact]
        public async Task Return_Price_Series_With_Persisted_ReferencePrice_LNG_DES_Argentina()
        {
            var referenceMarket = "LNG DES Argentina";
            var assessedDateTime = Today;
            var referencePriceSeriesIds = ContentBlockQueryHelper.CreatePriceSeriesIds([TestSeries.LNG_Argentina_MM1]);
            var priceSeriesIds = ContentBlockQueryHelper.CreatePriceSeriesIds([PriceSeriesId]);

            var referencePriceBusinessFlow = new PriceEntryBusinessFlow("referencePriceContentBlockId", referencePriceSeriesIds, HttpClient)
               .SaveContentBlockDefinition()
               .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_Argentina_MM1, assessedDateTime, SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice())
               .InitiatePublish(assessedDateTime)
               .AcknowledgePublished(assessedDateTime);
            await referencePriceBusinessFlow.Execute();

            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, priceSeriesIds, HttpClient)
               .SaveContentBlockDefinition()
               .SaveSingleValueWithReferencePriceSeriesItem(PriceSeriesId, assessedDateTime, SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithPremiumDiscount(referenceMarket))
               .InitiatePublish(assessedDateTime)
               .AcknowledgePublished(assessedDateTime);
            await businessFlow.Execute();

            var response = await ContentBlockClient
                              .GetContentBlockWithPriceSeriesOnly(ContentBlockId, Tomorrow)
                              .GetRawResponse();

            response.MatchSnapshot();
        }

        [Fact]
        public async Task Return_Price_Series_With_Persisted_ReferencePrice_TTF()
        {
            var referenceMarket = "TTF";
            var assessedDateTime = Today;
            var priceSeriesIds = ContentBlockQueryHelper.CreatePriceSeriesIds([PriceSeriesId]);

            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, priceSeriesIds, HttpClient)
               .SaveContentBlockDefinition()
               .SaveSingleValueWithReferencePriceSeriesItem(PriceSeriesId, assessedDateTime, SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithPremiumDiscount(referenceMarket))
               .InitiatePublish(assessedDateTime)
               .AcknowledgePublished(assessedDateTime);
            await businessFlow.Execute();

            var response = await ContentBlockClient
                              .GetContentBlockWithPriceSeriesOnly(ContentBlockId, Tomorrow)
                              .GetRawResponse();

            response.MatchSnapshot();
        }

        [Fact]
        public async Task Return_Price_Series_With_Persisted_ReferencePrice_LNG_FOB_RELOAD_NWE()
        {
            var assessedDateTime = new DateTime(2025, 05, 14, 0, 0, 0, DateTimeKind.Utc);
            var assessedDateTimeNextDay = assessedDateTime.AddDays(1);

            var periodGeneratorServiceStub = GetService<PeriodGeneratorServiceStub>();
            periodGeneratorServiceStub.ReplaceAbsolutePeriods(new List<AbsolutePeriod>
            {
                new AbsolutePeriod
                     {
                        Code = "M2405",
                        PeriodCode = "M1",
                        Label = "May 2025",
                        FromDate = new DateOnly(2025, 05, 01),
                        UntilDate = new DateOnly(2025, 05, 31)
                     },
            });

            var assessedPricesId = ContentBlockQueryHelper.CreatePriceSeriesIds([TestSeries.LNG_Reload_Spain_20_40D]);
            var referencePricesId = ContentBlockQueryHelper.CreatePriceSeriesIds([TestSeries.LNG_Reload_NWE_20_40D]);

            var referencePriceBusinessFlow = new PriceEntryBusinessFlow("referencePricesContentBlockId", referencePricesId, HttpClient)
               .SaveContentBlockDefinition()
               .SaveSingleValueWithReferencePriceSeriesItem(
                            TestSeries.LNG_Reload_NWE_20_40D,
                            assessedDateTime,
                            SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice(11))
               .PublishWithAdvanceWorkflow(assessedDateTime);

            await referencePriceBusinessFlow.Execute();

            var spainFobBusinessFlow = new PriceEntryBusinessFlow(ContentBlockId, assessedPricesId, HttpClient)
                .SaveContentBlockDefinition()
                .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_Reload_Spain_20_40D, assessedDateTime, SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithPremiumDiscount("LNG FOB Reload NWE"))
                .PublishWithAdvanceWorkflow(assessedDateTime);

            await spainFobBusinessFlow.Execute();

            var response = await ContentBlockClient
                              .GetContentBlockWithPriceSeriesOnly(ContentBlockId, assessedDateTimeNextDay)
                              .GetRawResponse();

            periodGeneratorServiceStub.ClearAbsolutePeriods();

            response.MatchSnapshotWithoutSeriesItemId();
        }

        [Fact]
        public async Task Return_ValidationErrors_With_Persisted_AssessmentMethod_When_Status_Is_Not_ReadyToStart()
        {
            var assessedDateTime = Today;
            var priceSeriesIds = ContentBlockQueryHelper.CreatePriceSeriesIds([PriceSeriesId]);

            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, priceSeriesIds, HttpClient)
               .SaveContentBlockDefinition()
               .SaveSingleValueWithReferencePriceSeriesItem(PriceSeriesId, assessedDateTime, BuildInvalidSeriesItem());
            await businessFlow.Execute();

            var response = await ContentBlockClient
                              .GetContentBlockWithPriceSeriesValidationErrorsOnly(ContentBlockId, assessedDateTime)
                              .GetRawResponse();

            response.MatchSnapshot();
        }

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
                                                                    assessedDateTime,
                                                                    isReviewMode: false)
                                                               .GetRawResponse();

            contentBlockWithPriceSeriesResponseEditor.MatchSnapshotWithSeriesItemId(
                SnapshotNameExtension.Create(scenario, "editor"));

            var contentBlockWithPriceSeriesResponseCopyEditor = await ContentBlockClient
                                                                   .GetContentBlockWithPriceSeriesOnly(
                                                                        ContentBlockId,
                                                                        assessedDateTime,
                                                                        isReviewMode: true)
                                                                   .GetRawResponse();

            contentBlockWithPriceSeriesResponseCopyEditor.MatchSnapshotWithSeriesItemId(
                SnapshotNameExtension.Create(scenario, "copy_editor"));

            dataPackageWorkflowServiceStub.Requests.MatchSnapshotForWorkflowRequests(
                SnapshotNameExtension.Create(scenario, "Workflow_Requests"));

            canvasApiServiceStub.Requests.MatchSnapshotForCanvasApiRequests(SnapshotNameExtension.Create(scenario, "Canvas_API_Requests"));
        }

        [Fact]
        public async Task Get_Last_Assessed_PremiumDiscount()
        {
            // arrange
            var yesterday = TestData.Now.AddDays(-1);

            var seriesItem = new SeriesItem
            {
                AssessmentMethod = AssessmentMethod.PremiumDiscount.Value,
                ReferenceMarketName = "TTF",
                PremiumDiscount = 10,
                DataUsed = "Bid/offer"
            };

            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, [PriceSeriesId], HttpClient)
                .SaveContentBlockDefinition()
                .SaveSingleValueWithReferencePriceSeriesItem(PriceSeriesId, yesterday, seriesItem)
                .InitiatePublish(yesterday)
                .AcknowledgePublished(yesterday);
            await businessFlow.Execute();

            // act
            var today = yesterday.AddDays(1);
            seriesItem.PremiumDiscount = 20;

            businessFlow.SaveSingleValueWithReferencePriceSeriesItem(PriceSeriesId, today, seriesItem);
            await businessFlow.Execute();

            // assert
            var response = await ContentBlockClient
                              .GetContentBlockWithPriceSeriesOnly(ContentBlockId, today)
                              .GetRawResponse();

            response.MatchSnapshotWithSeriesItemId();
        }

        [Fact(Skip = "Can be un-skipped after LNG moves to advanced workflow")]
        public async Task Return_Price_Series_With_Correction_Published_Status_When_Having_Multiple_Grids_In_Content_Block()
        {
            string[] assessedPriceSeries = [
                    TestSeries.LNG_China_HM1, TestSeries.LNG_China_HM2, TestSeries.LNG_China_HM3,
                    TestSeries.LNG_China_HM4, TestSeries.LNG_China_HM5
               ];

            string[] derivedPriceSeries = [TestSeries.LNG_China_MM1, TestSeries.LNG_China_MM2];

            var priceSeriesIds = ContentBlockQueryHelper.CreatePriceSeriesIds(assessedPriceSeries, derivedPriceSeries);

            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, priceSeriesIds, HttpClient)
             .SaveContentBlockDefinition();

            foreach (var priceSeries in assessedPriceSeries)
            {
                businessFlow.SaveSingleValueWithReferencePriceSeriesItem(priceSeries, Today, SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice(100));
            }

            businessFlow.PublishWithAdvanceWorkflow(Today);

            foreach (var priceSeries in assessedPriceSeries)
            {
                businessFlow.SaveSingleValueWithReferencePriceSeriesItem(priceSeries, Tomorrow, SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice(200));
            }

            businessFlow.PublishWithAdvanceWorkflow(Tomorrow);

            businessFlow.InitiateCorrection(Today)
                        .SaveSingleValueWithReferencePriceSeriesItem(
                                TestSeries.LNG_China_HM1,
                                Today,
                                SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice(150),
                                "correction")
                        .SaveSingleValueWithReferencePriceSeriesItem(
                                TestSeries.LNG_China_HM4,
                                Today,
                                SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice(200),
                                "correction")
                        .PublishWithAdvanceCorrectionWorkflow(Today);

            await businessFlow.Execute();

            var response = await ContentBlockClient
                             .GetContentBlockWithPriceSeriesOnly(ContentBlockId, Today)
                             .GetRawResponse();

            response.MatchSnapshotWithSeriesItemId();
        }

        protected override SeriesItem BuildInvalidSeriesItem()
        {
            return SingleValueWithReferencePriceSeriesItemBuilder.BuildInvalidSeriesItem();
        }

        protected override SeriesItem BuildValidSeriesItem()
        {
            return SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice();
        }

        protected override SeriesItem GetDefaultSeriesItem()
        {
            return SingleValueWithReferencePriceSeriesItemBuilder.BuildDefaultValidSeriesItem();
        }

        protected override SeriesItemTypeCode GetSeriesItemTypeCode()
        {
            return SeriesItemTypeCode.SingleValueWithReferenceSeries;
        }

        protected override string[] GetFourSeriesIds()
        {
            return new[]
            {
                TestSeries.LNG_China_HM1,
                TestSeries.LNG_China_HM2,
                TestSeries.LNG_China_HM3,
                TestSeries.LNG_China_HM4
            };
        }

        protected override SeriesItem SetNonMarketAdjustmentValues(SeriesItem seriesItem)
        {
            seriesItem.AdjustedPriceDelta = 10.1m;
            return seriesItem;
        }
    }
}