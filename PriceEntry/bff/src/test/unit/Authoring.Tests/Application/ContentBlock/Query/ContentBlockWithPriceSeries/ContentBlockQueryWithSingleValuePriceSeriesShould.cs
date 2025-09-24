namespace Authoring.Tests.Application.ContentBlock.Query.ContentBlockWithPriceSeries
{
    using System;
    using System.Text.Json;
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
    public class ContentBlockQueryWithSingleValuePriceSeriesShould : ContentBlockQueryWithPriceSeriesTestBase
    {
        public ContentBlockQueryWithSingleValuePriceSeriesShould(AuthoringBffApplicationFactory factory)
            : base(factory)
        {
        }

        protected override string PriceSeriesId => TestSeries.LNG_Japan_Import_M1;

        protected override string AlternativePriceSeriesId => TestSeries.LNG_Japan_Import_M2;

        protected override SeriesItemTypeCode SeriesItemTypeCode => SeriesItemTypeCode.SingleValueSeries;

        [Fact]
        public async Task InitiateDeltaCorrectionForNextDate()
        {
            var priceSeriesId = TestSeries.Petchem_8603620;
            var assessedDateTime = Today;
            var nextDay = Tomorrow;
            var contentBlockNextDay = "ContentBlockIdNextDay";
            var priceSeriesIds = ContentBlockQueryHelper.CreatePriceSeriesIds([TestSeries.Petchem_8603620]);

            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, priceSeriesIds, HttpClient)
               .SaveContentBlockDefinition()
               .SaveSingleValuePriceSeriesItem(priceSeriesId, assessedDateTime, new SeriesItem { Price = 100 })
               .InitiateSendForReview(assessedDateTime)
               .AcknowledgeSendForReview(assessedDateTime)
               .InitiateStartReview(assessedDateTime)
               .AcknowledgeStartReview(assessedDateTime)
               .InitiateApproval(assessedDateTime)
               .AcknowledgeApproval(assessedDateTime)
               .AcknowledgePublished(assessedDateTime);
            await businessFlow.Execute();

            var periodGeneratorServiceStub = GetService<PeriodGeneratorServiceStub>();
            periodGeneratorServiceStub.ReplaceAbsolutePeriods(
            [
                 new()
                {
                    Code = "M2402",
                    PeriodCode = "M",
                    Label = "Feb 2024",
                    FromDate = new DateOnly(2024, 02, 01),
                    UntilDate = new DateOnly(2024, 02, 29)
                }
            ]);

            var businessFlowForNextDay = new PriceEntryBusinessFlow(contentBlockNextDay, priceSeriesIds, HttpClient)
               .SaveContentBlockDefinition()
               .SaveSingleValuePriceSeriesItem(priceSeriesId, nextDay, new SeriesItem { Price = 150 })
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
               .SaveSingleValuePriceSeriesItem(
                    priceSeriesId,
                    assessedDateTime,
                    new SeriesItem { Price = 130 },
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
                                                                 nextDay,
                                                                 isReviewMode: false)
                                                            .GetRawResponse();

            contentBlockDeltaCorrectionForNextDate.MatchSnapshotWithSeriesItemId(
                SnapshotNameExtension.Create("SingleValue"));

            periodGeneratorServiceStub.ClearAbsolutePeriods();
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

            var priceSeriesIds = ContentBlockQueryHelper.CreatePriceSeriesIds(
                fourSeries,
                [
                    TestSeries.LNG_China_HM1, TestSeries.LNG_China_HM2,
                    TestSeries.LNG_China_HM3, TestSeries.LNG_China_HM4
                ],
                [
                    TestSeries.LNG_Japan_HM1, TestSeries.LNG_Japan_HM2,
                    TestSeries.LNG_Japan_HM3, TestSeries.LNG_Japan_HM4,
                ]);

            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, priceSeriesIds, HttpClient)
               .SaveContentBlockDefinition()
               .SaveSingleValueWithReferencePriceSeriesItem(
                    TestSeries.LNG_China_HM1,
                    assessedDateTime,
                    SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice())
               .SaveSingleValueWithReferencePriceSeriesItem(
                    TestSeries.LNG_China_HM2,
                    assessedDateTime,
                    SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice())
               .SaveSingleValueWithReferencePriceSeriesItem(
                    TestSeries.LNG_China_HM3,
                    assessedDateTime,
                    SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice())
               .SaveSingleValueWithReferencePriceSeriesItem(
                    TestSeries.LNG_China_HM4,
                    assessedDateTime,
                    SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice())
               .SaveSingleValueWithReferencePriceSeriesItem(
                    TestSeries.LNG_Japan_HM1,
                    assessedDateTime,
                    SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice())
               .SaveSingleValueWithReferencePriceSeriesItem(
                    TestSeries.LNG_Japan_HM2,
                    assessedDateTime,
                    SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice())
               .SaveSingleValueWithReferencePriceSeriesItem(
                    TestSeries.LNG_Japan_HM3,
                    assessedDateTime,
                    SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice())
               .SaveSingleValueWithReferencePriceSeriesItem(
                    TestSeries.LNG_Japan_HM4,
                    assessedDateTime,
                    SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice())
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
        public async Task Return_Latest_Last_Assessed_PriceSeries_For_Series_With_PeriodLabelTypeCode_Reference_Time()
        {
            var periodGeneratorServiceStub = GetService<PeriodGeneratorServiceStub>();

            try
            {
                // Arrange
                var priceSeriesId = TestSeries.Petchem_8603620;

                var assessedDate = Today;
                var lastPublishedAssessedDate = assessedDate.AddDays(-5);
                var secondLastPublishedAssessedDate = assessedDate.AddDays(-9);

                var priceSeriesIds = ContentBlockQueryHelper.CreatePriceSeriesIds([priceSeriesId]);

                // Act
                var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, priceSeriesIds, HttpClient)
                   .SaveContentBlockDefinition()
                   .SaveSingleValuePriceSeriesItem(priceSeriesId, secondLastPublishedAssessedDate, SingleValuePriceSeriesItemBuilder.BuildDefaultValidSeriesItem(50))
                   .PublishWithAdvanceWorkflow(secondLastPublishedAssessedDate);

                await businessFlow.Execute();

                periodGeneratorServiceStub.ReplaceAbsolutePeriods(
                [
                     new()
                {
                    Code = "M2402",
                    PeriodCode = "M",
                    Label = "Feb 2024",
                    FromDate = new DateOnly(2024, 02, 01),
                    UntilDate = new DateOnly(2024, 02, 29)
                }
                ]);

                await businessFlow.SaveSingleValuePriceSeriesItem(priceSeriesId, lastPublishedAssessedDate, SingleValuePriceSeriesItemBuilder.BuildDefaultValidSeriesItem(100))
                                  .PublishWithAdvanceWorkflow(lastPublishedAssessedDate)
                                  .Execute();

                periodGeneratorServiceStub.ReplaceAbsolutePeriods(
                [
                     new()
                {
                    Code = "M2403",
                    PeriodCode = "M",
                    Label = "Mar 2024",
                    FromDate = new DateOnly(2024, 03, 01),
                    UntilDate = new DateOnly(2024, 03, 31)
                }
                ]);

                businessFlow.SaveSingleValuePriceSeriesItem(priceSeriesId, assessedDate, SingleValuePriceSeriesItemBuilder.BuildDefaultValidSeriesItem(100));
                await businessFlow.Execute();

                var doc = await ContentBlockClient.GetContentBlockWithPriceSeriesOnly(
                                                            ContentBlockId,
                                                            assessedDate,
                                                            isReviewMode: false)
                                                          .GetResponseAsJsonDocument();

                // Assert
                var root = doc.RootElement
                              .GetProperty("data")
                              .GetProperty("contentBlock")
                              .GetProperty("priceSeriesGrids")[0]
                              .GetProperty("priceSeries")[0];

                var lastAssessmentDate = root.GetProperty("lastAssessmentDate").GetString();
                var lastAssessmentPrice = root.GetProperty("lastAssessmentPrice").GetString();

                lastAssessmentDate.Should().Be("05 Jan 2024");
                lastAssessmentPrice.Should().Be("100");
            }
            finally
            {
                periodGeneratorServiceStub.ClearAbsolutePeriods();
            }
        }

        [Fact]
        public async Task No_Period_Should_Be_Set_If_No_AbsolutePeriod_Was_Returned_By_PeriodGenerator()
        {
            var periodGeneratorServiceStub = GetService<PeriodGeneratorServiceStub>();

            try
            {
                // Arrange
                var assessedDatetime = Today.AddDays(4);
                var priceSeriesId = TestSeries.Petchem_8603620;

                periodGeneratorServiceStub.ClearAbsolutePeriods();
                periodGeneratorServiceStub.KeepAbsolutePeriodsEmpty = true;

                var priceSeriesIds = ContentBlockQueryHelper.CreatePriceSeriesIds([priceSeriesId]);

                // Act
                var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, priceSeriesIds, HttpClient)
                  .SaveContentBlockDefinition()
                  .SaveSingleValueWithReferencePriceSeriesItem(priceSeriesId, assessedDatetime, SingleValuePriceSeriesItemBuilder.BuildDefaultValidSeriesItem());
                await businessFlow.Execute();

                var doc = await ContentBlockClient.GetContentBlockWithPriceSeriesOnly(
                                                            ContentBlockId,
                                                            assessedDatetime,
                                                            isReviewMode: false)
                                                          .GetResponseAsJsonDocument();

                // Assert
                var root = doc.RootElement
                              .GetProperty("data")
                              .GetProperty("contentBlock")
                              .GetProperty("priceSeriesGrids")[0]
                              .GetProperty("priceSeries")[0];

                var period = root.GetProperty("period").GetString();

                period.Should().BeEmpty();
            }
            finally
            {
                periodGeneratorServiceStub.KeepAbsolutePeriodsEmpty = false;
            }
        }

        protected override SeriesItem BuildInvalidSeriesItem()
        {
            return new SeriesItem { Price = 0 };
        }

        protected override SeriesItem BuildValidSeriesItem()
        {
            return new SeriesItem { Price = 10 };
        }

        protected override SeriesItem GetDefaultSeriesItem()
        {
            return SingleValuePriceSeriesItemBuilder.BuildDefaultValidSeriesItem();
        }

        protected override SeriesItemTypeCode GetSeriesItemTypeCode()
        {
            return SeriesItemTypeCode.SingleValueSeries;
        }

        protected override string[] GetFourSeriesIds()
        {
            return new[]
            {
                TestSeries.LNG_Japan_MM1,
                TestSeries.LNG_Japan_MM2,
                TestSeries.LNG_China_MM1,
                TestSeries.LNG_China_MM2
            };
        }

        protected override SeriesItem SetNonMarketAdjustmentValues(SeriesItem seriesItem)
        {
            seriesItem.AdjustedPriceDelta = 10m;
            return seriesItem;
        }
    }
}