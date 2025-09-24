namespace Authoring.Tests.Application.PriceEntry.Mutation.CharterRateSingleValue
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
    public class SaveCharterRateSingleValuePriceEntryDataShould : WebApplicationTestBase
    {
        private const string ContentBlockId = "contentBlockId";

        private const string PriceSeriesId = TestSeries.CharterRates_Pacific_Prompt_Two_Stroke;

        private static readonly DateTime Now = UtcDateTime.GetUtcDateTime(new DateTime(2024, 10, 21));

        private readonly TestClock testClock;

        private readonly ContentBlockGraphQLClient contentBlockClient;

        public SaveCharterRateSingleValuePriceEntryDataShould(AuthoringBffApplicationFactory factory)
            : base(factory)
        {
            contentBlockClient = new ContentBlockGraphQLClient(GraphQLClient);
            testClock = factory.Services.GetRequiredService<TestClock>();
            testClock.Reset();
        }

        [Fact]
        public async Task Save_Price_Entry_Data()
        {
            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, new List<string> { PriceSeriesId }, HttpClient)
               .SaveContentBlockDefinition()
               .SaveCharterRateSingleValuePriceSeriesItem(PriceSeriesId, Now, new SeriesItem { Price = 100, DataUsed = "Interpolation/extrapolation" });
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
               .SaveCharterRateSingleValuePriceSeriesItem(PriceSeriesId, Now, new SeriesItem { Price = 100, DataUsed = "Interpolation/extrapolation" });

            var response = await businessFlow.Execute().GetRawResponse();

            response.MatchSnapshotForPriceEntryUpdate();
        }

        [Fact]
        public async Task Verify_Updated_Data()
        {
            var seriesItem = new SeriesItem { Price = 100, DataUsed = "Interpolation/extrapolation" };

            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, new List<string> { PriceSeriesId }, HttpClient)
               .SaveContentBlockDefinition()
               .SaveCharterRateSingleValuePriceSeriesItem(PriceSeriesId, Now, seriesItem);
            await businessFlow.Execute();

            seriesItem.Price = 200;

            businessFlow.SaveCharterRateSingleValuePriceSeriesItem(PriceSeriesId, Now, seriesItem);
            await businessFlow.Execute();

            var response = await contentBlockClient
                              .GetContentBlockWithPriceSeriesOnly(ContentBlockId, Now)
                              .GetRawResponse();

            response.MatchSnapshotWithSeriesItemId();
        }

        [Fact]
        public async Task Calculate_The_Price_Delta_Based_On_Previous_Price()
        {
            // arrange
            var yesterday = Now.AddDays(-1);

            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, new List<string> { PriceSeriesId }, HttpClient)
                .SaveContentBlockDefinition()
                .SaveCharterRateSingleValuePriceSeriesItem(PriceSeriesId, yesterday, new SeriesItem { Price = 100, DataUsed = "Interpolation/extrapolation" })
                .InitiatePublish(yesterday)
                .AcknowledgePublished(yesterday);
            await businessFlow.Execute();

            // act
            businessFlow.SaveCharterRateSingleValuePriceSeriesItem(PriceSeriesId, Now, new SeriesItem { Price = 200, DataUsed = "Interpolation/extrapolation" });
            await businessFlow.Execute();

            // assert
            var response = await contentBlockClient
                              .GetContentBlockWithPriceSeriesOnly(ContentBlockId, Now)
                              .GetRawResponse();

            response.MatchSnapshotWithSeriesItemId();
        }

        [Fact]
        public async Task Save_Price_Delta_Type()
        {
            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, new List<string> { PriceSeriesId }, HttpClient)
               .SaveContentBlockDefinition()
               .SaveCharterRateSingleValuePriceSeriesItem(PriceSeriesId, Now, new SeriesItem { Price = 100, DataUsed = "Interpolation/extrapolation" });

            var response = await businessFlow.Execute().GetRawResponse();

            response.MatchSnapshotForPriceEntryUpdate();
        }
    }
}
