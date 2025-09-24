namespace Authoring.Tests.Application.PriceEntry.Mutation.Range
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Authoring.Tests.Functional.Tests.Common.Infrastructure;

    using global::BusinessLayer.Helpers;
    using global::BusinessLayer.PriceEntry.DTOs.Input;

    using Microsoft.Extensions.DependencyInjection;

    using Test.Infrastructure.Authoring;
    using Test.Infrastructure.Authoring.BusinessFlows.PriceEntryBusinessFlow;
    using Test.Infrastructure.Authoring.GraphQLClients.PriceEntry;
    using Test.Infrastructure.Extensions;
    using Test.Infrastructure.Stubs;
    using Test.Infrastructure.TestData;

    using Xunit;

    [Collection(WebApplicationCollection.Name)]
    public class SaveRangePriceEntryDataShould : WebApplicationTestBase
    {
        private const string ContentBlockId = "contentBlockId";

        private const string PriceSeriesId = TestSeries.Melamine_Asia_SE_Spot;

        private static readonly DateTime Now = UtcDateTime.GetUtcDateTime(new DateTime(2023, 12, 17));

        private readonly TestClock testClock;

        private readonly ContentBlockGraphQLClient contentBlockClient;

        public SaveRangePriceEntryDataShould(AuthoringBffApplicationFactory factory)
            : base(factory)
        {
            contentBlockClient = new ContentBlockGraphQLClient(GraphQLClient);
            testClock = factory.Services.GetRequiredService<TestClock>();
            testClock.Reset();
        }

        [Fact]
        public async Task Save_The_Price_Entry_Data()
        {
            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, new List<string> { PriceSeriesId }, HttpClient)
                .SaveContentBlockDefinition()
                .SaveRangePriceSeriesItem(PriceSeriesId, Now, new SeriesItem { PriceLow = 50, PriceHigh = 100 });
            await businessFlow.Execute();

            var response = await contentBlockClient
                              .GetContentBlockWithPriceSeriesOnly(ContentBlockId, Now)
                              .GetRawResponse();

            response.MatchSnapshotWithSeriesItemId();
        }

        [Fact]
        public async Task Save_The_Price_Entry_Data_With_Absolute_Period()
        {
            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, new List<string> { PriceSeriesId }, HttpClient)
                .SaveContentBlockDefinition()
                .SaveRangePriceSeriesItem(PriceSeriesId, Now, new SeriesItem { PriceLow = 50, PriceHigh = 100 });

            var response = await businessFlow.Execute().GetRawResponse();

            response.MatchSnapshotForPriceEntryUpdate();
        }

        [Fact]
        public async Task Save_The_Price_Entry_Data_With_No_Absolute_Period()
        {
            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, new List<string> { TestSeries.Petchem_8602999 }, HttpClient)
                .SaveContentBlockDefinition()
                .SaveRangePriceSeriesItem(TestSeries.Petchem_8602999, Now, new SeriesItem { PriceLow = 50, PriceHigh = 100 });

            var response = await businessFlow.Execute().GetRawResponse();

            response.MatchSnapshotForPriceEntryUpdate();
        }

        [Fact]
        public async Task Verify_Price_Values_Are_Updated()
        {
            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, new List<string> { PriceSeriesId }, HttpClient)
                .SaveContentBlockDefinition()
                .SaveRangePriceSeriesItem(PriceSeriesId, Now, new SeriesItem { PriceLow = 50, PriceHigh = 100 })
                .SaveRangePriceSeriesItem(PriceSeriesId, Now, new SeriesItem { PriceLow = 55, PriceHigh = 60 });
            await businessFlow.Execute();

            var response = await contentBlockClient
                              .GetContentBlockWithPriceSeriesOnly(ContentBlockId, Now)
                              .GetRawResponse();

            response.MatchSnapshotWithSeriesItemId();
        }

        [Fact]
        public async Task Verify_MidPrice_Resets_When_A_Price_Is_Reset()
        {
            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, new List<string> { PriceSeriesId }, HttpClient)
               .SaveContentBlockDefinition()
               .SaveRangePriceSeriesItem(PriceSeriesId, Now, new SeriesItem { PriceLow = 50, PriceHigh = 100 })
               .SaveRangePriceSeriesItem(PriceSeriesId, Now, new SeriesItem { PriceLow = 50, PriceHigh = null });
            await businessFlow.Execute();

            var response = await contentBlockClient
                              .GetContentBlockWithPriceSeriesOnly(ContentBlockId, Now)
                              .GetRawResponse();

            response.MatchSnapshotWithSeriesItemId();
        }

        [Fact]
        public async Task Verify_Delta_Calculated_As_Positive_When_Previous_Price_Is_Available()
        {
            // arrange
            var yesterday = Now.AddDays(-1);

            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, new List<string> { PriceSeriesId }, HttpClient)
               .SaveContentBlockDefinition()
               .SaveRangePriceSeriesItem(PriceSeriesId, yesterday, new SeriesItem { PriceLow = 50, PriceHigh = 100 })
               .InitiatePublish(yesterday)
               .AcknowledgePublished(yesterday);
            await businessFlow.Execute();

            // act
            var today = yesterday.AddDays(1);

            businessFlow.SaveRangePriceSeriesItem(PriceSeriesId, today, new SeriesItem { PriceLow = 60, PriceHigh = 120 });
            await businessFlow.Execute();

            var response = await contentBlockClient
                              .GetContentBlockWithPriceSeriesOnly(ContentBlockId, today)
                              .GetRawResponse();

            response.MatchSnapshotWithSeriesItemId();
        }

        [Fact]
        public async Task Verify_Delta_Calculated_As_Negative_When_Previous_Price_Is_Available()
        {
            // arrange
            var yesterday = Now.AddDays(-1);

            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, new List<string> { PriceSeriesId }, HttpClient)
               .SaveContentBlockDefinition()
               .SaveRangePriceSeriesItem(PriceSeriesId, yesterday, new SeriesItem { PriceLow = 60, PriceHigh = 120 })
               .InitiatePublish(yesterday)
               .AcknowledgePublished(yesterday);
            await businessFlow.Execute();

            // act
            var today = yesterday.AddDays(1);

            businessFlow.SaveRangePriceSeriesItem(PriceSeriesId, today, new SeriesItem { PriceLow = 50, PriceHigh = 100 });
            await businessFlow.Execute();

            // assert
            var response = await contentBlockClient
                              .GetContentBlockWithPriceSeriesOnly(ContentBlockId, today)
                              .GetRawResponse();

            response.MatchSnapshotWithSeriesItemId();
        }

        [Fact]
        public async Task Save_Price_Delta_Type()
        {
            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, new List<string> { PriceSeriesId }, HttpClient)
               .SaveContentBlockDefinition()
               .SaveRangePriceSeriesItem(PriceSeriesId, Now, new SeriesItem { PriceLow = 50, PriceHigh = 100 });

            var response = await businessFlow.Execute().GetRawResponse();

            response.MatchSnapshotForPriceEntryUpdate();
        }
    }
}