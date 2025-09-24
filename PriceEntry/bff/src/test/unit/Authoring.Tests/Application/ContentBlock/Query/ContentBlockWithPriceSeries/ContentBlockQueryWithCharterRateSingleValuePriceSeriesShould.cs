namespace Authoring.Tests.Application.ContentBlock.Query.ContentBlockWithPriceSeries
{
    using System.Threading.Tasks;

    using Authoring.Tests.Functional.Tests.Common.Infrastructure;

    using FluentAssertions;

    using global::BusinessLayer.PriceEntry.DTOs.Input;
    using global::BusinessLayer.PriceEntry.ValueObjects;

    using Snapshooter;
    using Snapshooter.Xunit;

    using Test.Infrastructure.Authoring;
    using Test.Infrastructure.Authoring.BusinessFlows.PriceEntryBusinessFlow;
    using Test.Infrastructure.Authoring.BusinessFlows.PriceEntryBusinessFlow.PriceSeriesItemBuilders;
    using Test.Infrastructure.Extensions;
    using Test.Infrastructure.Services;
    using Test.Infrastructure.TestData;

    using Xunit;

    [Collection(WebApplicationCollection.Name)]
    public class ContentBlockQueryWithCharterRateSingleValuePriceSeriesShould : ContentBlockQueryWithPriceSeriesTestBase
    {
        public ContentBlockQueryWithCharterRateSingleValuePriceSeriesShould(AuthoringBffApplicationFactory factory)
            : base(factory)
        {
        }

        protected override string PriceSeriesId => TestSeries.CharterRates_Pacific_Prompt_Two_Stroke;

        protected override string AlternativePriceSeriesId => TestSeries.CharterRates_Pacific_Prompt_Steam;

        protected override SeriesItemTypeCode SeriesItemTypeCode => SeriesItemTypeCode.CharterRateSingleValueSeries;

        [Fact]
        public async Task Return_Non_Terminated_Series_Only()
        {
            var assessedDateTime = Today;
            var fourSeries = GetFourSeriesIds();
            fourSeries.Should().HaveCount(4);
            var priceSeriesService = GetService<PriceSeriesTestService>();
            await priceSeriesService.SetTerminationDate(fourSeries[0], assessedDateTime.AddDays(-1));
            await priceSeriesService.SetTerminationDate(fourSeries[1], assessedDateTime);
            await priceSeriesService.SetTerminationDate(fourSeries[2], assessedDateTime.AddDays(1));
            await priceSeriesService.SetTerminationDate(fourSeries[3], null);

            var priceSeriesIds = ContentBlockQueryHelper.CreatePriceSeriesIds(fourSeries);

            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, priceSeriesIds, HttpClient)
                   .SaveContentBlockDefinition();

            _ = await businessFlow.Execute();

            var result = await ContentBlockClient.GetContentBlockWithPriceSeriesOnly(ContentBlockId, assessedDateTime)
                            .GetRawResponse();

            foreach (var seriesId in fourSeries)
            {
                await priceSeriesService.SetTerminationDate(seriesId, null);
            }

            result.MatchSnapshot(SnapshotNameExtension.Create(SeriesItemTypeCode.Value));
        }

        [Fact]
        public async Task Return_Non_Terminated_Series_Only_When_Send_For_Review()
        {
            var assessedDateTime = Today;
            var fourSeries = GetFourSeriesIds();
            fourSeries.Should().HaveCount(4);
            var priceSeriesService = GetService<PriceSeriesTestService>();
            await priceSeriesService.SetTerminationDate(fourSeries[0], assessedDateTime.AddDays(-1));
            await priceSeriesService.SetTerminationDate(fourSeries[1], assessedDateTime);
            await priceSeriesService.SetTerminationDate(fourSeries[2], assessedDateTime.AddDays(1));
            await priceSeriesService.SetTerminationDate(fourSeries[3], null);

            var priceSeriesIds = ContentBlockQueryHelper.CreatePriceSeriesIds(fourSeries);

            var businessFlow =
                new PriceEntryBusinessFlow(ContentBlockId, priceSeriesIds, HttpClient)
                   .SaveContentBlockDefinition()
                   .SavePriceSeriesItem(
                        fourSeries[1],
                        assessedDateTime,
                        GetSeriesItemTypeCode(),
                        GetDefaultSeriesItem())
                   .SavePriceSeriesItem(
                        fourSeries[2],
                        assessedDateTime,
                        GetSeriesItemTypeCode(),
                        GetDefaultSeriesItem())
                   .SavePriceSeriesItem(
                        fourSeries[3],
                        assessedDateTime,
                        GetSeriesItemTypeCode(),
                        GetDefaultSeriesItem())
                   .InitiateSendForReview(assessedDateTime)
                   .AcknowledgeSendForReview(assessedDateTime);

            _ = await businessFlow.Execute().GetRawResponse();

            var result = await ContentBlockClient.GetContentBlockWithPriceSeriesOnly(ContentBlockId, assessedDateTime)
                            .GetRawResponse();

            foreach (var seriesId in fourSeries)
            {
                await priceSeriesService.SetTerminationDate(seriesId, null);
            }

            result.MatchSnapshotWithSeriesItemId(SnapshotNameExtension.Create(SeriesItemTypeCode.Value));
        }

        protected override SeriesItem BuildInvalidSeriesItem()
        {
            return CharterRateSingleValuePriceSeriesItemBuilder.BuildInvalidSeriesItem();
        }

        protected override SeriesItem BuildValidSeriesItem()
        {
            return CharterRateSingleValuePriceSeriesItemBuilder.BuildDefaultValidSeriesItem();
        }

        protected override SeriesItem GetDefaultSeriesItem()
        {
            return CharterRateSingleValuePriceSeriesItemBuilder.BuildDefaultValidSeriesItem();
        }

        protected override SeriesItemTypeCode GetSeriesItemTypeCode()
        {
            return SeriesItemTypeCode.CharterRateSingleValueSeries;
        }

        protected override string[] GetFourSeriesIds()
        {
            return
            [
                TestSeries.CharterRates_Pacific_MidTerm_Two_Stroke,
                TestSeries.CharterRates_Pacific_LongTerm_Two_Stroke,
                TestSeries.CharterRates_Pacific_Prompt_Two_Stroke,
                TestSeries.CharterRates_Pacific_Prompt_Steam
            ];
        }

        protected override SeriesItem SetNonMarketAdjustmentValues(SeriesItem seriesItem)
        {
            seriesItem.AdjustedPriceDelta = -1.1m;
            return seriesItem;
        }
    }
}
