namespace Authoring.Tests.Application.ContentBlock.Query.ContentBlockWithCommentary
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Authoring.Tests.Functional.Tests.Common.Infrastructure;

    using global::BusinessLayer.PriceEntry.DTOs.Input;
    using global::BusinessLayer.PriceEntry.ValueObjects;

    using Snapshooter;
    using Snapshooter.Xunit;

    using Test.Infrastructure.Authoring;
    using Test.Infrastructure.Authoring.BusinessFlows.PriceEntryBusinessFlow;
    using Test.Infrastructure.Authoring.GraphQLClients.PriceEntry;
    using Test.Infrastructure.Extensions;
    using Test.Infrastructure.TestData;

    using Xunit;

    [Collection(WebApplicationCollection.Name)]
    public class ContentBlockQueryWithCommentaryShould : WebApplicationTestBase
    {
        private const string ContentBlockId = "contentBlockId";

        private static readonly DateTime AssessedDateTime = TestData.Now;

        private readonly ContentBlockGraphQLClient contentBlockClient;

        public ContentBlockQueryWithCommentaryShould(AuthoringBffApplicationFactory factory)
            : base(factory)
        {
            contentBlockClient = new ContentBlockGraphQLClient(GraphQLClient);
        }

        public static TheoryData<string, Func<PriceEntryBusinessFlow, DateTime, PriceEntryBusinessFlow>, string>
            Scenarios =>
            new()
            {
                {
                    "CORRECT_COMMENTARY",
                    (businessFlow, now) => businessFlow
                       .SaveContentBlockDefinition()
                       .SaveCommentary(now, "0.1")
                       .SavePriceSeriesItem(
                            TestSeries.Melamine_Asia_SE_Spot,
                            now,
                            SeriesItemTypeCode.RangeSeries,
                            new SeriesItem { PriceLow = 10, PriceHigh = 20 })
                       .InitiateSendForReview(now)
                       .AcknowledgeSendForReview(now)
                       .InitiateStartReview(now)
                       .AcknowledgeStartReview(now)
                       .InitiateApproval(now)
                       .AcknowledgeApproval(now)
                       .AcknowledgePublished(now)
                       .InitiateCorrection(now)
                       .SaveCommentary(now, "0.2", "correction"),
                    TestSeries.Melamine_Asia_SE_Spot
                },
                {
                    "CORRECT_PRICE",
                    (businessFlow, now) => businessFlow
                       .SaveContentBlockDefinition()
                       .SaveCommentary(now, "0.1")
                       .SavePriceSeriesItem(
                            TestSeries.Melamine_Asia_SE_Spot,
                            now,
                            SeriesItemTypeCode.RangeSeries,
                            new SeriesItem { PriceLow = 10, PriceHigh = 20 })
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
                            new SeriesItem { PriceLow = 10, PriceHigh = 30 },
                            "correction"),
                    TestSeries.Melamine_Asia_SE_Spot
                },
                {
                    "CORRECT_COMMENTARY_AND_PRICE",
                    (businessFlow, now) => businessFlow
                       .SaveContentBlockDefinition()
                       .SaveCommentary(now, "0.1")
                       .SavePriceSeriesItem(
                            TestSeries.Melamine_Asia_SE_Spot,
                            now,
                            SeriesItemTypeCode.RangeSeries,
                            new SeriesItem { PriceLow = 10, PriceHigh = 20 })
                       .InitiateSendForReview(now)
                       .AcknowledgeSendForReview(now)
                       .InitiateStartReview(now)
                       .AcknowledgeStartReview(now)
                       .InitiateApproval(now)
                       .AcknowledgeApproval(now)
                       .AcknowledgePublished(now)
                       .InitiateCorrection(now)
                       .SaveCommentary(now, "0.2", "correction")
                       .SavePriceSeriesItem(
                            TestSeries.Melamine_Asia_SE_Spot,
                            now,
                            SeriesItemTypeCode.RangeSeries,
                            new SeriesItem { PriceLow = 10, PriceHigh = 30 },
                            "correction"),
                    TestSeries.Melamine_Asia_SE_Spot
                },
                {
                    "CORRECT_AND_PUBLISH_COMMENTARY",
                    (businessFlow, now) => businessFlow
                       .SaveContentBlockDefinition()
                       .SaveCommentary(now, "0.1")
                       .SavePriceSeriesItem(
                            TestSeries.Melamine_Asia_SE_Spot,
                            now,
                            SeriesItemTypeCode.RangeSeries,
                            new SeriesItem { PriceLow = 10, PriceHigh = 20 })
                       .InitiateSendForReview(now)
                       .AcknowledgeSendForReview(now)
                       .InitiateStartReview(now)
                       .AcknowledgeStartReview(now)
                       .InitiateApproval(now)
                       .AcknowledgeApproval(now)
                       .AcknowledgePublished(now)
                       .InitiateCorrection(now)
                       .SaveCommentary(now, "0.2", "correction")
                       .AdvancedCorrectionInitiateSendForReview(now)
                       .AdvancedCorrectionAcknowledgeSendForReview(now)
                       .AdvancedCorrectionInitiateStartReview(now)
                       .AdvancedCorrectionAcknowledgeStartReview(now)
                       .AdvancedCorrectionInitiateApproval(now)
                       .AdvancedCorrectionAcknowledgeApproval(now)
                       .AdvancedCorrectionAcknowledgePublished(now),
                    TestSeries.Melamine_Asia_SE_Spot
                },
                {
                    "CORRECT_AND_PUBLISH_PRICE",
                    (businessFlow, now) => businessFlow
                       .SaveContentBlockDefinition()
                       .SaveCommentary(now, "0.1")
                       .SavePriceSeriesItem(
                            TestSeries.Melamine_Asia_SE_Spot,
                            now,
                            SeriesItemTypeCode.RangeSeries,
                            new SeriesItem { PriceLow = 10, PriceHigh = 20 })
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
                            new SeriesItem { PriceLow = 10, PriceHigh = 30 },
                            "correction")
                       .AdvancedCorrectionInitiateSendForReview(now)
                       .AdvancedCorrectionAcknowledgeSendForReview(now)
                       .AdvancedCorrectionInitiateStartReview(now)
                       .AdvancedCorrectionAcknowledgeStartReview(now)
                       .AdvancedCorrectionInitiateApproval(now)
                       .AdvancedCorrectionAcknowledgeApproval(now)
                       .AdvancedCorrectionAcknowledgePublished(now),
                    TestSeries.Melamine_Asia_SE_Spot
                },
                {
                    "CORRECT_AND_PUBLISH_COMMENTARY_AND_PRICE",
                    (businessFlow, now) => businessFlow
                       .SaveContentBlockDefinition()
                       .SaveCommentary(now, "0.1")
                       .SaveRangePriceSeriesItem(
                            TestSeries.Melamine_Asia_SE_Spot,
                            now,
                            new SeriesItem { PriceLow = 10, PriceHigh = 20 })
                       .InitiateSendForReview(now)
                       .AcknowledgeSendForReview(now)
                       .InitiateStartReview(now)
                       .AcknowledgeStartReview(now)
                       .InitiateApproval(now)
                       .AcknowledgeApproval(now)
                       .AcknowledgePublished(now)
                       .InitiateCorrection(now)
                       .SaveCommentary(now, "0.2", "correction")
                       .SavePriceSeriesItem(
                            TestSeries.Melamine_Asia_SE_Spot,
                            now,
                            SeriesItemTypeCode.RangeSeries,
                            new SeriesItem { PriceLow = 10, PriceHigh = 30 },
                            "correction")
                       .AdvancedCorrectionInitiateSendForReview(now)
                       .AdvancedCorrectionAcknowledgeSendForReview(now)
                       .AdvancedCorrectionInitiateStartReview(now)
                       .AdvancedCorrectionAcknowledgeStartReview(now)
                       .AdvancedCorrectionInitiateApproval(now)
                       .AdvancedCorrectionAcknowledgeApproval(now)
                       .AdvancedCorrectionAcknowledgePublished(now),
                    TestSeries.Melamine_Asia_SE_Spot
                },
                {
                    "CORRECT_AND_CANCEL_COMMENTARY",
                    (businessFlow, now) => businessFlow
                       .SaveContentBlockDefinition()
                       .SaveCommentary(now, "0.1")
                       .SavePriceSeriesItem(
                            TestSeries.Melamine_Asia_SE_Spot,
                            now,
                            SeriesItemTypeCode.RangeSeries,
                            new SeriesItem { PriceLow = 10, PriceHigh = 20 })
                       .InitiateSendForReview(now)
                       .AcknowledgeSendForReview(now)
                       .InitiateStartReview(now)
                       .AcknowledgeStartReview(now)
                       .InitiateApproval(now)
                       .AcknowledgeApproval(now)
                       .AcknowledgePublished(now)
                       .InitiateCorrection(now)
                       .SaveCommentary(now, "0.2", "correction")
                       .InitiateCancel(now)
                       .AcknowledgeCancelled(now),
                    TestSeries.Melamine_Asia_SE_Spot
                },
                {
                    "CORRECT_AND_CANCEL_PRICE",
                    (businessFlow, now) => businessFlow
                       .SaveContentBlockDefinition()
                       .SaveCommentary(now, "0.1")
                       .SavePriceSeriesItem(
                            TestSeries.Melamine_Asia_SE_Spot,
                            now,
                            SeriesItemTypeCode.RangeSeries,
                            new SeriesItem { PriceLow = 10, PriceHigh = 20 })
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
                            new SeriesItem { PriceLow = 10, PriceHigh = 30 },
                            "correction")
                       .InitiateCancel(now)
                       .AcknowledgeCancelled(now),
                    TestSeries.Melamine_Asia_SE_Spot
                },
                {
                    "CORRECT_AND_CANCEL_COMMENTARY_AND_PRICE",
                    (businessFlow, now) => businessFlow
                       .SaveContentBlockDefinition()
                       .SavePriceSeriesItem(
                            TestSeries.Melamine_Asia_SE_Spot,
                            now,
                            SeriesItemTypeCode.RangeSeries,
                            new SeriesItem { PriceLow = 10, PriceHigh = 20 })
                       .SaveCommentary(now, "0.1")
                       .InitiateSendForReview(now)
                       .AcknowledgeSendForReview(now)
                       .InitiateStartReview(now)
                       .AcknowledgeStartReview(now)
                       .InitiateApproval(now)
                       .AcknowledgeApproval(now)
                       .AcknowledgePublished(now)
                       .InitiateCorrection(now)
                       .SaveCommentary(now, "0.2", "correction")
                       .SavePriceSeriesItem(
                            TestSeries.Melamine_Asia_SE_Spot,
                            now,
                            SeriesItemTypeCode.RangeSeries,
                            new SeriesItem { PriceLow = 10, PriceHigh = 30 },
                            "correction")
                       .InitiateCancel(now)
                       .AcknowledgeCancelled(now),
                    TestSeries.Melamine_Asia_SE_Spot
                },
                {
                    "CORRECT_COMMENTARY_WHEN_EMPTY_INITIALLY",
                    (businessFlow, now) => businessFlow
                       .SaveContentBlockDefinition()
                       .SavePriceSeriesItem(
                            TestSeries.Melamine_Asia_SE_Spot,
                            now,
                            SeriesItemTypeCode.RangeSeries,
                            new SeriesItem { PriceLow = 10, PriceHigh = 20 })
                       .InitiateSendForReview(now)
                       .AcknowledgeSendForReview(now)
                       .InitiateStartReview(now)
                       .AcknowledgeStartReview(now)
                       .InitiateApproval(now)
                       .AcknowledgeApproval(now)
                       .AcknowledgePublished(now)
                       .InitiateCorrection(now)
                       .SaveCommentary(now, "0.1", "correction")
                       .SavePriceSeriesItem(
                            TestSeries.Melamine_Asia_SE_Spot,
                            now,
                            SeriesItemTypeCode.RangeSeries,
                            new SeriesItem { PriceLow = 10, PriceHigh = 30 },
                            "correction"),
                    TestSeries.Melamine_Asia_SE_Spot
                },
                {
                    "CORRECT_AND_PUBLISH_COMMENTARY_WHEN_EMPTY_INITIALLY",
                    (businessFlow, now) => businessFlow
                       .SaveContentBlockDefinition()
                       .SavePriceSeriesItem(
                            TestSeries.Melamine_Asia_SE_Spot,
                            now,
                            SeriesItemTypeCode.RangeSeries,
                            new SeriesItem { PriceLow = 10, PriceHigh = 20 })
                       .InitiateSendForReview(now)
                       .AcknowledgeSendForReview(now)
                       .InitiateStartReview(now)
                       .AcknowledgeStartReview(now)
                       .InitiateApproval(now)
                       .AcknowledgeApproval(now)
                       .AcknowledgePublished(now)
                       .InitiateCorrection(now)
                       .SaveCommentary(now, "0.1", "correction")
                       .AdvancedCorrectionInitiateSendForReview(now)
                       .AdvancedCorrectionAcknowledgeSendForReview(now)
                       .AdvancedCorrectionInitiateStartReview(now)
                       .AdvancedCorrectionAcknowledgeStartReview(now)
                       .AdvancedCorrectionInitiateApproval(now)
                       .AdvancedCorrectionAcknowledgeApproval(now)
                       .AdvancedCorrectionAcknowledgePublished(now),
                    TestSeries.Melamine_Asia_SE_Spot
                },
                {
                    "CORRECT_AND_CANCEL_COMMENTARY_WHEN_EMPTY_INITIALLY",
                    (businessFlow, now) => businessFlow
                       .SaveContentBlockDefinition()
                       .SavePriceSeriesItem(
                            TestSeries.Melamine_Asia_SE_Spot,
                            now,
                            SeriesItemTypeCode.RangeSeries,
                            new SeriesItem { PriceLow = 10, PriceHigh = 20 })
                       .InitiateSendForReview(now)
                       .AcknowledgeSendForReview(now)
                       .InitiateStartReview(now)
                       .AcknowledgeStartReview(now)
                       .InitiateApproval(now)
                       .AcknowledgeApproval(now)
                       .AcknowledgePublished(now)
                       .InitiateCorrection(now)
                       .SaveCommentary(now, "0.1", "correction")
                       .InitiateCancel(now)
                       .AcknowledgeCancelled(now),
                    TestSeries.Melamine_Asia_SE_Spot
                },
                {
                    "CORRECT_COMMENTARY_AFTER_COMMENTARY_CORRECTION",
                    (businessFlow, now) => businessFlow
                       .SaveContentBlockDefinition()
                       .SaveCommentary(now, "0.1")
                       .SavePriceSeriesItem(
                            TestSeries.Melamine_Asia_SE_Spot,
                            now,
                            SeriesItemTypeCode.RangeSeries,
                            new SeriesItem { PriceLow = 10, PriceHigh = 20 })
                       .InitiateSendForReview(now)
                       .AcknowledgeSendForReview(now)
                       .InitiateStartReview(now)
                       .AcknowledgeStartReview(now)
                       .InitiateApproval(now)
                       .AcknowledgeApproval(now)
                       .AcknowledgePublished(now)
                       .InitiateCorrection(now)
                       .SaveCommentary(now, "0.2", "correction")
                       .SaveCommentary(now, "0.3", "correction"),
                    TestSeries.Melamine_Asia_SE_Spot
                }
            };

        [Fact]
        public async Task Return_Null_Commentary_When_Commentary_Is_Not_Saved()
        {
            // Arrange
            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, new List<string> { TestSeries.LNG_China_HM1 }, HttpClient)
                                .SaveContentBlockDefinition();

            await businessFlow.Execute();

            // Act
            var result = await contentBlockClient.GetContentBlockWithCommentaryOnly(ContentBlockId, AssessedDateTime)
                              .GetRawResponse();

            // Assert
            result.MatchSnapshot();
        }

        [Fact]
        public async Task Return_Commentary_For_ContentBlock()
        {
            // Arrange
            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, new List<string> { TestSeries.LNG_China_HM1 }, HttpClient)
                                .SaveContentBlockDefinition()
                                .SaveCommentary(AssessedDateTime);

            await businessFlow.Execute();

            // Act
            var result = await contentBlockClient.GetContentBlockWithCommentaryOnly(ContentBlockId, AssessedDateTime)
                              .GetRawResponse();

            // Assert
            result.MatchSnapshot();
        }

        [Theory]
        [MemberData(nameof(Scenarios))]
        public async Task Return_Commentary_For_ContentBlock_For_Advanced_Workflow_Scenario(
            string scenario,
            Func<PriceEntryBusinessFlow, DateTime, PriceEntryBusinessFlow> configureScenario,
            string seriesId)
        {
            // Arrange
            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, new List<string> { seriesId }, HttpClient);

            // Act
            await configureScenario.Invoke(businessFlow, AssessedDateTime).Execute();

            // Assert
            var contentBlockWithCommentary = await contentBlockClient
                                                .GetContentBlockWithCommentaryOnly(ContentBlockId, AssessedDateTime)
                                                .GetRawResponse();

            contentBlockWithCommentary.MatchSnapshotWithSeriesItemId(SnapshotNameExtension.Create(scenario));
        }
    }
}