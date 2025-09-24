namespace Subscriber.Tests.Application.PriceDisplay.ContentBlock.Query.ContentBlockForDisplayWithPriceSeries
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using BusinessLayer.Helpers;
    using BusinessLayer.PriceDisplay.ContentBlock.DTOs.Input;
    using BusinessLayer.PriceEntry.DTOs.Input;
    using BusinessLayer.PriceEntry.ValueObjects;

    using Microsoft.Extensions.DependencyInjection;

    using Snapshooter.Xunit;

    using Subscriber.Tests.Infrastructure;

    using Test.Infrastructure.Authoring;
    using Test.Infrastructure.Authoring.BusinessFlows.PriceEntryBusinessFlow;
    using Test.Infrastructure.Authoring.BusinessFlows.PriceEntryBusinessFlow.PriceSeriesItemBuilders;
    using Test.Infrastructure.Authoring.GraphQLClients.PriceDisplay;
    using Test.Infrastructure.Extensions;
    using Test.Infrastructure.Stubs;
    using Test.Infrastructure.Subscriber;
    using Test.Infrastructure.Subscriber.GraphQLClients.PriceDisplay;
    using Test.Infrastructure.TestData;

    using Xunit;

    [Collection(WebApplicationCollection.Name)]
    public class ContentBlockQueryForDisplayWithPriceSeriesShould : WebApplicationTestBase
    {
        protected const string ContentBlockId = "contentBlockId";

        protected static readonly long Today = TestData.Now.ToUnixTimeMilliseconds();
        protected static readonly DateTime AssessedDateTime = TestData.Now;

        private readonly ContentBlockForDisplaySubscriberGraphQLClient subscriberContentBlockClient;

        private readonly ContentBlockForDisplayAuthoringGraphQLClient authoringContentBlockClient;
        private readonly TestClock testClock;

        public ContentBlockQueryForDisplayWithPriceSeriesShould(
            AuthoringBffApplicationFactory authoringFactory,
            SubscriberBffApplicationFactory subscriberFactory)
            : base(subscriberFactory, authoringFactory)
        {
            subscriberContentBlockClient = new ContentBlockForDisplaySubscriberGraphQLClient(SubscriberGraphQLClient);
            authoringContentBlockClient = new ContentBlockForDisplayAuthoringGraphQLClient(AuthoringGraphQLClient);

            testClock = subscriberFactory.Services.GetRequiredService<TestClock>();
            testClock.SetUtcNow(TestData.Now);
        }

        [Fact]
        public async Task
            Return_Multiple_Price_Series_Details_When_Multiple_Price_Series_Is_Saved_With_Price_Display_Configuration()
        {
            // Arrange
            var contentBlockSaveRequest = new ContentBlockForDisplayInput
            {
                ContentBlockId = ContentBlockId,
                Title = "title",
                Columns =
                [
                    new ColumnForDisplayInput { Field = "field", DisplayOrder = 1, Hidden = false }
                ],
                Rows =
                [
                    new RowForDisplayInput
                    {
                        PriceSeriesId = TestSeries.Melamine_Asia_SE_Spot,
                        DisplayOrder = 0,
                        SeriesItemTypeCode = SeriesItemTypeCode.Range
                    },
                    new RowForDisplayInput
                    {
                        PriceSeriesId = TestSeries.Melamine_China_Spot_Cif_2_6_Weeks,
                        DisplayOrder = 1,
                        SeriesItemTypeCode = SeriesItemTypeCode.Range
                    },
                ],
                SelectedFilters = new SelectedFiltersForDisplayInput()
                {
                    SelectedCommodities = ["Melamine"],
                    SelectedRegions = ["Asia", "China", "Europe"],
                    SelectedTransactionTypes = ["Spot"],
                    SelectedPriceCategories = null,
                    SelectedAssessedFrequencies = null,
                    IsInactiveIncluded = false,
                }
            };

            _ = await authoringContentBlockClient.SaveContentBlockForDisplay(contentBlockSaveRequest).GetRawResponse();

            var businessFlow = new PriceEntryBusinessFlow(
                ContentBlockId,
                new List<string> { TestSeries.Melamine_Asia_SE_Spot, TestSeries.Melamine_China_Spot_Cif_2_6_Weeks },
                AuthoringHttpClient)
               .SaveContentBlockDefinition()
               .SaveRangePriceSeriesItem(TestSeries.Melamine_China_Spot_Cif_2_6_Weeks, AssessedDateTime, RangePriceSeriesItemBuilder.BuildDefaultValidSeriesItem())
               .SaveRangePriceSeriesItem(TestSeries.Melamine_Asia_SE_Spot, AssessedDateTime, RangePriceSeriesItemBuilder.BuildDefaultValidSeriesItem())
               .PublishWithAdvanceWorkflow(AssessedDateTime);
            await businessFlow.Execute();

            // Act
            var response = await subscriberContentBlockClient.GetContentBlockForDisplayWithPriceSeriesOnly(ContentBlockId, Today).GetRawResponse();

            // Assert
            response.MatchSnapshotWithLastModifiedDate();
        }

        [Fact]
        public async Task
            Return_Price_Series_Details_When_Multiple_Price_Series_Saved_With_Price_Display_Configuration_Has_Only_One_Series_Item()
        {
            // Arrange
            var contentBlockSaveRequest = new ContentBlockForDisplayInput
            {
                ContentBlockId = ContentBlockId,
                Title = "title",
                Columns =
                [
                    new ColumnForDisplayInput { Field = "field", DisplayOrder = 1, Hidden = false }
                ],
                Rows =
                [
                    new RowForDisplayInput { PriceSeriesId = TestSeries.Melamine_Asia_SE_Spot, DisplayOrder = 1, SeriesItemTypeCode = SeriesItemTypeCode.Range },
                    new RowForDisplayInput { PriceSeriesId = TestSeries.Melamine_China_Spot_Cif_2_6_Weeks, DisplayOrder = 0, SeriesItemTypeCode = SeriesItemTypeCode.Range },

                ],
                SelectedFilters = new SelectedFiltersForDisplayInput()
                {
                    SelectedCommodities = ["Melamine"],
                    SelectedRegions = ["Asia", "China"],
                    SelectedTransactionTypes = ["Spot"],
                    SelectedPriceCategories = null,
                    SelectedAssessedFrequencies = null,
                    IsInactiveIncluded = false,
                }
            };

            _ = await authoringContentBlockClient.SaveContentBlockForDisplay(contentBlockSaveRequest);

            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, new List<string> { TestSeries.Melamine_Asia_SE_Spot }, AuthoringHttpClient)
               .SaveContentBlockDefinition()
               .SaveRangePriceSeriesItem(TestSeries.Melamine_Asia_SE_Spot, AssessedDateTime, RangePriceSeriesItemBuilder.BuildDefaultValidSeriesItem())
               .PublishWithAdvanceWorkflow(AssessedDateTime);
            await businessFlow.Execute();

            var businessFlow2 = new PriceEntryBusinessFlow("ContentBlockId2", new List<string> { TestSeries.Melamine_China_Spot_Cif_2_6_Weeks }, AuthoringHttpClient)
              .SaveContentBlockDefinition();
            await businessFlow2.Execute();

            // Act
            var response = await subscriberContentBlockClient.GetContentBlockForDisplayWithPriceSeriesOnly("contentBlockId", Today).GetRawResponse();

            // Assert
            response.MatchSnapshotWithLastModifiedDate();
        }

        [Fact]
        public async Task
            Return_Single_Price_Series_Details_When_Single_Price_Series_Is_Saved_With_Price_Display_Configuration()
        {
            // Arrange
            var contentBlockSaveRequest = new ContentBlockForDisplayInput
            {
                ContentBlockId = ContentBlockId,
                Title = "title",
                Columns =
                [
                    new ColumnForDisplayInput { Field = "field", DisplayOrder = 1, Hidden = false }
                ],
                Rows =
                [
                    new RowForDisplayInput { PriceSeriesId = TestSeries.Melamine_Asia_SE_Spot, DisplayOrder = 1, SeriesItemTypeCode = SeriesItemTypeCode.Range }
                ],
                SelectedFilters = new SelectedFiltersForDisplayInput()
                {
                    SelectedCommodities = ["Melamine"],
                    SelectedRegions = ["Asia"],
                    SelectedTransactionTypes = ["Spot"],
                    SelectedPriceCategories = null,
                    SelectedAssessedFrequencies = null,
                    IsInactiveIncluded = false,
                }
            };

            _ = await authoringContentBlockClient.SaveContentBlockForDisplay(contentBlockSaveRequest);

            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, new List<string> { TestSeries.Melamine_Asia_SE_Spot }, AuthoringHttpClient)
               .SaveContentBlockDefinition()
               .SaveRangePriceSeriesItem(TestSeries.Melamine_Asia_SE_Spot, AssessedDateTime, RangePriceSeriesItemBuilder.BuildDefaultValidSeriesItem())
               .PublishWithAdvanceWorkflow(AssessedDateTime);
            await businessFlow.Execute();

            // Act
            var response = await subscriberContentBlockClient.GetContentBlockForDisplayWithPriceSeriesOnly("contentBlockId", Today).GetRawResponse();

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
                    new ColumnForDisplayInput { Field = "field", DisplayOrder = 1, Hidden = false }
                ],
                Rows =
                [
                    new RowForDisplayInput
                    {
                        PriceSeriesId = "notexistPriceSeriesIds",
                        DisplayOrder = 1,
                        SeriesItemTypeCode = SeriesItemTypeCode.Range
                    },
                    new RowForDisplayInput
                    {
                        PriceSeriesId = "notexistPriceSeriesIds2",
                        DisplayOrder = 1,
                        SeriesItemTypeCode = SeriesItemTypeCode.Range
                    }
                ]
            };

            _ = await authoringContentBlockClient.SaveContentBlockForDisplay(contentBlockInput);

            // Act
            var response = await subscriberContentBlockClient.GetContentBlockForDisplayWithPriceSeriesOnly(contentBlockInput.ContentBlockId, Today).GetRawResponse();

            // Assert
            response.MatchSnapshot();
        }

        [Fact]
        public async Task Return_Price_Series_Details_In_Custom_Order()
        {
            // Arrange
            var contentBlockSaveRequest = new ContentBlockForDisplayInput
            {
                ContentBlockId = ContentBlockId,
                Title = "title",
                Columns =
                [
                    new ColumnForDisplayInput { Field = "field", DisplayOrder = 1, Hidden = false }
                ],
                Rows =
                 [
                    new RowForDisplayInput { PriceSeriesId = TestSeries.Melamine_Asia_SE_Spot, DisplayOrder = 1, SeriesItemTypeCode = SeriesItemTypeCode.Range },
                    new RowForDisplayInput { PriceSeriesId = TestSeries.Melamine_China_Spot_Cif_2_6_Weeks, DisplayOrder = 0, SeriesItemTypeCode = SeriesItemTypeCode.Range },
                ],
                SelectedFilters = new SelectedFiltersForDisplayInput()
                {
                    SelectedCommodities = ["Melamine"],
                    SelectedRegions = ["Asia", "China", "Europe"],
                    SelectedTransactionTypes = ["Spot"],
                    SelectedPriceCategories = null,
                    SelectedAssessedFrequencies = null,
                    IsInactiveIncluded = false,
                }
            };
            _ = await authoringContentBlockClient.SaveContentBlockForDisplay(contentBlockSaveRequest);

            var businessFlow = new PriceEntryBusinessFlow(
                ContentBlockId,
                new List<string> { TestSeries.Melamine_Asia_SE_Spot, TestSeries.Melamine_China_Spot_Cif_2_6_Weeks },
                AuthoringHttpClient)
               .SaveContentBlockDefinition()
               .SaveRangePriceSeriesItem(TestSeries.Melamine_China_Spot_Cif_2_6_Weeks, AssessedDateTime, RangePriceSeriesItemBuilder.BuildDefaultValidSeriesItem())
               .SaveRangePriceSeriesItem(TestSeries.Melamine_Asia_SE_Spot, AssessedDateTime, RangePriceSeriesItemBuilder.BuildDefaultValidSeriesItem())
               .PublishWithAdvanceWorkflow(AssessedDateTime);
            await businessFlow.Execute();

            // Act
            var response = await subscriberContentBlockClient.GetContentBlockForDisplayWithPriceSeriesOnly(ContentBlockId, Today).GetRawResponse();

            // Assert
            response.MatchSnapshotWithLastModifiedDate();
        }

        [Fact]
        public async Task Return_Price_Series_Details_With_Published_Status()
        {
            // Arrange
            var contentBlockSaveRequest = new ContentBlockForDisplayInput
            {
                ContentBlockId = ContentBlockId,
                Title = "title",
                Columns =
                [
                    new ColumnForDisplayInput { Field = "field", DisplayOrder = 1, Hidden = false }
                ],
                Rows =
                [

                    new RowForDisplayInput { PriceSeriesId = TestSeries.Melamine_China_Spot_Cif_2_6_Weeks, DisplayOrder = 0, SeriesItemTypeCode = SeriesItemTypeCode.Range },

                ],
                SelectedFilters = new SelectedFiltersForDisplayInput()
                {
                    SelectedCommodities = ["Melamine"],
                    SelectedRegions = ["China"],
                    SelectedTransactionTypes = ["Spot"],
                    SelectedPriceCategories = null,
                    SelectedAssessedFrequencies = null,
                    IsInactiveIncluded = false,
                }
            };
            _ = await authoringContentBlockClient.SaveContentBlockForDisplay(contentBlockSaveRequest);

            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, new List<string> { TestSeries.Melamine_China_Spot_Cif_2_6_Weeks }, AuthoringHttpClient)
                 .SaveContentBlockDefinition()
                 .SaveRangePriceSeriesItem(TestSeries.Melamine_China_Spot_Cif_2_6_Weeks, AssessedDateTime, RangePriceSeriesItemBuilder.BuildDefaultValidSeriesItem())
                 .PublishWithAdvanceWorkflow(AssessedDateTime);
            await businessFlow.Execute();

            // Act
            var response = await subscriberContentBlockClient.GetContentBlockForDisplayWithPriceSeriesOnly(ContentBlockId, Today).GetRawResponse();

            // Assert
            response.MatchSnapshotWithLastModifiedDate();
        }

        [Fact]
        public async Task
            Return_Null_For_Price_Series_Details_When_Multiple_PriceSeries_Saved_With_Price_Display_Configuration_Has_No_Price_Series_Items()
        {
            // Arrange
            var contentBlockSaveRequest = new ContentBlockForDisplayInput
            {
                ContentBlockId = ContentBlockId,
                Title = "title",
                Columns =
                [
                    new ColumnForDisplayInput { Field = "field", DisplayOrder = 1, Hidden = false }
                ],
                Rows =
                [

                    new RowForDisplayInput { PriceSeriesId = TestSeries.Melamine_China_Spot_Cif_2_6_Weeks, DisplayOrder = 0, SeriesItemTypeCode = SeriesItemTypeCode.Range },
                    new RowForDisplayInput { PriceSeriesId = TestSeries.Melamine_Europe_Spot_CIF, DisplayOrder = 1, SeriesItemTypeCode = SeriesItemTypeCode.Range },
                    new RowForDisplayInput { PriceSeriesId = TestSeries.Melamine_Asia_NE_Spot, DisplayOrder = 2, SeriesItemTypeCode = SeriesItemTypeCode.Range },

                ],
                SelectedFilters = new SelectedFiltersForDisplayInput()
                {
                    SelectedCommodities = ["Melamine"],
                    SelectedRegions = ["Asia", "China", "Europe"],
                    SelectedTransactionTypes = ["Spot"],
                    SelectedPriceCategories = null,
                    SelectedAssessedFrequencies = null,
                    IsInactiveIncluded = false,
                }
            };
            _ = await authoringContentBlockClient.SaveContentBlockForDisplay(contentBlockSaveRequest);

            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, new List<string> { TestSeries.Melamine_China_Spot_Cif_2_6_Weeks }, AuthoringHttpClient)
                 .SaveContentBlockDefinition();
            await businessFlow.Execute();

            var businessFlow2 = new PriceEntryBusinessFlow(
                ContentBlockId,
                new List<string> { TestSeries.Melamine_Europe_Spot_CIF },
                AuthoringHttpClient).SaveContentBlockDefinition();
            await businessFlow2.Execute();

            var businessFlow3 = new PriceEntryBusinessFlow(
                ContentBlockId,
                new List<string> { TestSeries.Melamine_Asia_NE_Spot },
                AuthoringHttpClient).SaveContentBlockDefinition();
            await businessFlow3.Execute();

            // Act
            var response = await subscriberContentBlockClient.GetContentBlockForDisplayWithPriceSeriesOnly(ContentBlockId, Today).GetRawResponse();

            // Assert
            response.MatchSnapshot();
        }

        [Fact]
        public async Task
            Return_Published_Price_Series_Details_For_Different_SeriesItemTypeCode_When_Price_Series_Saved_With_Price_Display_Configuration_Has_Different_SeriesItemTypeCode()
        {
            // Arrange
            var contentBlockSaveRequest = new ContentBlockForDisplayInput
            {
                ContentBlockId = ContentBlockId,
                Title = "title",
                Columns =
                [
                    new ColumnForDisplayInput { Field = "field", DisplayOrder = 1, Hidden = false }
                ],
                Rows =
                [

                    new RowForDisplayInput { PriceSeriesId = TestSeries.LNG_Argentina_Month_1, DisplayOrder = 1, SeriesItemTypeCode = SeriesItemTypeCode.SingleValueSeries.Value },
                    new RowForDisplayInput { PriceSeriesId = TestSeries.Melamine_Europe_NWE_Spot_FD, DisplayOrder = 1, SeriesItemTypeCode = SeriesItemTypeCode.RangeSeries.Value },
                    new RowForDisplayInput { PriceSeriesId = TestSeries.LNG_Dubai_MM1, DisplayOrder = 1, SeriesItemTypeCode = SeriesItemTypeCode.SingleValueWithReferenceSeries.Value }
                ],
                SelectedFilters = new SelectedFiltersForDisplayInput()
                {
                    SelectedCommodities = ["Melamine", "LNG"],
                    SelectedRegions = null,
                    SelectedTransactionTypes = null,
                    SelectedPriceCategories = null,
                    SelectedAssessedFrequencies = null,
                    IsInactiveIncluded = false,
                }
            };
            _ = await authoringContentBlockClient.SaveContentBlockForDisplay(contentBlockSaveRequest);

            var singleValueFlow =
                new PriceEntryBusinessFlow(
                        ContentBlockId,
                        new List<string> { TestSeries.LNG_Argentina_Month_1 },
                        AuthoringHttpClient)
                   .SaveContentBlockDefinition()
                   .SaveSingleValuePriceSeriesItem(
                        TestSeries.LNG_Argentina_Month_1,
                        AssessedDateTime,
                        SingleValuePriceSeriesItemBuilder.BuildDefaultValidSeriesItem())
                   .PublishWithAdvanceWorkflow(AssessedDateTime);
            await singleValueFlow.Execute();

            var rangeBusinessFlow = new PriceEntryBusinessFlow("ContentBlockId2", new List<string> { TestSeries.Melamine_Europe_NWE_Spot_FD }, AuthoringHttpClient)
                 .SaveContentBlockDefinition()
                 .SaveRangePriceSeriesItem(TestSeries.Melamine_Europe_NWE_Spot_FD, AssessedDateTime, RangePriceSeriesItemBuilder.BuildDefaultValidSeriesItem())
                 .PublishWithAdvanceWorkflow(AssessedDateTime);
            await rangeBusinessFlow.Execute();

            var singleValueWithReferenceFlow =
                new PriceEntryBusinessFlow(
                        "ContentBlockId3",
                        new List<string> { TestSeries.LNG_Dubai_MM1 },
                        AuthoringHttpClient)
                   .SaveContentBlockDefinition()
                   .SaveSingleValueWithReferencePriceSeriesItem(
                        TestSeries.LNG_Dubai_MM1,
                        AssessedDateTime,
                        SingleValueWithReferencePriceSeriesItemBuilder.BuildDefaultValidSeriesItem())
                   .PublishWithAdvanceWorkflow(AssessedDateTime);
            await singleValueWithReferenceFlow.Execute();

            // Act
            var response = await subscriberContentBlockClient.GetContentBlockForDisplayWithPriceSeriesOnly(ContentBlockId, Today).GetRawResponse();

            // Assert
            response.MatchSnapshotWithLastModifiedDate();
        }

        [Fact]
        public async Task
            Return_Published_Price_Series_Details_When_Multiple_Price_Series_Saved_With_Price_Display_Configuration_Has_Only_One_Published_Price_Series_Item()
        {
            // Arrange
            var contentBlockSaveRequest = new ContentBlockForDisplayInput
            {
                ContentBlockId = ContentBlockId,
                Title = "title",
                Columns =
                [
                    new ColumnForDisplayInput { Field = "field", DisplayOrder = 1, Hidden = false }
                ],
                Rows =
                [

                    new RowForDisplayInput { PriceSeriesId = TestSeries.LNG_Argentina_Month_1, DisplayOrder = 1, SeriesItemTypeCode = SeriesItemTypeCode.SingleValueSeries.Value },
                    new RowForDisplayInput { PriceSeriesId = TestSeries.Melamine_Europe_NWE_Spot_FD, DisplayOrder = 1, SeriesItemTypeCode = SeriesItemTypeCode.RangeSeries.Value }
                ],
                SelectedFilters = new SelectedFiltersForDisplayInput()
                {
                    SelectedCommodities = new List<string>() { "Melamine", "LNG" },
                    SelectedRegions = null,
                    SelectedTransactionTypes = null,
                    SelectedPriceCategories = null,
                    SelectedAssessedFrequencies = null,
                    IsInactiveIncluded = false,
                }
            };
            _ = await authoringContentBlockClient.SaveContentBlockForDisplay(contentBlockSaveRequest);

            var draftWorkflow =
                new PriceEntryBusinessFlow(
                        ContentBlockId,
                        new List<string> { TestSeries.LNG_Argentina_Month_1 },
                        AuthoringHttpClient)
                   .SaveContentBlockDefinition()
                   .SaveSingleValuePriceSeriesItem(
                        TestSeries.LNG_Argentina_Month_1,
                        AssessedDateTime,
                        SingleValuePriceSeriesItemBuilder.BuildDefaultValidSeriesItem());
            await draftWorkflow.Execute();

            var publishedWorkflow = new PriceEntryBusinessFlow("ContentBlockId2", new List<string> { TestSeries.Melamine_Europe_NWE_Spot_FD }, AuthoringHttpClient)
                 .SaveContentBlockDefinition()
                 .SaveRangePriceSeriesItem(TestSeries.Melamine_Europe_NWE_Spot_FD, AssessedDateTime, RangePriceSeriesItemBuilder.BuildDefaultValidSeriesItem())
                 .PublishWithAdvanceWorkflow(AssessedDateTime);
            await publishedWorkflow.Execute();

            // Act
            var response = await subscriberContentBlockClient.GetContentBlockForDisplayWithPriceSeriesOnly(ContentBlockId, Today).GetRawResponse();

            // Assert
            response.MatchSnapshotWithLastModifiedDate();
        }

        [Fact]
        public async Task
            Return_Latest_Published_Price_Series_Details_When_Multiple_Price_Series_Are_Saved_With_Price_Display_Configuration_On_Different_Days_With_Different_Status()
        {
            // Arrange
            var today = AssessedDateTime;
            var yesterday = today.AddDays(-1);
            var twoDaysAgo = today.AddDays(-2);

            var contentBlockSaveRequest = new ContentBlockForDisplayInput
            {
                ContentBlockId = ContentBlockId,
                Title = "title",
                Columns =
                [
                    new ColumnForDisplayInput { Field = "field", DisplayOrder = 1, Hidden = false }
                ],
                Rows =
                [

                    new RowForDisplayInput { PriceSeriesId = TestSeries.LNG_Argentina_Month_1, DisplayOrder = 0, SeriesItemTypeCode = SeriesItemTypeCode.SingleValueSeries.Value },
                    new RowForDisplayInput { PriceSeriesId = TestSeries.Melamine_Europe_NWE_Spot_FD, DisplayOrder = 1, SeriesItemTypeCode = SeriesItemTypeCode.RangeSeries.Value }
                ],
                SelectedFilters = new SelectedFiltersForDisplayInput()
                {
                    SelectedCommodities = ["Melamine", "LNG"],
                    SelectedRegions = null,
                    SelectedTransactionTypes = null,
                    SelectedPriceCategories = null,
                    SelectedAssessedFrequencies = null,
                    IsInactiveIncluded = false,
                }
            };
            _ = await authoringContentBlockClient.SaveContentBlockForDisplay(contentBlockSaveRequest);

            var rangeSeriesItemOne = RangePriceSeriesItemBuilder.BuildDefaultValidSeriesItem();
            rangeSeriesItemOne.PriceHigh = 50;

            var rangeSeriesItemTwo = RangePriceSeriesItemBuilder.BuildDefaultValidSeriesItem();
            rangeSeriesItemTwo.PriceHigh = 30;

            var workflow1 =
                new PriceEntryBusinessFlow(
                        ContentBlockId,
                        new List<string> { TestSeries.LNG_Argentina_Month_1 },
                        AuthoringHttpClient)
                   .SaveContentBlockDefinition()
                   .SaveSingleValuePriceSeriesItem(
                        TestSeries.LNG_Argentina_Month_1,
                        yesterday,
                        SingleValuePriceSeriesItemBuilder.BuildDefaultValidSeriesItem())
                   .PublishWithAdvanceWorkflow(yesterday);
            await workflow1.Execute();

            var workflow2 = new PriceEntryBusinessFlow("ContentBlockId2", new List<string> { TestSeries.Melamine_Europe_NWE_Spot_FD }, AuthoringHttpClient)
               .SaveContentBlockDefinition()
               .SaveRangePriceSeriesItem(TestSeries.Melamine_Europe_NWE_Spot_FD, twoDaysAgo, rangeSeriesItemTwo)
               .PublishWithAdvanceWorkflow(twoDaysAgo)
               .SaveRangePriceSeriesItem(TestSeries.Melamine_Europe_NWE_Spot_FD, yesterday, rangeSeriesItemOne)
               .PublishWithAdvanceWorkflow(yesterday)
               .SaveRangePriceSeriesItem(TestSeries.Melamine_Europe_NWE_Spot_FD, today, rangeSeriesItemTwo);
            await workflow2.Execute();

            // Act
            var response = await subscriberContentBlockClient.GetContentBlockForDisplayWithPriceSeriesOnly(ContentBlockId, Today).GetRawResponse();

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
                    new ColumnForDisplayInput { Field = "field", DisplayOrder = 1, Hidden = false }
                ],
                Rows = null
            };
            _ = await authoringContentBlockClient.SaveContentBlockForDisplay(contentBlockSaveRequest);

            // Act
            var response = await subscriberContentBlockClient.GetContentBlockForDisplayWithPriceSeriesOnly(ContentBlockId, Today).GetRawResponse();

            // Assert
            response.MatchSnapshot();
        }

        [Fact]
        public async Task Return_Null_When_Price_Series_Ids_Is_Null()
        {
            // Arrange
            var contentBlockSaveRequest = new ContentBlockForDisplayInput
            {
                ContentBlockId = ContentBlockId,
                Title = "title",
                Columns =
                [
                    new ColumnForDisplayInput { Field = "field", DisplayOrder = 1, Hidden = false }
                ],
                Rows =
                [

                    new RowForDisplayInput
                    {
                        PriceSeriesId = string.Empty,
                        DisplayOrder = 0,
                        SeriesItemTypeCode = SeriesItemTypeCode.SingleValueSeries.Value
                    },
                    new RowForDisplayInput
                    {
                        PriceSeriesId = string.Empty,
                        DisplayOrder = 1,
                        SeriesItemTypeCode = SeriesItemTypeCode.RangeSeries.Value
                    }
                ]
            };
            _ = await authoringContentBlockClient.SaveContentBlockForDisplay(contentBlockSaveRequest);

            // Act
            var response = await subscriberContentBlockClient.GetContentBlockForDisplayWithPriceSeriesOnly(ContentBlockId, Today).GetRawResponse();

            // Assert
            response.MatchSnapshot();
        }

        [Fact]
        public async Task Return_Published_Price_Series_For_A_Previous_Date_When_Multiple_Price_Series_Are_Saved_For_Today_And_Previous_Dates()
        {
            // Arrange
            var today = Today.FromUnixTimeMilliseconds();
            var yesterday = today.AddDays(-1);

            var contentBlockSaveRequest = new ContentBlockForDisplayInput
            {
                ContentBlockId = ContentBlockId,
                Title = "title",
                Columns =
                [
                    new ColumnForDisplayInput { Field = "field", DisplayOrder = 1, Hidden = false }
                ],
                Rows =
                [
                    new RowForDisplayInput
                    {
                        PriceSeriesId = TestSeries.Melamine_China_Spot_FOB,
                        DisplayOrder = 0,
                        SeriesItemTypeCode = SeriesItemTypeCode.RangeSeries.Value
                    },
                    new RowForDisplayInput
                    {
                        PriceSeriesId = TestSeries.Melamine_China_Spot_Cif_2_6_Weeks,
                        DisplayOrder = 1,
                        SeriesItemTypeCode = SeriesItemTypeCode.RangeSeries.Value
                    }
                ]
            };
            _ = await authoringContentBlockClient.SaveContentBlockForDisplay(contentBlockSaveRequest);

            var rangeSeriesItemOne = RangePriceSeriesItemBuilder.BuildDefaultValidSeriesItem();
            rangeSeriesItemOne.PriceHigh = 50;

            var rangeSeriesItemTwo = RangePriceSeriesItemBuilder.BuildDefaultValidSeriesItem();
            rangeSeriesItemTwo.PriceHigh = 30;

            var workflow = new PriceEntryBusinessFlow(ContentBlockId, [TestSeries.Melamine_China_Spot_FOB], AuthoringHttpClient)
              .SaveContentBlockDefinition()
              .SaveRangePriceSeriesItem(TestSeries.Melamine_China_Spot_FOB, yesterday, rangeSeriesItemOne)
              .PublishWithAdvanceWorkflow(yesterday)
              .SaveRangePriceSeriesItem(TestSeries.Melamine_China_Spot_FOB, today, rangeSeriesItemTwo)
              .PublishWithAdvanceWorkflow(today);
            await workflow.Execute();

            var melamineWorkflow = new PriceEntryBusinessFlow("contentBlockId1", [TestSeries.Melamine_China_Spot_Cif_2_6_Weeks], AuthoringHttpClient)
            .SaveContentBlockDefinition()
            .SaveRangePriceSeriesItem(TestSeries.Melamine_China_Spot_Cif_2_6_Weeks, today, rangeSeriesItemTwo)
            .PublishWithAdvanceWorkflow(today);
            await melamineWorkflow.Execute();

            // Act
            var response = await subscriberContentBlockClient.GetContentBlockForDisplayWithPriceSeriesOnly(ContentBlockId, yesterday.ToUnixTimeMilliseconds()).GetRawResponse();

            // Assert
            response.MatchSnapshotWithLastModifiedDate();
        }

        [Fact]
        public async Task Return_Latest_Published_Price_Series_When_Multiple_Price_Series_Are_Saved_For_Today_And_Previous_Dates()
        {
            // Arrange
            var today = Today.FromUnixTimeMilliseconds();
            var yesterday = today.AddDays(-1);

            var contentBlockSaveRequest = new ContentBlockForDisplayInput
            {
                ContentBlockId = ContentBlockId,
                Title = "title",
                Columns =
                [
                    new ColumnForDisplayInput { Field = "field", DisplayOrder = 1, Hidden = false }
                ],
                Rows =
                [
                    new RowForDisplayInput
                    {
                        PriceSeriesId = TestSeries.Melamine_China_Spot_FOB,
                        DisplayOrder = 0,
                        SeriesItemTypeCode = SeriesItemTypeCode.RangeSeries.Value
                    },
                    new RowForDisplayInput
                    {
                        PriceSeriesId = TestSeries.Melamine_China_Spot_Cif_2_6_Weeks,
                        DisplayOrder = 1,
                        SeriesItemTypeCode = SeriesItemTypeCode.RangeSeries.Value
                    }
                ],
                SelectedFilters = new SelectedFiltersForDisplayInput()
                {
                    SelectedCommodities = ["Melamine"],
                    SelectedRegions = null,
                    SelectedTransactionTypes = null,
                    SelectedPriceCategories = null,
                    SelectedAssessedFrequencies = null,
                    IsInactiveIncluded = false,
                }
            };
            _ = await authoringContentBlockClient.SaveContentBlockForDisplay(contentBlockSaveRequest);

            var rangeSeriesItemOne = RangePriceSeriesItemBuilder.BuildDefaultValidSeriesItem();
            rangeSeriesItemOne.PriceHigh = 50;

            var rangeSeriesItemTwo = RangePriceSeriesItemBuilder.BuildDefaultValidSeriesItem();
            rangeSeriesItemTwo.PriceHigh = 30;

            var workflow = new PriceEntryBusinessFlow(ContentBlockId, [TestSeries.Melamine_China_Spot_FOB], AuthoringHttpClient)
              .SaveContentBlockDefinition()
              .SaveRangePriceSeriesItem(TestSeries.Melamine_China_Spot_FOB, yesterday, rangeSeriesItemOne)
              .PublishWithAdvanceWorkflow(yesterday)
              .SaveRangePriceSeriesItem(TestSeries.Melamine_China_Spot_FOB, today, rangeSeriesItemTwo)
              .PublishWithAdvanceWorkflow(today);
            await workflow.Execute();

            var melamineWorkflow = new PriceEntryBusinessFlow("contentBlockId1", [TestSeries.Melamine_China_Spot_Cif_2_6_Weeks], AuthoringHttpClient)
            .SaveContentBlockDefinition()
            .SaveRangePriceSeriesItem(TestSeries.Melamine_China_Spot_Cif_2_6_Weeks, today, rangeSeriesItemTwo)
            .PublishWithAdvanceWorkflow(today);
            await melamineWorkflow.Execute();

            // Act
            var response = await subscriberContentBlockClient.GetContentBlockForDisplayWithPriceSeriesOnly(ContentBlockId, today.ToUnixTimeMilliseconds()).GetRawResponse();

            // Assert
            response.MatchSnapshotWithLastModifiedDate();
        }

        [Fact]
        public async Task Return_Corrected_Price_Series_Details_When_Price_Is_Corrected_And_Published()
        {
            // Arrange
            var contentBlockSaveRequest = new ContentBlockForDisplayInput
            {
                ContentBlockId = ContentBlockId,
                Title = "title",
                Columns =
                [
                     new ColumnForDisplayInput { Field = "field", DisplayOrder = 1, Hidden = false }
                ],
                Rows =
                [
                    new RowForDisplayInput { PriceSeriesId = TestSeries.Melamine_China_Spot_Cif_2_6_Weeks, DisplayOrder = 0, SeriesItemTypeCode = SeriesItemTypeCode.Range },
                ],
                SelectedFilters = new SelectedFiltersForDisplayInput()
                {
                    SelectedCommodities = ["Melamine"],
                    SelectedRegions = ["China"],
                    SelectedTransactionTypes = null,
                    SelectedPriceCategories = null,
                    SelectedAssessedFrequencies = null,
                    IsInactiveIncluded = false,
                }
            };

            _ = await authoringContentBlockClient.SaveContentBlockForDisplay(contentBlockSaveRequest);

            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, new List<string> { TestSeries.Melamine_China_Spot_Cif_2_6_Weeks }, AuthoringHttpClient)
                .SaveContentBlockDefinition()
                .SaveRangePriceSeriesItem(TestSeries.Melamine_China_Spot_Cif_2_6_Weeks, AssessedDateTime, RangePriceSeriesItemBuilder.BuildDefaultValidSeriesItem())
                .PublishWithAdvanceWorkflow(AssessedDateTime)
                .InitiateCorrection(AssessedDateTime)
                .SavePriceSeriesItem(TestSeries.Melamine_China_Spot_Cif_2_6_Weeks, AssessedDateTime, SeriesItemTypeCode.RangeSeries, new SeriesItem { PriceLow = 10, PriceHigh = 50 }, "correction")
                .AdvancedCorrectionInitiateSendForReview(AssessedDateTime)
                .AdvancedCorrectionAcknowledgeSendForReview(AssessedDateTime)
                .AdvancedCorrectionInitiateStartReview(AssessedDateTime)
                .AdvancedCorrectionAcknowledgeStartReview(AssessedDateTime)
                .AdvancedCorrectionInitiateApproval(AssessedDateTime)
                .AdvancedCorrectionAcknowledgeApproval(AssessedDateTime)
                .AdvancedCorrectionAcknowledgePublished(AssessedDateTime);

            await businessFlow.Execute();

            // Act
            var response = await subscriberContentBlockClient.GetContentBlockForDisplayWithPriceSeriesOnly(ContentBlockId, Today).GetRawResponse();

            // Assert
            response.MatchSnapshotWithLastModifiedDate();
        }

        [Fact]
        public async Task Return_Original_Price_Series_Details_When_Price_Is_Corrected_But_Not_Published()
        {
            // Arrange
            var contentBlockSaveRequest = new ContentBlockForDisplayInput
            {
                ContentBlockId = ContentBlockId,
                Title = "title",
                Columns =
                [
                     new ColumnForDisplayInput { Field = "field", DisplayOrder = 1, Hidden = false }
                ],
                Rows =
                [
                    new RowForDisplayInput { PriceSeriesId = TestSeries.Melamine_China_Spot_Cif_2_6_Weeks, DisplayOrder = 0, SeriesItemTypeCode = SeriesItemTypeCode.Range },
                ],
                SelectedFilters = new SelectedFiltersForDisplayInput()
                {
                    SelectedCommodities = ["Melamine"],
                    SelectedRegions = ["China"],
                    SelectedTransactionTypes = null,
                    SelectedPriceCategories = null,
                    SelectedAssessedFrequencies = null,
                    IsInactiveIncluded = false,
                }
            };

            _ = await authoringContentBlockClient.SaveContentBlockForDisplay(contentBlockSaveRequest);

            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, new List<string> { TestSeries.Melamine_China_Spot_Cif_2_6_Weeks }, AuthoringHttpClient)
                .SaveContentBlockDefinition()
                .SaveRangePriceSeriesItem(TestSeries.Melamine_China_Spot_Cif_2_6_Weeks, AssessedDateTime, RangePriceSeriesItemBuilder.BuildDefaultValidSeriesItem())
                .PublishWithAdvanceWorkflow(AssessedDateTime)
                .InitiateCorrection(AssessedDateTime)
                .SavePriceSeriesItem(TestSeries.Melamine_China_Spot_Cif_2_6_Weeks, AssessedDateTime, SeriesItemTypeCode.RangeSeries, new SeriesItem { PriceLow = 10, PriceHigh = 50 }, "correction")
                .AdvancedCorrectionInitiateSendForReview(AssessedDateTime)
                .AdvancedCorrectionAcknowledgeSendForReview(AssessedDateTime)
                .AdvancedCorrectionInitiateStartReview(AssessedDateTime)
                .AdvancedCorrectionAcknowledgeStartReview(AssessedDateTime);

            await businessFlow.Execute();

            // Act
            var response = await subscriberContentBlockClient.GetContentBlockForDisplayWithPriceSeriesOnly(ContentBlockId, Today).GetRawResponse();

            // Assert
            response.MatchSnapshotWithLastModifiedDate();
        }

        [Fact]
        public async Task Return_Range_Price_Series_Detail_With_Adjusted_Delta_When_High_And_Low_Delta_Is_Adjusted_And_Published()
        {
            // Arrange
            var contentBlockSaveRequest = new ContentBlockForDisplayInput
            {
                ContentBlockId = ContentBlockId,
                Title = "title",
                Columns =
                [
                     new ColumnForDisplayInput { Field = "field", DisplayOrder = 1, Hidden = false }
                ],
                Rows =
                [
                    new RowForDisplayInput { PriceSeriesId = TestSeries.Melamine_China_Spot_Cif_2_6_Weeks, DisplayOrder = 0, SeriesItemTypeCode = SeriesItemTypeCode.Range },
                ]
            };

            _ = await authoringContentBlockClient.SaveContentBlockForDisplay(contentBlockSaveRequest);

            var itemAfterAdjustment = RangePriceSeriesItemBuilder.SetNonMarketAdjustmentValues(RangePriceSeriesItemBuilder.BuildDefaultValidSeriesItem());

            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, new List<string> { TestSeries.Melamine_China_Spot_Cif_2_6_Weeks }, AuthoringHttpClient)
                .SaveContentBlockDefinition()
                .SaveRangePriceSeriesItem(TestSeries.Melamine_China_Spot_Cif_2_6_Weeks, AssessedDateTime, RangePriceSeriesItemBuilder.BuildDefaultValidSeriesItem())
                .StartNonMarketAdjustment(AssessedDateTime)
                .SaveRangePriceSeriesItem(TestSeries.Melamine_China_Spot_Cif_2_6_Weeks, AssessedDateTime, itemAfterAdjustment)
                .PublishWithAdvanceWorkflow(AssessedDateTime);
            await businessFlow.Execute();

            // Act
            var response = await subscriberContentBlockClient.GetContentBlockForDisplayWithPriceSeriesOnly(ContentBlockId, Today).GetRawResponse();

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
            var contentBlockSaveRequest = new ContentBlockForDisplayInput
            {
                ContentBlockId = ContentBlockId,
                Title = "title",
                Columns =
                [
                    new ColumnForDisplayInput { Field = "field", DisplayOrder = 1, Hidden = false }
                ],
                Rows =
                [
                    new RowForDisplayInput
                    {
                        PriceSeriesId = TestSeries.LNG_Argentina_Month_1,
                        DisplayOrder = 0,
                        SeriesItemTypeCode = SeriesItemTypeCode.SingleValue
                    }
                ]
            };

            _ = await authoringContentBlockClient.SaveContentBlockForDisplay(contentBlockSaveRequest);

            var itemAfterAdjustment = SingleValuePriceSeriesItemBuilder.SetNonMarketAdjustmentValues(SingleValuePriceSeriesItemBuilder.BuildDefaultValidSeriesItem());

            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, new List<string> { TestSeries.LNG_Argentina_Month_1 }, AuthoringHttpClient)
                .SaveContentBlockDefinition()
                .SaveSingleValuePriceSeriesItem(TestSeries.LNG_Argentina_Month_1, AssessedDateTime, SingleValuePriceSeriesItemBuilder.BuildDefaultValidSeriesItem())
                .StartNonMarketAdjustment(AssessedDateTime)
                .SaveSingleValuePriceSeriesItem(TestSeries.LNG_Argentina_Month_1, AssessedDateTime, itemAfterAdjustment)
                .PublishWithAdvanceWorkflow(AssessedDateTime);
            await businessFlow.Execute();

            // Act
            var response = await subscriberContentBlockClient.GetContentBlockForDisplayWithPriceSeriesOnly(ContentBlockId, Today).GetRawResponse();

            // Assert
            response.MatchSnapshotForAdjustedDeltaSinglePrice(
                TestData.NonMarketAdjustment,
                itemAfterAdjustment.AdjustedPriceDelta);
        }
    }
}
