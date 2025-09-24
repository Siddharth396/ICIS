namespace Authoring.Tests.Application.ContentBlock.Query.ContentBlockWithPriceSeries
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Authoring.Tests.Functional.Tests.Common.Infrastructure;

    using FluentAssertions;

    using global::BusinessLayer.ContentBlock.DTOs.Input;
    using global::BusinessLayer.PriceEntry.DTOs.Input;
    using global::BusinessLayer.PriceEntry.ValueObjects;

    using Snapshooter;
    using Snapshooter.Xunit;

    using Test.Infrastructure.Authoring;
    using Test.Infrastructure.Authoring.BusinessFlows.PriceEntryBusinessFlow;
    using Test.Infrastructure.Authoring.GraphQLClients.PriceEntry;
    using Test.Infrastructure.Extensions;
    using Test.Infrastructure.Services;
    using Test.Infrastructure.Stubs;
    using Test.Infrastructure.TestData;

    using Xunit;

    public abstract class ContentBlockQueryWithPriceSeriesTestBase : WebApplicationTestBase
    {
        protected const string ContentBlockId = "contentBlockId";

        protected static readonly DateTime Today = TestData.Now;

        protected static readonly DateTime Tomorrow = TestData.Now.AddDays(1);

        protected ContentBlockQueryWithPriceSeriesTestBase(AuthoringBffApplicationFactory factory)
            : base(factory)
        {
            ContentBlockClient = new ContentBlockGraphQLClient(GraphQLClient);
            var testClock = GetService<TestClock>();
            testClock.SetUtcNow(Today);
        }

        protected ContentBlockGraphQLClient ContentBlockClient { get; }

        protected abstract string PriceSeriesId { get; }

        protected abstract string AlternativePriceSeriesId { get; }

        protected abstract SeriesItemTypeCode SeriesItemTypeCode { get; }

        [Fact]
        public async Task Return_No_Price_Series_When_None_Configured()
        {
            var priceSeriesIds = ContentBlockQueryHelper.CreatePriceSeriesIds();

            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, priceSeriesIds, HttpClient)
               .SaveContentBlockDefinition();

            _ = await businessFlow.Execute();

            var result = await ContentBlockClient
                            .GetContentBlock(ContentBlockId, Today)
                            .GetRawResponse();

            result.MatchSnapshot(SnapshotNameExtension.Create(SeriesItemTypeCode.Value));
        }

        [Fact]
        public async Task Return_Empty_ValidationErrors_When_No_Price_Data_Is_Entered()
        {
            await CreateDefaultContentBlockDefinition();

            var response = await ContentBlockClient
                              .GetContentBlockWithPriceSeriesValidationErrorsOnly(ContentBlockId, Today)
                              .GetRawResponse();

            response.MatchSnapshot(SnapshotNameExtension.Create(SeriesItemTypeCode.Value));
        }

        [Fact]
        public async Task Return_Empty_ValidationErrors_When_Price_Is_Valid()
        {
            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, new List<string> { PriceSeriesId }, HttpClient)
               .SaveContentBlockDefinition()
               .SavePriceSeriesItem(PriceSeriesId, Today, GetSeriesItemTypeCode(), BuildValidSeriesItem());

            await businessFlow.Execute();

            var response = await ContentBlockClient
                              .GetContentBlockWithPriceSeriesValidationErrorsOnly(ContentBlockId, Today)
                              .GetRawResponse();

            response.MatchSnapshot(SnapshotNameExtension.Create(SeriesItemTypeCode.Value));
        }

        [Fact]
        public async Task Return_Empty_ValidationErrors_When_Status_Is_ReadyToStart()
        {
            await CreateDefaultContentBlockDefinition();

            var response = await ContentBlockClient
                              .GetContentBlockWithPriceSeriesValidationErrorsOnly(ContentBlockId, Today)
                              .GetRawResponse();

            response.MatchSnapshot(SnapshotNameExtension.Create(SeriesItemTypeCode.Value));
        }

        [Fact]
        public async Task Return_Price_Series_From_Date()
        {
            await CreateDefaultContentBlockDefinition();

            var response = await ContentBlockClient.GetContentBlockWithPriceSeriesOnly(ContentBlockId, Today)
                              .GetRawResponse();

            response.MatchSnapshot(SnapshotNameExtension.Create(SeriesItemTypeCode.Value));
        }

        [Fact]
        public async Task Return_ValidationErrors_When_Price_Is_Not_Valid()
        {
            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, new List<string> { PriceSeriesId }, HttpClient)
               .SaveContentBlockDefinition()
               .SavePriceSeriesItem(PriceSeriesId, Today, GetSeriesItemTypeCode(), BuildInvalidSeriesItem());

            await businessFlow.Execute();

            var response = await ContentBlockClient
                              .GetContentBlockWithPriceSeriesValidationErrorsOnly(ContentBlockId, Today)
                              .GetRawResponse();

            response.MatchSnapshot(SnapshotNameExtension.Create(SeriesItemTypeCode.Value));
        }

        [Fact]
        public async Task Return_ValidationErrors_When_Status_Is_ReadyToStart_And_All_Data_Should_Be_Validated()
        {
            await CreateDefaultContentBlockDefinition();

            var response = await ContentBlockClient
                              .GetContentBlockWithPriceSeriesValidationErrorsOnly(ContentBlockId, Today, includeNotStarted: true)
                              .GetRawResponse();

            response.MatchSnapshot(SnapshotNameExtension.Create(SeriesItemTypeCode.Value));
        }

        [Fact]
        public async Task Return_Price_Series_In_Custom_Order()
        {
            // TODO: These tests are running for all seriesItemTypes - we should change this
            await CreateDefaultContentBlockDefinition();

            var contentBlockSaveRequest = new ContentBlockInput
            {
                ContentBlockId = ContentBlockId,
                Title = "Some Melamine",
                PriceSeriesGrids =
                [
                    new PriceSeriesGrid
                    {
                        Title = "Price Series Grid Title",
                        PriceSeriesIds = [TestSeries.Melamine_China_Spot_FOB, TestSeries.Melamine_Asia_SE_Spot]
                    },
                ]
            };

            _ = await ContentBlockClient.SaveContentBlock(contentBlockSaveRequest);

            var response = await ContentBlockClient.GetContentBlockWithPriceSeriesOnly(ContentBlockId, Today)
                              .GetRawResponse();

            response.MatchSnapshot();
        }

        [Fact]
        public async Task Return_ReadOnly_Price_Series_When_Status_Is_PUBLISHED()
        {
            var assessedDate = Today;
            var priceSeriesIds = ContentBlockQueryHelper.CreatePriceSeriesIds([PriceSeriesId]);

            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, priceSeriesIds, HttpClient)
               .SaveContentBlockDefinition()
               .SavePriceSeriesItem(PriceSeriesId, assessedDate, GetSeriesItemTypeCode(), BuildValidSeriesItem())
               .InitiatePublish(assessedDate)
               .AcknowledgePublished(assessedDate);
            await businessFlow.Execute();

            var contentBlockWithPriceSeriesResponse = await ContentBlockClient
                                                         .GetContentBlockWithPriceSeriesOnly(ContentBlockId, Today)
                                                         .GetRawResponse();

            contentBlockWithPriceSeriesResponse.MatchSnapshotWithSeriesItemId(SnapshotNameExtension.Create(SeriesItemTypeCode.Value));
        }

        [Fact]
        public async Task Return_Same_Published_Price_Series_When_Content_Block_Definition_Changes()
        {
            var assessedDate = Today;
            var priceSeriesIds = ContentBlockQueryHelper.CreatePriceSeriesIds([PriceSeriesId]);

            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, priceSeriesIds, HttpClient)
               .SaveContentBlockDefinition()
               .SavePriceSeriesItem(PriceSeriesId, assessedDate, GetSeriesItemTypeCode(), GetDefaultSeriesItem())
               .InitiatePublish(assessedDate)
               .AcknowledgePublished(assessedDate)
               .AddSeriesIdToGrid(0, AlternativePriceSeriesId)
               .SaveContentBlockDefinition();

            _ = await businessFlow.Execute();

            var result = await ContentBlockClient.GetContentBlockWithPriceSeriesOnly(ContentBlockId, assessedDate)
                            .GetRawResponse();

            var nextDayResult = await ContentBlockClient
                                   .GetContentBlockWithPriceSeriesOnly(
                                        ContentBlockId,
                                        assessedDate.AddDays(1))
                                   .GetRawResponse();

            result.MatchSnapshotWithSeriesItemId(SnapshotNameExtension.Create(SeriesItemTypeCode.Value));

            // from next day onwards it should return based on the new content block definition
            nextDayResult.MatchSnapshot(SnapshotNameExtension.Create("nextDay", SeriesItemTypeCode.Value));
        }

        [Fact]
        public async Task Return_Validation_Error_When_Some_Series_Are_Invalid_While_Publishing()
        {
            var assessedDate = Today;
            var priceSeriesIds = ContentBlockQueryHelper.CreatePriceSeriesIds([PriceSeriesId, AlternativePriceSeriesId]);

            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, priceSeriesIds, HttpClient)
                   .SaveContentBlockDefinition()
                   .SavePriceSeriesItem(
                        PriceSeriesId,
                        assessedDate,
                        GetSeriesItemTypeCode(),
                        GetDefaultSeriesItem())
                   .InitiatePublish(assessedDate);

            var result = await businessFlow.Execute().GetRawResponse();

            result.MatchSnapshotWithSeriesItemId(SnapshotNameExtension.Create(SeriesItemTypeCode.Value));
        }

        [Fact]
        public async Task Return_Price_Series_With_NonMarketAdjustments()
        {
            var priceSeriesIds = ContentBlockQueryHelper.CreatePriceSeriesIds([PriceSeriesId]);

            var businessFlow =
                new PriceEntryBusinessFlow(ContentBlockId, priceSeriesIds, HttpClient)
                   .SaveContentBlockDefinition()
                   .StartNonMarketAdjustment(Today)
                   .SavePriceSeriesItem(
                        PriceSeriesId,
                        Today,
                        GetSeriesItemTypeCode(),
                        SetNonMarketAdjustmentValues(BuildValidSeriesItem()));

            _ = await businessFlow.Execute();

            var response = await ContentBlockClient.GetContentBlockWithPriceSeriesOnly(ContentBlockId, Today)
                              .GetRawResponse();
            response.MatchSnapshotWithSeriesItemId(SnapshotNameExtension.Create(SeriesItemTypeCode.Value));
        }

        [Fact]
        public async Task Reset_NonMarketAdjustments_Fields_When_Cancelling_It()
        {
            var priceSeriesIds = ContentBlockQueryHelper.CreatePriceSeriesIds([PriceSeriesId]);

            var businessFlow =
                new PriceEntryBusinessFlow(ContentBlockId, priceSeriesIds, HttpClient)
                   .SaveContentBlockDefinition()
                   .StartNonMarketAdjustment(Today)
                   .SavePriceSeriesItem(
                        PriceSeriesId,
                        Today,
                        GetSeriesItemTypeCode(),
                        SetNonMarketAdjustmentValues(BuildValidSeriesItem()))
                   .CancelNonMarketAdjustment(Today);

            _ = await businessFlow.Execute();

            var response = await ContentBlockClient.GetContentBlockWithPriceSeriesOnly(ContentBlockId, Today)
                              .GetRawResponse();
            response.MatchSnapshotWithSeriesItemId(SnapshotNameExtension.Create(SeriesItemTypeCode.Value));
        }

        [Fact]
        public async Task Return_Empty_Grid_When_All_Series_Are_Terminated()
        {
            var assessedDate = Today;
            var fourSeries = GetFourSeriesIds();
            fourSeries.Should().HaveCount(4);
            var priceSeriesService = GetService<PriceSeriesTestService>();

            // Terminate all series
            foreach (var seriesId in fourSeries)
            {
                await priceSeriesService.SetTerminationDate(seriesId, assessedDate.AddDays(-1));
            }

            var priceSeriesIds = ContentBlockQueryHelper.CreatePriceSeriesIds(fourSeries);

            // Create content block with all terminated series
            var businessFlow =
                    new PriceEntryBusinessFlow(ContentBlockId, priceSeriesIds, HttpClient)
                       .SaveContentBlockDefinition();

            _ = await businessFlow.Execute();

            var result = await ContentBlockClient.GetContentBlockWithPriceSeriesOnly(ContentBlockId, assessedDate)
                            .GetRawResponse();

            result.MatchSnapshotWithSeriesItemId(SnapshotNameExtension.Create(SeriesItemTypeCode.Value));

            // Reset termination dates as series might be used by other tests
            foreach (var seriesId in fourSeries)
            {
                await priceSeriesService.SetTerminationDate(seriesId, null);
            }
        }

        protected abstract SeriesItem BuildInvalidSeriesItem();

        protected abstract SeriesItem BuildValidSeriesItem();

        protected abstract SeriesItem GetDefaultSeriesItem();

        protected abstract SeriesItemTypeCode GetSeriesItemTypeCode();

        protected abstract string[] GetFourSeriesIds();

        protected abstract SeriesItem SetNonMarketAdjustmentValues(SeriesItem seriesItem);

        protected async Task CreateDefaultContentBlockDefinition()
        {
            var contentBlockSaveRequest = new ContentBlockInput
            {
                ContentBlockId = ContentBlockId,
                Title = "Some content block title",
                PriceSeriesGrids =
                [
                    new PriceSeriesGrid { Title = "Price Series Grid Title", PriceSeriesIds = [PriceSeriesId] },
                ]
            };

            _ = await ContentBlockClient.SaveContentBlock(contentBlockSaveRequest);
        }
    }
}