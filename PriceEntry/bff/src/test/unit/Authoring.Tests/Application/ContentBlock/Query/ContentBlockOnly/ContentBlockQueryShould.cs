namespace Authoring.Tests.Application.ContentBlock.Query.ContentBlockOnly
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Authoring.Tests.Functional.Tests.Common.Infrastructure;

    using global::BusinessLayer.ContentBlock.DTOs.Input;

    using Snapshooter.Xunit;

    using Test.Infrastructure.Authoring;
    using Test.Infrastructure.Authoring.BusinessFlows.PriceEntryBusinessFlow;
    using Test.Infrastructure.Authoring.BusinessFlows.PriceEntryBusinessFlow.PriceSeriesItemBuilders;
    using Test.Infrastructure.Authoring.GraphQLClients.PriceEntry;
    using Test.Infrastructure.Extensions;
    using Test.Infrastructure.TestData;

    using Xunit;

    [Collection(WebApplicationCollection.Name)]
    public class ContentBlockQueryShould : WebApplicationTestBase
    {
        private readonly ContentBlockGraphQLClient contentBlockClient;

        public ContentBlockQueryShould(AuthoringBffApplicationFactory factory)
            : base(factory)
        {
            contentBlockClient = new ContentBlockGraphQLClient(GraphQLClient);
        }

        [Fact]
        public async Task Return_A_Specific_Version_Of_Content_Block()
        {
            var contentBlockSaveRequest = new ContentBlockInput { ContentBlockId = "contentBlockId", Title = "title" };
            _ = await contentBlockClient.SaveContentBlock(contentBlockSaveRequest);

            contentBlockSaveRequest.Title = "new title";

            _ = await contentBlockClient.SaveContentBlock(contentBlockSaveRequest);

            var response = await contentBlockClient
                              .GetContentBlockOnly(contentBlockSaveRequest.ContentBlockId, 1, DateTime.UtcNow)
                              .GetRawResponse();

            response.MatchSnapshot();
        }

        [Fact]
        public async Task Return_Latest_Content_Block_When_Version_Is_Not_Specified()
        {
            var contentBlockSaveRequest = new ContentBlockInput { ContentBlockId = "contentBlockId", Title = "title" };
            _ = await contentBlockClient.SaveContentBlock(contentBlockSaveRequest);

            contentBlockSaveRequest.Title = "new title";

            _ = await contentBlockClient.SaveContentBlock(contentBlockSaveRequest);

            var response = await contentBlockClient
                              .GetContentBlockOnly(contentBlockSaveRequest.ContentBlockId, null, DateTime.UtcNow)
                              .GetRawResponse();

            response.MatchSnapshot();
        }

        [Fact]
        public async Task Return_Nothing_When_Content_Block_Does_Not_Exists()
        {
            var response = await contentBlockClient.GetContentBlockOnly("some-non-existing-id", null, DateTime.UtcNow).GetRawResponse();

            response.MatchSnapshot();
        }

        [Fact]
        public async Task Return_Full_Grid_Configuration_After_Price_Series_Are_Saved()
        {
            var contentBlockSaveRequest = new ContentBlockInput
            {
                ContentBlockId = "contentBlockId",
                Title = "LNG China HM 1 & 2",
                PriceSeriesGrids =
                [
                    new PriceSeriesGrid
                    {
                        Title = "Test Grid Title",
                        PriceSeriesIds = [TestSeries.LNG_China_HM1, TestSeries.LNG_China_HM2]
                    }
                ]
            };

            _ = await contentBlockClient.SaveContentBlock(contentBlockSaveRequest);

            var response = await contentBlockClient
                              .GetContentBlockWithGridConfigurationOnly(
                                   contentBlockSaveRequest.ContentBlockId,
                                   null,
                                   DateTime.UtcNow)
                              .GetRawResponse();

            response.MatchSnapshot();
        }

        [Fact]
        public async Task Return_Null_Grid_Configuration_When_Price_Series_Are_Not_Saved()
        {
            var contentBlockSaveRequest = new ContentBlockInput
            {
                ContentBlockId = "contentBlockId",
                Title = "LNG Britain"
            };

            _ = await contentBlockClient.SaveContentBlock(contentBlockSaveRequest);

            var response = await contentBlockClient
                              .GetContentBlockWithGridConfigurationOnly(
                                   contentBlockSaveRequest.ContentBlockId,
                                   null,
                                   DateTime.UtcNow)
                              .GetRawResponse();

            response.MatchSnapshot();
        }

        [Fact]
        public async Task Return_DataPackageId()
        {
            const string ContentBlockId = "contentBlockId";

            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, (ICollection<string>?)[], HttpClient)
               .SaveContentBlockDefinition();

            await businessFlow.Execute();

            var response = await contentBlockClient
                              .GetContentBlockWithDataPackageIdOnly(
                                   ContentBlockId,
                                   TestData.Now)
                              .GetRawResponse();

            response.MatchSnapshot(
                options => options
                   .IsTypeField<Guid>("data.contentBlock.dataPackageId")
                   .Assert(
                        option => Assert.DoesNotContain(
                            Guid.Empty,
                            option.Fields<Guid>("data.contentBlock.dataPackageId"))));
        }

        [Fact]
        public async Task Return_NmaEnabled_Field_Before_Starting_NonMarketAdjustment()
        {
            var businessFlow = new PriceEntryBusinessFlow("contentBlockId", (ICollection<string>?)[], HttpClient)
               .SaveContentBlockDefinition();

            _ = await businessFlow.Execute();

            var response = await contentBlockClient
                              .GetContentBlockWithNmaEnabledOnly("contentBlockId", TestData.Now)
                              .GetRawResponse();
            response.MatchSnapshot();
        }

        [Fact]
        public async Task Return_NmaEnabled_Field_After_Starting_NonMarketAdjustment()
        {
            var businessFlow = new PriceEntryBusinessFlow("contentBlockId", (ICollection<string>?)[], HttpClient)
               .SaveContentBlockDefinition()
               .StartNonMarketAdjustment(TestData.Now);

            _ = await businessFlow.Execute();

            var response = await contentBlockClient
                              .GetContentBlockWithNmaEnabledOnly("contentBlockId", TestData.Now)
                              .GetRawResponse();
            response.MatchSnapshot();
        }

        [Fact]
        public async Task Return_NmaEnabled_Field_After_Cancelling_NonMarketAdjustment()
        {
            var businessFlow = new PriceEntryBusinessFlow("contentBlockId", (ICollection<string>?)[], HttpClient)
               .SaveContentBlockDefinition()
               .StartNonMarketAdjustment(TestData.Now)
               .CancelNonMarketAdjustment(TestData.Now);

            _ = await businessFlow.Execute();

            var response = await contentBlockClient
                              .GetContentBlockWithNmaEnabledOnly("contentBlockId", TestData.Now)
                              .GetRawResponse();
            response.MatchSnapshot();
        }

        [Fact]
        public async Task Return_Grid_Configuration_With_Linked_Prices_Column_When_In_Correction_Styrene()
        {
            var assessedDateTime = DateTime.UtcNow;

            var businessFlow =
                new PriceEntryBusinessFlow(
                        "contentBlockId",
                        [TestSeries.Styrene_Dagu_Spot_China_N],
                        HttpClient)
                   .SaveContentBlockDefinition()
                   .SaveRangePriceSeriesItem(
                        TestSeries.Styrene_Dagu_Spot_China_N,
                        assessedDateTime,
                        RangePriceSeriesItemBuilder.BuildDefaultValidSeriesItem(20, 40))
                   .InitiateSendForReview(assessedDateTime)
                   .AcknowledgeSendForReview(assessedDateTime)
                   .InitiateStartReview(assessedDateTime)
                   .AcknowledgeStartReview(assessedDateTime)
                   .InitiateApproval(assessedDateTime)
                   .AcknowledgeApproval(assessedDateTime)
                   .AcknowledgePublished(assessedDateTime)
                   .InitiateCorrection(assessedDateTime)
                   .SaveRangePriceSeriesItem(
                        TestSeries.Styrene_Dagu_Spot_China_N,
                        assessedDateTime,
                        RangePriceSeriesItemBuilder.BuildDefaultValidSeriesItem(40, 60),
                        "correction");

            await businessFlow.Execute();

            var response = await contentBlockClient
                              .GetContentBlockWithGridConfigurationOnly(
                                   "contentBlockId",
                                   null,
                                   assessedDateTime)
                              .GetRawResponse();

            response.MatchSnapshot();
        }

        [Fact]
        public async Task Return_Grid_Configurations_With_Linked_Prices_Column_When_In_Correction_Styrene()
        {
            var assessedDateTime = DateTime.UtcNow;

            var businessFlow =
                new PriceEntryBusinessFlow(
                        "contentBlockId",
                        [[TestSeries.Styrene_Dagu_Spot_China_N]],
                        HttpClient)
                   .SaveContentBlockDefinition()
                   .SaveRangePriceSeriesItem(
                        TestSeries.Styrene_Dagu_Spot_China_N,
                        assessedDateTime,
                        RangePriceSeriesItemBuilder.BuildDefaultValidSeriesItem(20, 40))
                   .InitiateSendForReview(assessedDateTime)
                   .AcknowledgeSendForReview(assessedDateTime)
                   .InitiateStartReview(assessedDateTime)
                   .AcknowledgeStartReview(assessedDateTime)
                   .InitiateApproval(assessedDateTime)
                   .AcknowledgeApproval(assessedDateTime)
                   .AcknowledgePublished(assessedDateTime)
                   .InitiateCorrection(assessedDateTime)
                   .SaveRangePriceSeriesItem(
                        TestSeries.Styrene_Dagu_Spot_China_N,
                        assessedDateTime,
                        RangePriceSeriesItemBuilder.BuildDefaultValidSeriesItem(40, 60),
                        "correction");

            await businessFlow.Execute();

            var response = await contentBlockClient
                              .GetContentBlockWithGridConfigurationOnly(
                                   "contentBlockId",
                                   null,
                                   assessedDateTime)
                              .GetRawResponse();

            response.MatchSnapshot();
        }

        [Fact(Skip = "Skipping since data used works with advanced workflow, can be un-skipped after LNG moves to advanced workflow")]
        public async Task Return_Grid_Configuration_With_Linked_Prices_Column_When_In_Correction_LNG()
        {
            var assessedDateTime = DateTime.UtcNow;

            var businessFlow =
                new PriceEntryBusinessFlow(
                        "contentBlockId",
                        [TestSeries.LNG_China_HM1],
                        HttpClient)
                   .SaveContentBlockDefinition()
                   .SaveSingleValueWithReferencePriceSeriesItem(
                        TestSeries.LNG_China_HM1,
                        assessedDateTime,
                        SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice(20))
                   .InitiateSendForReview(assessedDateTime)
                   .AcknowledgeSendForReview(assessedDateTime)
                   .InitiateStartReview(assessedDateTime)
                   .AcknowledgeStartReview(assessedDateTime)
                   .InitiateApproval(assessedDateTime)
                   .AcknowledgeApproval(assessedDateTime)
                   .AcknowledgePublished(assessedDateTime)
                   .InitiateCorrection(assessedDateTime)
                   .SaveSingleValueWithReferencePriceSeriesItem(
                        TestSeries.LNG_China_HM1,
                        assessedDateTime,
                        SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice(40),
                        "correction");

            await businessFlow.Execute();

            var response = await contentBlockClient
                              .GetContentBlockWithGridConfigurationOnly(
                                   "contentBlockId",
                                   null,
                                   assessedDateTime)
                              .GetRawResponse();

            response.MatchSnapshot();
        }

        [Fact(Skip = "Skipping since data used works with advanced workflow, can be un-skipped after LNG moves to advanced workflow")]
        public async Task Return_Grid_Configurations_With_Linked_Prices_Column_When_In_Correction_LNG()
        {
            var assessedDateTime = DateTime.UtcNow;

            var businessFlow =
                new PriceEntryBusinessFlow(
                        "contentBlockId",
                        [[TestSeries.LNG_China_HM1]],
                        HttpClient)
                   .SaveContentBlockDefinition()
                   .SaveSingleValueWithReferencePriceSeriesItem(
                        TestSeries.LNG_China_HM1,
                        assessedDateTime,
                        SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice(20))
                   .InitiateSendForReview(assessedDateTime)
                   .AcknowledgeSendForReview(assessedDateTime)
                   .InitiateStartReview(assessedDateTime)
                   .AcknowledgeStartReview(assessedDateTime)
                   .InitiateApproval(assessedDateTime)
                   .AcknowledgeApproval(assessedDateTime)
                   .AcknowledgePublished(assessedDateTime)
                   .InitiateCorrection(assessedDateTime)
                   .SaveSingleValueWithReferencePriceSeriesItem(
                        TestSeries.LNG_China_HM1,
                        assessedDateTime,
                        SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice(40),
                        "correction");

            await businessFlow.Execute();

            var response = await contentBlockClient
                              .GetContentBlockWithGridConfigurationOnly(
                                   "contentBlockId",
                                   null,
                                   assessedDateTime)
                              .GetRawResponse();

            response.MatchSnapshot();
        }
    }
}