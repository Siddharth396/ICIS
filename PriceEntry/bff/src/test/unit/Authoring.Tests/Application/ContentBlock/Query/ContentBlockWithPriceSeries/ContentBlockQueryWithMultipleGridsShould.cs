namespace Authoring.Tests.Application.ContentBlock.Query.ContentBlockWithPriceSeries
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Authoring.Tests.Functional.Tests.Common.Infrastructure;
    using global::BusinessLayer.PriceEntry.ValueObjects;

    using Snapshooter;
    using Snapshooter.Xunit;

    using Test.Infrastructure.Authoring;
    using Test.Infrastructure.Authoring.BusinessFlows.PriceEntryBusinessFlow;
    using Test.Infrastructure.Authoring.BusinessFlows.PriceEntryBusinessFlow.PriceSeriesItemBuilders;
    using Test.Infrastructure.Authoring.GraphQLClients.PriceEntry;
    using Test.Infrastructure.Extensions;
    using Test.Infrastructure.Stubs;
    using Test.Infrastructure.TestData;

    using Xunit;

    [Collection(WebApplicationCollection.Name)]
    public class ContentBlockQueryWithMultipleGridsShould : WebApplicationTestBase
    {
        protected const string ContentBlockId = "contentBlockId";

        protected static readonly DateTime Today = TestData.Now;

        protected static readonly DateTime Tomorrow = TestData.Now.AddDays(1);

        public ContentBlockQueryWithMultipleGridsShould(AuthoringBffApplicationFactory factory)
            : base(factory)
        {
            ContentBlockClient = new ContentBlockGraphQLClient(GraphQLClient);
            var testClock = GetService<TestClock>();
            testClock.SetUtcNow(Today);
        }

        protected ContentBlockGraphQLClient ContentBlockClient { get; }

        private static string[] LNGHalfMonthAssessedSeries => new[]
        {
            TestSeries.LNG_China_HM1,
            TestSeries.LNG_China_HM2,
            TestSeries.LNG_China_HM3,
            TestSeries.LNG_China_HM4,
            TestSeries.LNG_China_HM5,
            TestSeries.LNG_Japan_HM1,
            TestSeries.LNG_Japan_HM2,
            TestSeries.LNG_Japan_HM3,
            TestSeries.LNG_Japan_HM4,
            TestSeries.LNG_Japan_HM5,
            TestSeries.LNG_South_Korea_HM1,
            TestSeries.LNG_South_Korea_HM2,
            TestSeries.LNG_South_Korea_HM3,
            TestSeries.LNG_South_Korea_HM4,
            TestSeries.LNG_South_Korea_HM5,
            TestSeries.LNG_Taiwan_HM1,
            TestSeries.LNG_Taiwan_HM2,
            TestSeries.LNG_Taiwan_HM3,
            TestSeries.LNG_Taiwan_HM4,
            TestSeries.LNG_Taiwan_HM5
        };

        private static string[] LNGDerivedFullMonthSeries => new[]
        {
            TestSeries.LNG_China_MM1,
            TestSeries.LNG_China_MM2,
            TestSeries.LNG_Japan_MM1,
            TestSeries.LNG_Japan_MM2,
            TestSeries.LNG_South_Korea_MM1,
            TestSeries.LNG_South_Korea_MM2,
            TestSeries.LNG_Taiwan_MM1,
            TestSeries.LNG_Taiwan_MM2
        };

        private static string[] LNGDerivedEaxHalfMonthIndices => new[]
        {
            TestSeries.LNG_EAX_Index_HM2,
            TestSeries.LNG_EAX_Index_HM3,
            TestSeries.LNG_EAX_Index_HM4,
            TestSeries.LNG_EAX_Index_HM5,
            TestSeries.LNG_EAX_Index_HM6
        };

        private static string[] LNGDerivedEaxFullMonthIndices => new[]
        {
            TestSeries.LNG_EAX_Index_MM1,
            TestSeries.LNG_EAX_Index_MM2
        };

        [Fact]
        public async Task Return_Empty_ValidationErrors_When_No_Price_Data_Is_Entered()
        {
            var priceSeriesIds = GetPriceSeriesIds();

            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, priceSeriesIds, HttpClient)
               .SaveContentBlockDefinition();

            _ = await businessFlow.Execute();

            var response = await ContentBlockClient
                              .GetContentBlockWithPriceSeriesValidationErrorsOnly(ContentBlockId, Today)
                              .GetRawResponse();

            response.MatchSnapshot();
        }

        [Fact]
        public async Task Return_Empty_ValidationErrors_When_Price_Is_Valid()
        {
            var priceSeriesIds = GetPriceSeriesIds();

            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, priceSeriesIds, HttpClient)
               .SaveContentBlockDefinition();

            foreach (var priceSeriesId in LNGHalfMonthAssessedSeries)
            {
                businessFlow.SavePriceSeriesItem(
                    priceSeriesId,
                    Today,
                    SeriesItemTypeCode.SingleValueWithReferenceSeries,
                    SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice());
            }

            await businessFlow.Execute();

            var response = await ContentBlockClient
                              .GetContentBlockWithPriceSeriesValidationErrorsOnly(ContentBlockId, Today)
                              .GetRawResponse();

            response.MatchSnapshot();
        }

        [Fact]
        public async Task Return_Empty_ValidationErrors_When_Status_Is_ReadyToStart()
        {
            var priceSeriesIds = GetPriceSeriesIds();

            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, priceSeriesIds, HttpClient)
               .SaveContentBlockDefinition();

            _ = await businessFlow.Execute();

            var response = await ContentBlockClient
                              .GetContentBlockWithPriceSeriesValidationErrorsOnly(ContentBlockId, Today)
                              .GetRawResponse();

            response.MatchSnapshot();
        }

        [Fact]
        public async Task Return_Price_Series_From_Date()
        {
            var priceSeriesIds = GetPriceSeriesIds();

            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, priceSeriesIds, HttpClient)
               .SaveContentBlockDefinition();

            _ = await businessFlow.Execute();

            var response = await ContentBlockClient.GetContentBlockWithPriceSeriesOnly(ContentBlockId, Today)
                              .GetRawResponse();

            response.MatchSnapshot();
        }

        [Fact]
        public async Task Return_ValidationErrors_When_Price_Is_Not_Valid()
        {
            var priceSeriesIds = GetPriceSeriesIds();
            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, priceSeriesIds, HttpClient)
               .SaveContentBlockDefinition();

            foreach (var priceSeriesId in LNGHalfMonthAssessedSeries)
            {
                businessFlow.SavePriceSeriesItem(
                    priceSeriesId,
                    Today,
                    SeriesItemTypeCode.SingleValueWithReferenceSeries,
                    SingleValueWithReferencePriceSeriesItemBuilder.BuildInvalidSeriesItem());
            }

            _ = await businessFlow.Execute();

            var response = await ContentBlockClient
                              .GetContentBlockWithPriceSeriesValidationErrorsOnly(ContentBlockId, Today)
                              .GetRawResponse();

            response.MatchSnapshot();
        }

        [Fact]
        public async Task Return_ValidationErrors_When_Status_Is_ReadyToStart_And_All_Data_Should_Be_Validated()
        {
            var priceSeriesIds = GetPriceSeriesIds();
            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, priceSeriesIds, HttpClient)
               .SaveContentBlockDefinition();

            _ = await businessFlow.Execute();

            var response = await ContentBlockClient
                              .GetContentBlockWithPriceSeriesValidationErrorsOnly(ContentBlockId, Today, includeNotStarted: true)
                              .GetRawResponse();

            response.MatchSnapshot();
        }

        [Fact]
        public async Task Return_ReadOnly_Price_Series_When_Status_Is_PUBLISHED()
        {
            var priceSeriesIds = GetPriceSeriesIds();

            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, priceSeriesIds, HttpClient)
               .SaveContentBlockDefinition();

            foreach (var priceSeriesId in LNGHalfMonthAssessedSeries)
            {
                businessFlow.SavePriceSeriesItem(
                    priceSeriesId,
                    Today,
                    SeriesItemTypeCode.SingleValueWithReferenceSeries,
                    SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice());
            }

            businessFlow
               .InitiatePublish(Today)
               .AcknowledgePublished(Today);

            await businessFlow.Execute();

            var contentBlockWithPriceSeriesResponse = await ContentBlockClient
                                                         .GetContentBlockWithPriceSeriesOnly(ContentBlockId, Today)
                                                         .GetRawResponse();

            contentBlockWithPriceSeriesResponse.MatchSnapshotWithSeriesItemId();
        }

        [Fact]
        public async Task Return_Same_Published_Price_Series_When_Content_Block_Definition_Changes()
        {
            var priceSeriesIds = GetPriceSeriesIds();

            var seriesToRemove = new[]
            {
                TestSeries.LNG_China_HM5,
                TestSeries.LNG_China_MM2,
                TestSeries.LNG_EAX_Index_HM6,
                TestSeries.LNG_EAX_Index_MM2
            };

            foreach (var list in priceSeriesIds)
            {
                foreach (var series in seriesToRemove)
                {
                    list.Remove(series);
                }
            }

            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, priceSeriesIds, HttpClient)
               .SaveContentBlockDefinition();

            foreach (var priceSeriesId in LNGHalfMonthAssessedSeries.Where(x => x != TestSeries.LNG_China_HM5))
            {
                businessFlow.SavePriceSeriesItem(
                    priceSeriesId,
                    Today,
                    SeriesItemTypeCode.SingleValueWithReferenceSeries,
                    SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice());
            }

            businessFlow
               .InitiatePublish(Today)
               .AcknowledgePublished(Today)
               .AddSeriesIdToGrid(0, TestSeries.LNG_China_HM5)
               .AddSeriesIdToGrid(1, TestSeries.LNG_China_MM2)
               .AddSeriesIdToGrid(2, TestSeries.LNG_EAX_Index_HM6)
               .AddSeriesIdToGrid(3, TestSeries.LNG_EAX_Index_MM2)
               .SaveContentBlockDefinition();

            _ = await businessFlow.Execute();

            var result = await ContentBlockClient
                            .GetContentBlockWithPriceSeriesOnly(ContentBlockId, Today)
                            .GetRawResponse();

            var nextDayResult = await ContentBlockClient
                                   .GetContentBlockWithPriceSeriesOnly(ContentBlockId, Tomorrow)
                                   .GetRawResponse();

            result.MatchSnapshotWithSeriesItemId();

            // from next day onwards it should return based on the new content block definition
            nextDayResult.MatchSnapshot(SnapshotNameExtension.Create("nextDay"));
        }

        [Fact]
        public async Task Return_Validation_Error_When_Some_Series_Are_Invalid_While_Publishing()
        {
            var priceSeriesIds = GetPriceSeriesIds();

            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, priceSeriesIds, HttpClient)
               .SaveContentBlockDefinition()
               .SavePriceSeriesItem(
                    TestSeries.LNG_China_HM1,
                    Today,
                    SeriesItemTypeCode.SingleValueWithReferenceSeries,
                    SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice())
               .InitiatePublish(Today);

            var result = await businessFlow.Execute().GetRawResponse();

            result.MatchSnapshotWithSeriesItemId();
        }

        [Fact]
        public async Task Return_Price_Series_With_NonMarketAdjustments()
        {
            var priceSeriesIds = GetPriceSeriesIds();

            var seriesItem = SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice();
            seriesItem.AdjustedPriceDelta = 10.1m;

            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, priceSeriesIds, HttpClient)
               .SaveContentBlockDefinition()
               .StartNonMarketAdjustment(Today)
               .SavePriceSeriesItem(
                    TestSeries.LNG_China_HM1,
                    Today,
                    SeriesItemTypeCode.SingleValueWithReferenceSeries,
                    seriesItem);

            _ = await businessFlow.Execute();

            var response = await ContentBlockClient.GetContentBlockWithPriceSeriesOnly(ContentBlockId, Today)
                              .GetRawResponse();
            response.MatchSnapshotWithSeriesItemId();
        }

        [Fact]
        public async Task Reset_NonMarketAdjustments_Fields_When_Cancelling_It()
        {
            var priceSeriesIds = GetPriceSeriesIds();

            var seriesItem = SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice();
            seriesItem.AdjustedPriceDelta = 10.1m;

            var businessFlow =
                new PriceEntryBusinessFlow(ContentBlockId, priceSeriesIds, HttpClient)
                   .SaveContentBlockDefinition()
                   .StartNonMarketAdjustment(Today)
                   .SavePriceSeriesItem(
                        TestSeries.LNG_China_HM1,
                        Today,
                        SeriesItemTypeCode.SingleValueWithReferenceSeries,
                        seriesItem)
                   .CancelNonMarketAdjustment(Today);

            _ = await businessFlow.Execute();

            var response = await ContentBlockClient.GetContentBlockWithPriceSeriesOnly(ContentBlockId, Today)
                              .GetRawResponse();
            response.MatchSnapshotWithSeriesItemId();
        }

        private static ICollection<ICollection<string>> GetPriceSeriesIds()
        {
            return ContentBlockQueryHelper.CreatePriceSeriesIds(
            [
                LNGHalfMonthAssessedSeries,
                LNGDerivedFullMonthSeries,
                LNGDerivedEaxHalfMonthIndices,
                LNGDerivedEaxFullMonthIndices
            ]);
        }
    }
}
