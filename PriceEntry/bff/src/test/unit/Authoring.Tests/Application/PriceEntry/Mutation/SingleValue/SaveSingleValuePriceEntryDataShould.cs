namespace Authoring.Tests.Application.PriceEntry.Mutation.SingleValue
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Authoring.Tests.Functional.Tests.Common.Infrastructure;

    using global::BusinessLayer.Helpers;
    using global::BusinessLayer.PriceEntry.DTOs.Input;

    using global::Infrastructure.Services.PeriodGenerator;

    using Microsoft.Extensions.DependencyInjection;

    using Test.Infrastructure.Authoring;
    using Test.Infrastructure.Authoring.BusinessFlows.PriceEntryBusinessFlow;
    using Test.Infrastructure.Authoring.GraphQLClients.PriceEntry;
    using Test.Infrastructure.Extensions;
    using Test.Infrastructure.Stubs;
    using Test.Infrastructure.TestData;

    using Xunit;

    [Collection(WebApplicationCollection.Name)]
    public class SaveSingleValuePriceEntryDataShould : WebApplicationTestBase
    {
        private const string ContentBlockId = "contentBlockId";

        private const string PriceSeriesId = TestSeries.Petchem_8603620;

        private static readonly DateTime Now = UtcDateTime.GetUtcDateTime(new DateTime(2024, 3, 15));

        private readonly TestClock testClock;

        private readonly ContentBlockGraphQLClient contentBlockClient;

        public SaveSingleValuePriceEntryDataShould(AuthoringBffApplicationFactory factory)
            : base(factory)
        {
            contentBlockClient = new ContentBlockGraphQLClient(GraphQLClient);
            testClock = factory.Services.GetRequiredService<TestClock>();
            testClock.Reset();
        }

        [Fact]
        public async Task Save_Price_Entry_Data()
        {
            var periodGeneratorServiceStub = GetService<PeriodGeneratorServiceStub>();
            periodGeneratorServiceStub.ReplaceAbsolutePeriods(GetAbsolutePeriods());

            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, new List<string> { PriceSeriesId }, HttpClient)
               .SaveContentBlockDefinition()
               .SaveSingleValuePriceSeriesItem(PriceSeriesId, Now, new SeriesItem { Price = 100 });
            await businessFlow.Execute();

            var response = await contentBlockClient
                              .GetContentBlockWithPriceSeriesOnly(ContentBlockId, Now)
                              .GetRawResponse();

            response.MatchSnapshotWithSeriesItemId();

            periodGeneratorServiceStub.ClearAbsolutePeriods();
        }

        [Fact]
        public async Task Save_The_Price_Entry_Data_With_Absolute_Period()
        {
            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, new List<string> { PriceSeriesId }, HttpClient)
               .SaveContentBlockDefinition()
               .SaveSingleValuePriceSeriesItem(PriceSeriesId, Now, new SeriesItem { Price = 100 });

            var response = await businessFlow.Execute().GetRawResponse();

            response.MatchSnapshotForPriceEntryUpdate();
        }

        [Fact]
        public async Task Verify_Updated_Data()
        {
            var periodGeneratorServiceStub = GetService<PeriodGeneratorServiceStub>();
            periodGeneratorServiceStub.ReplaceAbsolutePeriods(GetAbsolutePeriods());

            var seriesItem = new SeriesItem { Price = 100 };

            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, new List<string> { PriceSeriesId }, HttpClient)
               .SaveContentBlockDefinition()
               .SaveSingleValuePriceSeriesItem(PriceSeriesId, Now, seriesItem);
            await businessFlow.Execute();

            seriesItem.Price = 200;

            businessFlow.SaveSingleValuePriceSeriesItem(PriceSeriesId, Now, seriesItem);
            await businessFlow.Execute();

            var response = await contentBlockClient
                              .GetContentBlockWithPriceSeriesOnly(ContentBlockId, Now)
                              .GetRawResponse();

            response.MatchSnapshotWithSeriesItemId();

            periodGeneratorServiceStub.ClearAbsolutePeriods();
        }

        [Fact]
        public async Task Calculate_The_Price_Delta_Based_On_Previous_Price()
        {
            // arrange
            var yesterday = Now.AddDays(-1);

            var periodGeneratorServiceStub = GetService<PeriodGeneratorServiceStub>();
            periodGeneratorServiceStub.ReplaceAbsolutePeriods(new()
            {
                new()
                {
                    Code = "M2404",
                    PeriodCode = "M",
                    Label = "Apr 2024",
                    FromDate = new DateOnly(2024, 04, 01),
                    UntilDate = new DateOnly(2024, 04, 30)
                }
            });

            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, new List<string> { PriceSeriesId }, HttpClient)
                .SaveContentBlockDefinition()
                .SaveSingleValuePriceSeriesItem(PriceSeriesId, yesterday, new SeriesItem { Price = 100 })
                .InitiatePublish(yesterday)
                .AcknowledgePublished(yesterday);
            await businessFlow.Execute();

            // act
            businessFlow.SaveSingleValuePriceSeriesItem(PriceSeriesId, Now, new SeriesItem { Price = 200 });
            await businessFlow.Execute();

            // assert
            var response = await contentBlockClient
                              .GetContentBlockWithPriceSeriesOnly(ContentBlockId, Now)
                              .GetRawResponse();

            response.MatchSnapshotWithSeriesItemId();

            periodGeneratorServiceStub.ClearAbsolutePeriods();
        }

        [Fact]
        public async Task Save_Price_Delta_Type()
        {
            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, new List<string> { PriceSeriesId }, HttpClient)
               .SaveContentBlockDefinition()
               .SaveSingleValuePriceSeriesItem(PriceSeriesId, Now, new SeriesItem { Price = 100 });

            var response = await businessFlow.Execute().GetRawResponse();

            response.MatchSnapshotForPriceEntryUpdate();
        }

        private List<AbsolutePeriod> GetAbsolutePeriods()
        {
            return new List<AbsolutePeriod>
            {
                new()
                {
                    Code = "M2403",
                    PeriodCode = "M",
                    Label = "Mar 2024",
                    FromDate = new DateOnly(2024, 03, 01),
                    UntilDate = new DateOnly(2024, 03, 31)
                }
            };
        }
    }
}