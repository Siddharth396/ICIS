namespace Authoring.Tests.Application.PriceDisplay.ContentBlock.Query.ContentBlockForDisplayWithPriceSeries
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Authoring.Tests.Functional.Tests.Common.Infrastructure;

    using global::BusinessLayer.Helpers;
    using global::BusinessLayer.PriceDisplay.ContentBlock.DTOs.Input;
    using global::BusinessLayer.PriceEntry.DTOs.Input;
    using global::BusinessLayer.PriceEntry.ValueObjects;

    using Microsoft.Extensions.DependencyInjection;

    using Snapshooter.Xunit;

    using Test.Infrastructure.Authoring;
    using Test.Infrastructure.Authoring.BusinessFlows.PriceEntryBusinessFlow;
    using Test.Infrastructure.Authoring.BusinessFlows.PriceEntryBusinessFlow.PriceSeriesItemBuilders;
    using Test.Infrastructure.Authoring.GraphQLClients.PriceDisplay;
    using Test.Infrastructure.Extensions;
    using Test.Infrastructure.Stubs;
    using Test.Infrastructure.TestData;

    using Xunit;

    [Collection(WebApplicationCollection.Name)]
    public class ContentBlockQueryForDisplayWithPriceSeriesShould : WebApplicationTestBase
    {
        protected const string ContentBlockId = "contentBlockId";

        protected static readonly long Today = TestData.Now.ToUnixTimeMilliseconds();

        private readonly ContentBlockForDisplayAuthoringGraphQLClient contentBlockClient;
        private readonly TestClock testClock;

        public ContentBlockQueryForDisplayWithPriceSeriesShould(AuthoringBffApplicationFactory factory)
            : base(factory)
        {
            contentBlockClient = new ContentBlockForDisplayAuthoringGraphQLClient(GraphQLClient);
            testClock = factory.Services.GetRequiredService<TestClock>();
        }

        [Fact]
        public async Task Return_Multiple_Price_Series_Details_When_Multiple_Price_Series_Is_Saved_With_Price_Display_Configuration()
        {
            // Arrange
            var assessedDateTime = Today.FromUnixTimeMilliseconds();

            var contentBlockSaveRequest = new ContentBlockForDisplayInput
            {
                ContentBlockId = ContentBlockId,
                Title = "title",
                Columns =
                 [
                     new() { Field = "field", DisplayOrder = 1, Hidden = false }
                 ],
                Rows =
                 [
                    new() { PriceSeriesId = TestSeries.Melamine_Asia_SE_Spot, DisplayOrder = 0, SeriesItemTypeCode = SeriesItemTypeCode.Range },
                    new() { PriceSeriesId = TestSeries.Melamine_China_Spot_Cif_2_6_Weeks, DisplayOrder = 1, SeriesItemTypeCode = SeriesItemTypeCode.Range },
                ]
            };

            _ = await contentBlockClient.SaveContentBlockForDisplay(contentBlockSaveRequest);

            var businessFlow = new PriceEntryBusinessFlow(
                ContentBlockId,
                new List<string> { TestSeries.Melamine_Asia_SE_Spot, TestSeries.Melamine_China_Spot_Cif_2_6_Weeks },
                HttpClient)
               .SaveContentBlockDefinition()
               .SaveRangePriceSeriesItem(TestSeries.Melamine_China_Spot_Cif_2_6_Weeks, assessedDateTime, RangePriceSeriesItemBuilder.BuildDefaultValidSeriesItem())
               .SaveRangePriceSeriesItem(TestSeries.Melamine_Asia_SE_Spot, assessedDateTime, RangePriceSeriesItemBuilder.BuildDefaultValidSeriesItem())
               .PublishWithAdvanceWorkflow(assessedDateTime);
            await businessFlow.Execute();

            // Act
            var response = await contentBlockClient.GetContentBlockForDisplayWithPriceSeriesOnly(ContentBlockId).GetRawResponse();

            // Assert
            response.MatchSnapshotWithLastModifiedDate();
        }

        [Fact]

        public async Task Return_Price_Series_Details_When_Multiple_Price_Series_Saved_With_Price_Display_Configuration_Has_Only_One_Series_Item()
        {
            // Arrange
            var assessedDateTime = Today.FromUnixTimeMilliseconds();

            var contentBlockSaveRequest = new ContentBlockForDisplayInput
            {
                ContentBlockId = ContentBlockId,
                Title = "title",
                Columns =
                [
                    new() { Field = "field", DisplayOrder = 1, Hidden = false }
                ],
                Rows =
                [
                    new() { PriceSeriesId = TestSeries.Melamine_Asia_SE_Spot, DisplayOrder = 1, SeriesItemTypeCode = SeriesItemTypeCode.Range },
                    new() { PriceSeriesId = TestSeries.Melamine_China_Spot_Cif_2_6_Weeks, DisplayOrder = 0, SeriesItemTypeCode = SeriesItemTypeCode.Range },

                ]
            };

            _ = await contentBlockClient.SaveContentBlockForDisplay(contentBlockSaveRequest);

            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, new List<string> { TestSeries.Melamine_Asia_SE_Spot }, HttpClient)
               .SaveContentBlockDefinition()
               .SaveRangePriceSeriesItem(TestSeries.Melamine_Asia_SE_Spot, assessedDateTime, RangePriceSeriesItemBuilder.BuildDefaultValidSeriesItem())
               .PublishWithAdvanceWorkflow(assessedDateTime);
            await businessFlow.Execute();

            var businessFlow2 = new PriceEntryBusinessFlow("ContentBlockId2", new List<string> { TestSeries.Melamine_China_Spot_Cif_2_6_Weeks }, HttpClient)
              .SaveContentBlockDefinition();
            await businessFlow2.Execute();

            // Act
            var response = await contentBlockClient.GetContentBlockForDisplayWithPriceSeriesOnly("contentBlockId").GetRawResponse();

            // Assert
            response.MatchSnapshotWithLastModifiedDate();
        }

        [Fact]
        public async Task Return_Single_Price_Series_Details_When_Single_Price_Series_Is_Saved_With_Price_Display_Configuration()
        {
            // Arrange
            var assessedDateTime = Today.FromUnixTimeMilliseconds();

            var contentBlockSaveRequest = new ContentBlockForDisplayInput
            {
                ContentBlockId = ContentBlockId,
                Title = "title",
                Columns =
                [
                    new() { Field = "field", DisplayOrder = 1, Hidden = false }
                ],
                Rows =
                [
                    new() { PriceSeriesId = TestSeries.Melamine_Asia_SE_Spot, DisplayOrder = 1, SeriesItemTypeCode = SeriesItemTypeCode.Range }
                ]
            };

            _ = await contentBlockClient.SaveContentBlockForDisplay(contentBlockSaveRequest);

            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, new List<string> { TestSeries.Melamine_Asia_SE_Spot }, HttpClient)
               .SaveContentBlockDefinition()
               .SaveRangePriceSeriesItem(TestSeries.Melamine_Asia_SE_Spot, assessedDateTime, RangePriceSeriesItemBuilder.BuildDefaultValidSeriesItem())
               .PublishWithAdvanceWorkflow(assessedDateTime);
            await businessFlow.Execute();

            // Act
            var response = await contentBlockClient.GetContentBlockForDisplayWithPriceSeriesOnly("contentBlockId").GetRawResponse();

            // Assert
            response.MatchSnapshotWithLastModifiedDate();
        }

        [Fact]
        public async Task Return_Empty_Response_When_PriceSeries_Saved_With_Price_Display_Configuration_Does_Not_Exist()
        {
            // Arrange
            var contentBlockInput = new ContentBlockForDisplayInput
            {
                ContentBlockId = "contentBlockId",
                Title = "title",
                Columns =
                 [
                     new() { Field = "field", DisplayOrder = 1, Hidden = false }
                 ],
                Rows =
                [
                    new() { PriceSeriesId = "notexistPriceSeriesIds", DisplayOrder = 1, SeriesItemTypeCode = SeriesItemTypeCode.Range },
                    new() { PriceSeriesId = "notexistPriceSeriesIds2", DisplayOrder = 1, SeriesItemTypeCode = SeriesItemTypeCode.Range }
                ]
            };

            _ = await contentBlockClient.SaveContentBlockForDisplay(contentBlockInput);

            // Act
            var response = await contentBlockClient.GetContentBlockForDisplayWithPriceSeriesOnly(contentBlockInput.ContentBlockId).GetRawResponse();

            // Assert
            response.MatchSnapshot();
        }

        [Fact]
        public async Task Return_Price_Series_Details_In_Custom_Order()
        {
            // Arrange
            var assessedDateTime = Today.FromUnixTimeMilliseconds();

            var contentBlockSaveRequest = new ContentBlockForDisplayInput
            {
                ContentBlockId = ContentBlockId,
                Title = "title",
                Columns =
                 [
                     new() { Field = "field", DisplayOrder = 1, Hidden = false }
                 ],
                Rows =
                 [
                   new() { PriceSeriesId = TestSeries.Melamine_Asia_SE_Spot, DisplayOrder = 1, SeriesItemTypeCode = SeriesItemTypeCode.Range },
                   new() { PriceSeriesId = TestSeries.Melamine_China_Spot_Cif_2_6_Weeks, DisplayOrder = 0, SeriesItemTypeCode = SeriesItemTypeCode.Range },
                ]
            };
            _ = await contentBlockClient.SaveContentBlockForDisplay(contentBlockSaveRequest);

            var businessFlow = new PriceEntryBusinessFlow(
                ContentBlockId,
                new List<string> { TestSeries.Melamine_Asia_SE_Spot, TestSeries.Melamine_China_Spot_Cif_2_6_Weeks },
                HttpClient)
               .SaveContentBlockDefinition()
               .SaveRangePriceSeriesItem(TestSeries.Melamine_China_Spot_Cif_2_6_Weeks, assessedDateTime, RangePriceSeriesItemBuilder.BuildDefaultValidSeriesItem())
               .SaveRangePriceSeriesItem(TestSeries.Melamine_Asia_SE_Spot, assessedDateTime, RangePriceSeriesItemBuilder.BuildDefaultValidSeriesItem())
               .PublishWithAdvanceWorkflow(assessedDateTime);
            await businessFlow.Execute();

            // Act
            var response = await contentBlockClient.GetContentBlockForDisplayWithPriceSeriesOnly(ContentBlockId).GetRawResponse();

            // Assert
            response.MatchSnapshotWithLastModifiedDate();
        }

        [Fact]
        public async Task Return_Price_Series_Details_With_Published_Status()
        {
            // Arrange
            var assessedDateTime = Today.FromUnixTimeMilliseconds();

            var contentBlockSaveRequest = new ContentBlockForDisplayInput
            {
                ContentBlockId = ContentBlockId,
                Title = "title",
                Columns =
                 [
                    new() { Field = "field", DisplayOrder = 1, Hidden = false }
                 ],
                Rows =
                 [

                    new() { PriceSeriesId = TestSeries.Melamine_China_Spot_Cif_2_6_Weeks, DisplayOrder = 0, SeriesItemTypeCode = SeriesItemTypeCode.Range },

                ]
            };
            _ = await contentBlockClient.SaveContentBlockForDisplay(contentBlockSaveRequest);

            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, new List<string> { TestSeries.Melamine_China_Spot_Cif_2_6_Weeks }, HttpClient)
                 .SaveContentBlockDefinition()
                 .SaveRangePriceSeriesItem(TestSeries.Melamine_China_Spot_Cif_2_6_Weeks, assessedDateTime, RangePriceSeriesItemBuilder.BuildDefaultValidSeriesItem())
                 .PublishWithAdvanceWorkflow(assessedDateTime);

            await businessFlow.Execute();

            // Act
            var response = await contentBlockClient.GetContentBlockForDisplayWithPriceSeriesOnly(ContentBlockId).GetRawResponse();

            // Assert
            response.MatchSnapshotWithLastModifiedDate();
        }

        [Fact]
        public async Task Return_Null_For_Price_Series_Details_When_Multiple_PriceSeries_Saved_With_Price_Display_Configuration_Has_No_Price_Series_Items()
        {
            // Arrange
            var contentBlockSaveRequest = new ContentBlockForDisplayInput
            {
                ContentBlockId = ContentBlockId,
                Title = "title",
                Columns =
                 [
                     new() { Field = "field", DisplayOrder = 1, Hidden = false }
                 ],
                Rows =
                 [
                    new() { PriceSeriesId = TestSeries.Melamine_China_Spot_Cif_2_6_Weeks, DisplayOrder = 0, SeriesItemTypeCode = SeriesItemTypeCode.Range },
                    new() { PriceSeriesId = TestSeries.Melamine_Europe_Spot_CIF, DisplayOrder = 1, SeriesItemTypeCode = SeriesItemTypeCode.Range },
                    new() { PriceSeriesId = TestSeries.Melamine_Asia_NE_Spot, DisplayOrder = 2, SeriesItemTypeCode = SeriesItemTypeCode.Range },
                ]
            };
            _ = await contentBlockClient.SaveContentBlockForDisplay(contentBlockSaveRequest);

            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, new List<string> { TestSeries.Melamine_China_Spot_Cif_2_6_Weeks }, HttpClient)
                 .SaveContentBlockDefinition();
            await businessFlow.Execute();

            var businessFlow2 = new PriceEntryBusinessFlow(ContentBlockId, new List<string> { TestSeries.Melamine_Europe_Spot_CIF }, HttpClient)
              .SaveContentBlockDefinition();
            await businessFlow2.Execute();

            var businessFlow3 = new PriceEntryBusinessFlow(ContentBlockId, new List<string> { TestSeries.Melamine_Asia_NE_Spot }, HttpClient)
              .SaveContentBlockDefinition();
            await businessFlow3.Execute();

            // Act
            var response = await contentBlockClient.GetContentBlockForDisplayWithPriceSeriesOnly(ContentBlockId).GetRawResponse();

            // Assert
            response.MatchSnapshot();
        }

        [Fact]
        public async Task Return_Published_Price_Series_Details_For_Different_SeriesItemTypeCode_When_Price_Series_Saved_With_Price_Display_Configuration_Has_Different_SeriesItemTypeCode()
        {
            // Arrange
            var assessedDateTime = Today.FromUnixTimeMilliseconds();

            var contentBlockSaveRequest = new ContentBlockForDisplayInput
            {
                ContentBlockId = ContentBlockId,
                Title = "title",
                Columns =
                 [
                     new() { Field = "field", DisplayOrder = 1, Hidden = false }
                 ],
                Rows =
                 [
                    new() { PriceSeriesId = TestSeries.LNG_Argentina_Month_1, DisplayOrder = 1, SeriesItemTypeCode = SeriesItemTypeCode.SingleValueSeries.Value },
                    new() { PriceSeriesId = TestSeries.Melamine_Europe_NWE_Spot_FD, DisplayOrder = 1, SeriesItemTypeCode = SeriesItemTypeCode.RangeSeries.Value },
                    new() { PriceSeriesId = TestSeries.LNG_Dubai_MM1, DisplayOrder = 1, SeriesItemTypeCode = SeriesItemTypeCode.SingleValueWithReferenceSeries.Value }
                ]
            };
            _ = await contentBlockClient.SaveContentBlockForDisplay(contentBlockSaveRequest);

            var singleValueFlow = new PriceEntryBusinessFlow(ContentBlockId, new List<string> { TestSeries.LNG_Argentina_Month_1 }, HttpClient)
                 .SaveContentBlockDefinition()
                 .SaveSingleValuePriceSeriesItem(TestSeries.LNG_Argentina_Month_1, assessedDateTime, SingleValuePriceSeriesItemBuilder.BuildDefaultValidSeriesItem())
                 .PublishWithAdvanceWorkflow(assessedDateTime);
            await singleValueFlow.Execute();

            var rangeBusinessFlow = new PriceEntryBusinessFlow("ContentBlockId2", new List<string> { TestSeries.Melamine_Europe_NWE_Spot_FD }, HttpClient)
                 .SaveContentBlockDefinition()
                 .SaveRangePriceSeriesItem(TestSeries.Melamine_Europe_NWE_Spot_FD, assessedDateTime, RangePriceSeriesItemBuilder.BuildDefaultValidSeriesItem())
                 .PublishWithAdvanceWorkflow(assessedDateTime);
            await rangeBusinessFlow.Execute();

            var singleValueWithReferenceFlow = new PriceEntryBusinessFlow("ContentBlockId3", new List<string> { TestSeries.LNG_Dubai_MM1 }, HttpClient)
                 .SaveContentBlockDefinition()
                 .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_Dubai_MM1, assessedDateTime, SingleValueWithReferencePriceSeriesItemBuilder.BuildDefaultValidSeriesItem())
                 .PublishWithAdvanceWorkflow(assessedDateTime);
            await singleValueWithReferenceFlow.Execute();

            // Act
            var response = await contentBlockClient.GetContentBlockForDisplayWithPriceSeriesOnly(ContentBlockId).GetRawResponse();

            // Assert
            response.MatchSnapshotWithLastModifiedDate();
        }

        [Fact]
        public async Task Return_All_Latest_Price_Series_Details_When_Multiple_Price_Series_Saved_With_Price_Display_Configuration_Has_Only_One_Published_Price_Series_Item()
        {
            // Arrange
            var assessedDateTime = Today.FromUnixTimeMilliseconds();

            var contentBlockSaveRequest = new ContentBlockForDisplayInput
            {
                ContentBlockId = ContentBlockId,
                Title = "title",
                Columns =
                 [
                     new() { Field = "field", DisplayOrder = 1, Hidden = false }
                 ],
                Rows =
                 [
                    new() { PriceSeriesId = TestSeries.LNG_Argentina_Month_1, DisplayOrder = 1, SeriesItemTypeCode = SeriesItemTypeCode.SingleValueSeries.Value },
                    new() { PriceSeriesId = TestSeries.Melamine_Europe_NWE_Spot_FD, DisplayOrder = 1, SeriesItemTypeCode = SeriesItemTypeCode.RangeSeries.Value }
                ]
            };
            _ = await contentBlockClient.SaveContentBlockForDisplay(contentBlockSaveRequest);

            var draftWorkflow = new PriceEntryBusinessFlow(ContentBlockId, new List<string> { TestSeries.LNG_Argentina_Month_1 }, HttpClient)
                 .SaveContentBlockDefinition()
                 .SaveSingleValuePriceSeriesItem(TestSeries.LNG_Argentina_Month_1, assessedDateTime, SingleValuePriceSeriesItemBuilder.BuildDefaultValidSeriesItem());
            await draftWorkflow.Execute();

            var publishedWorkflow = new PriceEntryBusinessFlow("ContentBlockId2", new List<string> { TestSeries.Melamine_Europe_NWE_Spot_FD }, HttpClient)
                 .SaveContentBlockDefinition()
                 .SaveRangePriceSeriesItem(TestSeries.Melamine_Europe_NWE_Spot_FD, assessedDateTime, RangePriceSeriesItemBuilder.BuildDefaultValidSeriesItem())
                 .PublishWithAdvanceWorkflow(assessedDateTime);
            await publishedWorkflow.Execute();

            // Act
            var response = await contentBlockClient.GetContentBlockForDisplayWithPriceSeriesOnly(ContentBlockId).GetRawResponse();

            // Assert
            response.MatchSnapshotWithLastModifiedDate();
        }

        [Fact]
        public async Task Return_Latest_Price_Series_Details_When_Multiple_Price_Series_Are_Saved_With_Price_Display_Configuration_On_Different_Days_With_Different_Status()
        {
            // Arrange
            var assessedDateTime = Today.FromUnixTimeMilliseconds();
            var yesterday = assessedDateTime.AddDays(-1);
            var twoDaysAgo = assessedDateTime.AddDays(-2);

            var contentBlockSaveRequest = new ContentBlockForDisplayInput
            {
                ContentBlockId = ContentBlockId,
                Title = "title",
                Columns =
                 [
                     new() { Field = "field", DisplayOrder = 1, Hidden = false }
                 ],
                Rows =
                 [
                    new() { PriceSeriesId = TestSeries.LNG_Argentina_Month_1, DisplayOrder = 0, SeriesItemTypeCode = SeriesItemTypeCode.SingleValueSeries.Value },
                    new() { PriceSeriesId = TestSeries.Melamine_Europe_NWE_Spot_FD, DisplayOrder = 1, SeriesItemTypeCode = SeriesItemTypeCode.RangeSeries.Value }
                ]
            };
            _ = await contentBlockClient.SaveContentBlockForDisplay(contentBlockSaveRequest);

            var rangeSeriesItemOne = RangePriceSeriesItemBuilder.BuildDefaultValidSeriesItem();
            rangeSeriesItemOne.PriceHigh = 50;

            var rangeSeriesItemTwo = RangePriceSeriesItemBuilder.BuildDefaultValidSeriesItem();
            rangeSeriesItemTwo.PriceHigh = 30;

            var workflow1 = new PriceEntryBusinessFlow(ContentBlockId, new List<string> { TestSeries.LNG_Argentina_Month_1 }, HttpClient)
                 .SaveContentBlockDefinition()
                 .SaveSingleValuePriceSeriesItem(TestSeries.LNG_Argentina_Month_1, yesterday, SingleValuePriceSeriesItemBuilder.BuildDefaultValidSeriesItem())
                 .PublishWithAdvanceWorkflow(yesterday)
                 .SaveSingleValuePriceSeriesItem(TestSeries.LNG_Argentina_Month_1, assessedDateTime, SingleValuePriceSeriesItemBuilder.BuildDefaultValidSeriesItem());
            await workflow1.Execute();

            var workflow2 = new PriceEntryBusinessFlow("ContentBlockId2", new List<string> { TestSeries.Melamine_Europe_NWE_Spot_FD }, HttpClient)
               .SaveContentBlockDefinition()
               .SaveRangePriceSeriesItem(TestSeries.Melamine_Europe_NWE_Spot_FD, twoDaysAgo, rangeSeriesItemTwo)
               .PublishWithAdvanceWorkflow(twoDaysAgo)
               .SaveRangePriceSeriesItem(TestSeries.Melamine_Europe_NWE_Spot_FD, yesterday, rangeSeriesItemOne)
               .PublishWithAdvanceWorkflow(yesterday)
               .SaveRangePriceSeriesItem(TestSeries.Melamine_Europe_NWE_Spot_FD, assessedDateTime, rangeSeriesItemTwo);
            await workflow2.Execute();

            // Act
            var response = await contentBlockClient.GetContentBlockForDisplayWithPriceSeriesOnly(ContentBlockId).GetRawResponse();

            // Assert
            response.MatchSnapshotWithLastModifiedDate();
        }

        [Fact]
        public async Task Return_Null_When_Row_Property_In_Content_Block_Definition_Is_Null()
        {
            // Arrange
            var contentBlockSaveRequest = new ContentBlockForDisplayInput
            {
                ContentBlockId = ContentBlockId,
                Title = "title",
                Columns =
                 [
                     new() { Field = "field", DisplayOrder = 1, Hidden = false }
                 ],
                Rows = null
            };
            _ = await contentBlockClient.SaveContentBlockForDisplay(contentBlockSaveRequest);

            // Act
            var response = await contentBlockClient.GetContentBlockForDisplayWithPriceSeriesOnly(ContentBlockId).GetRawResponse();

            // Assert
            response.MatchSnapshot();
        }

        [Fact]
        public async Task Return_Null_When_Price_Series_Ids_Are_Not_Null_But_Empty_Strings()
        {
            // Arrange
            var contentBlockSaveRequest = new ContentBlockForDisplayInput
            {
                ContentBlockId = ContentBlockId,
                Title = "title",
                Columns =
                 [
                     new() { Field = "field", DisplayOrder = 1, Hidden = false }
                 ],
                Rows =
                 [
                    new() { PriceSeriesId = string.Empty, DisplayOrder = 0, SeriesItemTypeCode = SeriesItemTypeCode.SingleValueSeries.Value },
                    new() { PriceSeriesId = string.Empty, DisplayOrder = 1, SeriesItemTypeCode = SeriesItemTypeCode.RangeSeries.Value }
                ]
            };
            _ = await contentBlockClient.SaveContentBlockForDisplay(contentBlockSaveRequest);

            // Act
            var response = await contentBlockClient.GetContentBlockForDisplayWithPriceSeriesOnly(ContentBlockId).GetRawResponse();

            // Assert
            response.MatchSnapshot();
        }

        [Fact]
        public async Task Return_Corrected_Price_Series_Details_When_Price_Is_Corrected_And_Published()
        {
            // Arrange
            var assessedDateTime = Today.FromUnixTimeMilliseconds();

            var contentBlockSaveRequest = new ContentBlockForDisplayInput
            {
                ContentBlockId = ContentBlockId,
                Title = "title",
                Columns =
                [
                     new() { Field = "field", DisplayOrder = 1, Hidden = false }
                ],
                Rows =
                [
                    new() { PriceSeriesId = TestSeries.Melamine_China_Spot_Cif_2_6_Weeks, DisplayOrder = 0, SeriesItemTypeCode = SeriesItemTypeCode.Range },
                ]
            };

            _ = await contentBlockClient.SaveContentBlockForDisplay(contentBlockSaveRequest);

            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, new List<string> { TestSeries.Melamine_China_Spot_Cif_2_6_Weeks }, HttpClient)
                .SaveContentBlockDefinition()
                .SaveRangePriceSeriesItem(TestSeries.Melamine_China_Spot_Cif_2_6_Weeks, assessedDateTime, RangePriceSeriesItemBuilder.BuildDefaultValidSeriesItem())
                .PublishWithAdvanceWorkflow(assessedDateTime)
                .InitiateCorrection(assessedDateTime)
                .SavePriceSeriesItem(TestSeries.Melamine_China_Spot_Cif_2_6_Weeks, assessedDateTime, SeriesItemTypeCode.RangeSeries, new SeriesItem { PriceLow = 10, PriceHigh = 50 }, "correction")
                .AdvancedCorrectionInitiateSendForReview(assessedDateTime)
                .AdvancedCorrectionAcknowledgeSendForReview(assessedDateTime)
                .AdvancedCorrectionInitiateStartReview(assessedDateTime)
                .AdvancedCorrectionAcknowledgeStartReview(assessedDateTime)
                .AdvancedCorrectionInitiateApproval(assessedDateTime)
                .AdvancedCorrectionAcknowledgeApproval(assessedDateTime)
                .AdvancedCorrectionAcknowledgePublished(assessedDateTime);

            await businessFlow.Execute();

            // Act
            var response = await contentBlockClient.GetContentBlockForDisplayWithPriceSeriesOnly(ContentBlockId).GetRawResponse();

            // Assert
            response.MatchSnapshotWithLastModifiedDate();
        }

        [Fact]
        public async Task Return_Original_Price_Series_Details_When_Price_Is_Corrected_But_Not_Published()
        {
            // Arrange
            var assessedDateTime = Today.FromUnixTimeMilliseconds();

            var contentBlockSaveRequest = new ContentBlockForDisplayInput
            {
                ContentBlockId = ContentBlockId,
                Title = "title",
                Columns =
                [
                    new() { Field = "field", DisplayOrder = 1, Hidden = false }
                ],
                Rows =
                [
                    new() { PriceSeriesId = TestSeries.Melamine_China_Spot_Cif_2_6_Weeks, DisplayOrder = 0, SeriesItemTypeCode = SeriesItemTypeCode.Range },
                ]
            };

            _ = await contentBlockClient.SaveContentBlockForDisplay(contentBlockSaveRequest);

            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, new List<string> { TestSeries.Melamine_China_Spot_Cif_2_6_Weeks }, HttpClient)
                .SaveContentBlockDefinition()
                .SaveRangePriceSeriesItem(TestSeries.Melamine_China_Spot_Cif_2_6_Weeks, assessedDateTime, RangePriceSeriesItemBuilder.BuildDefaultValidSeriesItem())
                .PublishWithAdvanceWorkflow(assessedDateTime)
                .InitiateCorrection(assessedDateTime)
                .SavePriceSeriesItem(TestSeries.Melamine_China_Spot_Cif_2_6_Weeks, assessedDateTime, SeriesItemTypeCode.RangeSeries, new SeriesItem { PriceLow = 10, PriceHigh = 50 }, "correction")
                .AdvancedCorrectionInitiateSendForReview(assessedDateTime)
                .AdvancedCorrectionAcknowledgeSendForReview(assessedDateTime)
                .AdvancedCorrectionInitiateStartReview(assessedDateTime)
                .AdvancedCorrectionAcknowledgeStartReview(assessedDateTime);

            await businessFlow.Execute();

            // Act
            var response = await contentBlockClient.GetContentBlockForDisplayWithPriceSeriesOnly(ContentBlockId).GetRawResponse();

            // Assert
            response.MatchSnapshotWithLastModifiedDate();
        }

        [Fact]
        public async Task Return_Range_Price_Series_Detail_With_Adjusted_Delta_When_High_And_Low_Delta_Is_Adjusted_And_Published()
        {
            // Arrange
            var assessedDateTime = Today.FromUnixTimeMilliseconds();

            var contentBlockSaveRequest = new ContentBlockForDisplayInput
            {
                ContentBlockId = ContentBlockId,
                Title = "title",
                Columns =
                [
                     new() { Field = "field", DisplayOrder = 1, Hidden = false }
                ],
                Rows =
                [
                    new() { PriceSeriesId = TestSeries.Melamine_China_Spot_Cif_2_6_Weeks, DisplayOrder = 0, SeriesItemTypeCode = SeriesItemTypeCode.Range },
                ]
            };

            _ = await contentBlockClient.SaveContentBlockForDisplay(contentBlockSaveRequest);

            SeriesItem itemAfterAdjustment = RangePriceSeriesItemBuilder.SetNonMarketAdjustmentValues(RangePriceSeriesItemBuilder.BuildDefaultValidSeriesItem());

            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, new List<string> { TestSeries.Melamine_China_Spot_Cif_2_6_Weeks }, HttpClient)
                .SaveContentBlockDefinition()
                .SaveRangePriceSeriesItem(TestSeries.Melamine_China_Spot_Cif_2_6_Weeks, assessedDateTime, RangePriceSeriesItemBuilder.BuildDefaultValidSeriesItem())
                .StartNonMarketAdjustment(assessedDateTime)
                .SaveRangePriceSeriesItem(TestSeries.Melamine_China_Spot_Cif_2_6_Weeks, assessedDateTime, itemAfterAdjustment)
                .PublishWithAdvanceWorkflow(assessedDateTime);
            await businessFlow.Execute();

            // Act
            var response = await contentBlockClient.GetContentBlockForDisplayWithPriceSeriesOnly(ContentBlockId).GetRawResponse();

            // Assert
            response.MatchSnapshotForAdjustedDeltaRangePrice(
                TestData.NonMarketAdjustment,
                itemAfterAdjustment.AdjustedPriceLowDelta,
                itemAfterAdjustment.AdjustedPriceHighDelta);
        }

        [Fact]
        public async Task Return_Single_Price_Series_Detail_With_Adjusted_Delta_When_Delta_Is_Adjusted_And_Published()
        {
            // Arrange
            var assessedDateTime = Today.FromUnixTimeMilliseconds();

            var contentBlockSaveRequest = new ContentBlockForDisplayInput
            {
                ContentBlockId = ContentBlockId,
                Title = "title",
                Columns =
                [
                     new() { Field = "field", DisplayOrder = 1, Hidden = false }
                ],
                Rows =
                [
                    new() { PriceSeriesId = TestSeries.LNG_Argentina_Month_1, DisplayOrder = 0, SeriesItemTypeCode = SeriesItemTypeCode.SingleValue }
                ]
            };

            _ = await contentBlockClient.SaveContentBlockForDisplay(contentBlockSaveRequest);

            SeriesItem itemAfterAdjustment = SingleValuePriceSeriesItemBuilder.SetNonMarketAdjustmentValues(SingleValuePriceSeriesItemBuilder.BuildDefaultValidSeriesItem());

            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, new List<string> { TestSeries.LNG_Argentina_Month_1 }, HttpClient)
                .SaveContentBlockDefinition()
                .SaveSingleValuePriceSeriesItem(TestSeries.LNG_Argentina_Month_1, assessedDateTime, SingleValuePriceSeriesItemBuilder.BuildDefaultValidSeriesItem())
                .StartNonMarketAdjustment(assessedDateTime)
                .SaveSingleValuePriceSeriesItem(TestSeries.LNG_Argentina_Month_1, assessedDateTime, itemAfterAdjustment)
                .PublishWithAdvanceWorkflow(assessedDateTime);
            await businessFlow.Execute();

            // Act
            var response = await contentBlockClient.GetContentBlockForDisplayWithPriceSeriesOnly(ContentBlockId).GetRawResponse();

            // Assert
            response.MatchSnapshotForAdjustedDeltaSinglePrice(
                TestData.NonMarketAdjustment,
                itemAfterAdjustment.AdjustedPriceDelta);
        }
    }
}
