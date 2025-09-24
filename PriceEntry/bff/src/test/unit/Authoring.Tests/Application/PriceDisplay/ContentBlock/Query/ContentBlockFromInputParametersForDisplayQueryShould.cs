namespace Authoring.Tests.Application.PriceDisplay.ContentBlock.Query
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Authoring.Tests.Functional.Tests.Common.Infrastructure;

    using Test.Infrastructure.Authoring;
    using Test.Infrastructure.Authoring.BusinessFlows.PriceEntryBusinessFlow;
    using Test.Infrastructure.Authoring.BusinessFlows.PriceEntryBusinessFlow.PriceSeriesItemBuilders;
    using Test.Infrastructure.Authoring.GraphQLClients.PriceDisplay;
    using Test.Infrastructure.Extensions;
    using Test.Infrastructure.TestData;
    using Xunit;

    [Collection(WebApplicationCollection.Name)]
    public class ContentBlockFromInputParametersForDisplayQueryShould : WebApplicationTestBase
    {
        private static readonly DateTime AssessedDateTime = TestData.Now;

        private readonly ContentBlockForDisplayAuthoringGraphQLClient contentBlockClient;

        public ContentBlockFromInputParametersForDisplayQueryShould(AuthoringBffApplicationFactory factory)
            : base(factory)
        {
            contentBlockClient = new ContentBlockForDisplayAuthoringGraphQLClient(GraphQLClient);
        }

        [Fact]
        public async Task Return_Content_Block_With_Correct_Rows_When_Given_Input_Series_Ids_And_Date_Styrene()
        {
            // Arrange
            var savePriceBusinessFlow =
                new PriceEntryBusinessFlow("styreneContentBlockId", [TestSeries.Styrene_Dagu_Spot_China_N], HttpClient)
                   .SaveContentBlockDefinition()
                   .SaveRangePriceSeriesItem(
                        TestSeries.Styrene_Dagu_Spot_China_N,
                        AssessedDateTime,
                        RangePriceSeriesItemBuilder.BuildDefaultValidSeriesItem())
                   .InitiatePublish(AssessedDateTime)
                   .AcknowledgePublished(AssessedDateTime);

            await savePriceBusinessFlow.Execute();

            var correctionBusinessFlow =
                new PriceEntryBusinessFlow(
                        "styreneContentBlockId",
                        [TestSeries.Styrene_Dagu_Spot_China_N],
                        HttpClient)
                   .InitiateCorrection(AssessedDateTime)
                   .SaveRangePriceSeriesItem(
                        TestSeries.Styrene_Dagu_Spot_China_N,
                        AssessedDateTime,
                        RangePriceSeriesItemBuilder.BuildDefaultValidSeriesItem(20, 40),
                        "correction");

            await correctionBusinessFlow.Execute();

            // Assert
            var response = await contentBlockClient.GetContentBlockFromInputParametersForDisplay([TestSeries.Styrene_Dagu_Spot_China_N], AssessedDateTime).GetRawResponse();

            response.MatchSnapshotForDisplayWithInputsWithoutContentBlockIdOrLastModified();
        }

        [Fact(Skip = "Skipping since data used works with advanced workflow, can be un-skipped after LNG moves to advanced workflow")]
        public async Task Return_Content_Block_With_Correct_Rows_When_Given_Input_Series_Ids_And_Date_LNG()
        {
            // Arrange
            await PublishLngHalfMonthInputs("LngHalfMonthInputsContentBlockId", AssessedDateTime);

            await PublishLngDerivedContentBlock("LngFullMonthDerivedContentBlockId", AssessedDateTime, TestSeries.LngFullMonthDerivedSeriesIds);
            await PublishLngDerivedContentBlock("LngHalfMonthFortnightDerivedContentBlockId", AssessedDateTime, TestSeries.LngEaxHalfMonthDerivedFortnightSeriesIds);
            await PublishLngDerivedContentBlock("LngEaxFullMonthDerivedContentBlockId", AssessedDateTime, TestSeries.LngEaxFullMonthDerivedSeriesIds);

            var seriesIds = new List<string>()
            {
                TestSeries.LNG_China_MM1,
                TestSeries.LNG_EAX_Index_HM2,
                TestSeries.LNG_EAX_Index_MM1
            };

            var correctionBusinessFlow =
                new PriceEntryBusinessFlow(
                        "LngHalfMonthInputsContentBlockId",
                        TestSeries.LngHalfMonthInputSeriesIds,
                        HttpClient)
                   .InitiateCorrection(AssessedDateTime)
                   .SaveSingleValueWithReferencePriceSeriesItem(
                        TestSeries.LNG_China_HM1,
                        AssessedDateTime,
                        SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice(50),
                        "correction");

            await correctionBusinessFlow.Execute();

            // Assert
            var response = await contentBlockClient.GetContentBlockFromInputParametersForDisplay(seriesIds, AssessedDateTime).GetRawResponse();

            response.MatchSnapshotForDisplayWithInputsWithoutContentBlockIdOrLastModified();
        }

        private async Task PublishLngHalfMonthInputs(string inputsContentBlockId, DateTime assessedDateTime)
        {
            var publishHalfMonthInputsBusinessFlow =
                new PriceEntryBusinessFlow(
                        inputsContentBlockId,
                        TestSeries.LngHalfMonthInputSeriesIds,
                        HttpClient)
                   .SaveContentBlockDefinition()
                   .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_China_HM1, assessedDateTime, SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice(10))
                   .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_China_HM2, assessedDateTime, SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice(20))
                   .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_China_HM3, assessedDateTime, SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice(30))
                   .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_China_HM4, assessedDateTime, SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice(40))
                   .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_China_HM5, assessedDateTime, SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice(50))
                   .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_Japan_HM1, assessedDateTime, SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice(10))
                   .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_Japan_HM2, assessedDateTime, SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice(20))
                   .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_Japan_HM3, assessedDateTime, SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice(30))
                   .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_Japan_HM4, assessedDateTime, SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice(40))
                   .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_Japan_HM5, assessedDateTime, SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice(50))
                   .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_Taiwan_HM1, assessedDateTime, SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice(10))
                   .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_Taiwan_HM2, assessedDateTime, SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice(20))
                   .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_Taiwan_HM3, assessedDateTime, SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice(30))
                   .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_Taiwan_HM4, assessedDateTime, SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice(40))
                   .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_Taiwan_HM5, assessedDateTime, SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice(50))
                   .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_South_Korea_HM1, assessedDateTime, SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice(10))
                   .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_South_Korea_HM2, assessedDateTime, SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice(20))
                   .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_South_Korea_HM3, assessedDateTime, SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice(30))
                   .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_South_Korea_HM4, assessedDateTime, SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice(40))
                   .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_South_Korea_HM5, assessedDateTime, SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice(50))
                   .InitiateSendForReview(assessedDateTime)
                   .AcknowledgeSendForReview(assessedDateTime)
                   .InitiateStartReview(assessedDateTime)
                   .AcknowledgeStartReview(assessedDateTime)
                   .InitiateApproval(assessedDateTime)
                   .AcknowledgeApproval(assessedDateTime)
                   .AcknowledgePublished(assessedDateTime);

            await publishHalfMonthInputsBusinessFlow.Execute();
        }

        private async Task PublishLngDerivedContentBlock(string derivedContentBlockId, DateTime assessedDateTime, List<string> derivedPriceSeriesIds)
        {
            var setupDerivedContentBlockBusinessFlow = new PriceEntryBusinessFlow(
                    derivedContentBlockId,
                    derivedPriceSeriesIds,
                    HttpClient)
               .SaveContentBlockDefinition()
               .InitiateSendForReview(assessedDateTime)
               .AcknowledgeSendForReview(assessedDateTime)
               .InitiateStartReview(assessedDateTime)
               .AcknowledgeStartReview(assessedDateTime)
               .InitiateApproval(assessedDateTime)
               .AcknowledgeApproval(assessedDateTime)
               .AcknowledgePublished(assessedDateTime);

            await setupDerivedContentBlockBusinessFlow.Execute();
        }
    }
}
