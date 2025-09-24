namespace Authoring.Tests.Application.PriceSeriesSelection.Query
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Authoring.Tests.Functional.Tests.Common.Infrastructure;
    using Microsoft.Extensions.DependencyInjection;

    using Snapshooter;
    using Snapshooter.Xunit;

    using Test.Infrastructure.Authoring;
    using Test.Infrastructure.Authoring.GraphQLClients.PriceEntry;
    using Test.Infrastructure.Extensions;
    using Test.Infrastructure.Mongo.Repositories;
    using Test.Infrastructure.Stubs;
    using Test.Infrastructure.TestData;

    using Xunit;

    [Collection(WebApplicationCollection.Name)]
    public class PriceSeriesSelectionQueriesShould : WebApplicationTestBase
    {
        private readonly PriceSeriesSelectionGraphQLClient priceSeriesSelectionClient;
        private readonly PriceSeriesRepository priceSeriesRepository;

        private readonly TestClock testClock;

        public PriceSeriesSelectionQueriesShould(AuthoringBffApplicationFactory factory)
            : base(factory)
        {
            priceSeriesSelectionClient = new PriceSeriesSelectionGraphQLClient(GraphQLClient);
            testClock = factory.Services.GetRequiredService<TestClock>();
            testClock.Reset();
            this.priceSeriesRepository = GetService<PriceSeriesRepository>();
        }

        public static IEnumerable<object?[]> FiltersBasedOnSelectedPriceSeriesData =>
        [
            ["Default_Filters", Array.Empty<string>()],
            ["Pre_Selected_Filters_Null", null],
            ["World_Region_And_Null_Settlement_Type", new[] { TestSeries.CharterRates_Atlantic_Prompt_Steam }],
            ["Selected_TransactionType", new[] { TestSeries.LNG_Africa_West_FOB, TestSeries.LNG_Egypt_DES }],
            ["Selected_Commodity", new[] { TestSeries.Styrene_Fujian_Spot_China_S }],
            ["Selected_PriceCategory", new[] { TestSeries.LNG_EAX_Index_HM2 }],
            ["Selected_AssessedFrequency", new[] { TestSeries.LNG_Truck_Guangdong_Publication_Schedules }]
        ];

        [Fact]
        public async Task Return_The_Price_Series_By_Commodity()
        {
            var result = await priceSeriesSelectionClient.GetPriceSeriesWithFilters(
                             TestData.CommodityLng,
                             TestData.PriceCategoryAssessed,
                             TestData.RegionAsiaPacific,
                             TestData.TransactionTypeSpot,
                             TestData.FrequencyDaily)
                            .GetRawResponse();

            result.MatchSnapshot();
        }

        [Fact]
        public async Task Return_The_Charter_Rate_Series()
        {
            var result = await priceSeriesSelectionClient.GetPriceSeriesWithFilters(
                                 TestData.CommodityLng,
                                 TestData.PriceCategoryAssessed,
                                 TestData.RegionWorld,
                                 TestData.TransactionTypeNotApplicable,
                                 TestData.FrequencyDaily)
                            .GetRawResponse();

            result.MatchSnapshot();
        }

        [Fact]
        public async Task Return_The_Melamine_Price_Series_If_Termination_Date_Is_Null()
        {
            var result = await priceSeriesSelectionClient.GetPriceSeriesWithFilters(
                             TestData.CommodityMelamine,
                             TestData.PriceCategoryAssessed,
                             TestData.RegionEurope,
                             TestData.TransactionTypeContract,
                             TestData.FrequencyQuarterly)
                            .GetRawResponse();

            result.MatchSnapshot();
        }

        [Fact]
        public async Task Not_Return_The_Price_Series_If_They_Are_Terminated()
        {
            testClock.SetUtcNow(new DateTime(2014, 02, 11));
            var result = await priceSeriesSelectionClient.GetPriceSeriesWithFilters(
                             TestData.CommodityMelamine,
                             TestData.PriceCategoryAssessed,
                             TestData.RegionEurope,
                             TestData.TransactionTypeContract,
                             TestData.FrequencyWeekly)
                            .GetRawResponse();

            result.MatchSnapshot();
        }

        [Fact]
        public async Task Return_The_Price_Series_If_They_Are_Terminated_Today()
        {
            testClock.SetUtcNow(new DateTime(2014, 02, 10));
            var result = await priceSeriesSelectionClient.GetPriceSeriesWithFilters(
                             TestData.CommodityMelamine,
                             TestData.PriceCategoryAssessed,
                             TestData.RegionEurope,
                             TestData.TransactionTypeContract,
                             TestData.FrequencyQuarterly)
                            .GetRawResponse();

            result.MatchSnapshot();
        }

        [Theory]
        [MemberData(nameof(FiltersBasedOnSelectedPriceSeriesData))]
        public async Task Return_Filters_Based_On(string scenario, string[] series)
        {
            var result = await priceSeriesSelectionClient.GetFilters(series)
                            .GetRawResponse();

            result.MatchSnapshot(SnapshotNameExtension.Create(scenario));
        }

        [Fact]
        public async Task Not_Return_The_Price_Series_When_Commodity_Is_Null()
        {
            var originalPriceSeries = await priceSeriesRepository.GetPriceSeries(TestSeries.LNG_Dubai_MM1);

            await priceSeriesRepository.PatchPriceSeries(TestSeries.LNG_Dubai_MM1, x => x.Commodity, null);

            var result = await priceSeriesSelectionClient.GetPriceSeriesWithFilters(
                             TestData.CommodityLng,
                             TestData.PriceCategoryAssessed,
                             TestData.RegionAsiaPacific,
                             TestData.TransactionTypeSpot,
                             TestData.FrequencyDaily)
                            .GetRawResponse();

            await priceSeriesRepository.PatchPriceSeries(TestSeries.LNG_Dubai_MM1, x => x.Commodity, originalPriceSeries.Commodity);

            result.MatchSnapshot();
        }

        [Fact]
        public async Task Not_Return_The_Price_Series_When_Commodity_Guid_Is_Null()
        {
            var originalPriceSeries = await priceSeriesRepository.GetPriceSeries(TestSeries.LNG_Dubai_MM1);

            await priceSeriesRepository.PatchPriceSeries(TestSeries.LNG_Dubai_MM1, "commodity.guid", null);

            var result = await priceSeriesSelectionClient.GetPriceSeriesWithFilters(
                             TestData.CommodityLng,
                             TestData.PriceCategoryAssessed,
                             TestData.RegionAsiaPacific,
                             TestData.TransactionTypeSpot,
                             TestData.FrequencyDaily)
                            .GetRawResponse();

            await priceSeriesRepository.PatchPriceSeries(TestSeries.LNG_Dubai_MM1, x => x.Commodity.Guid!, originalPriceSeries.Commodity.Guid);

            result.MatchSnapshot();
        }

        [Fact]
        public async Task Not_Return_The_Price_Series_When_Region_Is_Null()
        {
            var originalPriceSeries = await priceSeriesRepository.GetPriceSeries(TestSeries.LNG_Dubai_MM1);

            await priceSeriesRepository.PatchPriceSeries(TestSeries.LNG_Dubai_MM1, x => x.Location.Region, null);

            var result = await priceSeriesSelectionClient.GetPriceSeriesWithFilters(
                             TestData.CommodityLng,
                             TestData.PriceCategoryAssessed,
                             TestData.RegionAsiaPacific,
                             TestData.TransactionTypeSpot,
                             TestData.FrequencyDaily)
                            .GetRawResponse();

            await priceSeriesRepository.PatchPriceSeries(TestSeries.LNG_Dubai_MM1, x => x.Location.Region, originalPriceSeries.Location.Region);

            result.MatchSnapshot();
        }

        [Fact]
        public async Task Not_Return_The_Price_Series_When_Region_Guid_Is_Null()
        {
            var originalPriceSeries = await priceSeriesRepository.GetPriceSeries(TestSeries.LNG_Dubai_MM1);

            await priceSeriesRepository.PatchPriceSeries(TestSeries.LNG_Dubai_MM1, "location.region.guid", null);

            var result = await priceSeriesSelectionClient.GetPriceSeriesWithFilters(
                             TestData.CommodityLng,
                             TestData.PriceCategoryAssessed,
                             TestData.RegionAsiaPacific,
                             TestData.TransactionTypeSpot,
                             TestData.FrequencyDaily)
                            .GetRawResponse();

            await priceSeriesRepository.PatchPriceSeries(TestSeries.LNG_Dubai_MM1, x => x.Location.Region.Guid, originalPriceSeries.Location.Region.Guid);

            result.MatchSnapshot();
        }

        [Fact]
        public async Task Not_Return_The_Price_Series_When_Location_Is_Null()
        {
            var originalPriceSeries = await priceSeriesRepository.GetPriceSeries(TestSeries.LNG_Dubai_MM1);

            await priceSeriesRepository.PatchPriceSeries(TestSeries.LNG_Dubai_MM1, x => x.Location, null);

            var result = await priceSeriesSelectionClient.GetPriceSeriesWithFilters(
                             TestData.CommodityLng,
                             TestData.PriceCategoryAssessed,
                             TestData.RegionAsiaPacific,
                             TestData.TransactionTypeSpot,
                             TestData.FrequencyDaily)
                            .GetRawResponse();

            await priceSeriesRepository.PatchPriceSeries(TestSeries.LNG_Dubai_MM1, x => x.Location, originalPriceSeries.Location);

            result.MatchSnapshot();
        }

        [Fact]
        public async Task Not_Return_The_Price_Series_When_PriceCategory_Is_Null()
        {
            var originalPriceSeries = await priceSeriesRepository.GetPriceSeries(TestSeries.LNG_Dubai_MM1);

            await priceSeriesRepository.PatchPriceSeries(TestSeries.LNG_Dubai_MM1, x => x.PriceCategory, null);

            var result = await priceSeriesSelectionClient.GetPriceSeriesWithFilters(
                             TestData.CommodityLng,
                             TestData.PriceCategoryAssessed,
                             TestData.RegionAsiaPacific,
                             TestData.TransactionTypeSpot,
                             TestData.FrequencyDaily)
                            .GetRawResponse();

            await priceSeriesRepository.PatchPriceSeries(TestSeries.LNG_Dubai_MM1, x => x.PriceCategory, originalPriceSeries.PriceCategory);

            result.MatchSnapshot();
        }

        [Fact]
        public async Task Return_The_Price_Series_When_PriceCategory_Name_Is_Null()
        {
            var originalPriceSeries = await priceSeriesRepository.GetPriceSeries(TestSeries.LNG_Dubai_MM1);

            await priceSeriesRepository.PatchPriceSeries(TestSeries.LNG_Dubai_MM1, x => x.PriceCategory.Name, null);

            var result = await priceSeriesSelectionClient.GetPriceSeriesWithFilters(
                             TestData.CommodityLng,
                             TestData.PriceCategoryAssessed,
                             TestData.RegionAsiaPacific,
                             TestData.TransactionTypeSpot,
                             TestData.FrequencyDaily)
                            .GetRawResponse();

            await priceSeriesRepository.PatchPriceSeries(TestSeries.LNG_Dubai_MM1, x => x.PriceCategory.Name, originalPriceSeries.PriceCategory.Name);

            result.MatchSnapshot();
        }

        [Fact]
        public async Task Not_Return_The_Price_Series_When_Frequency_Is_Null()
        {
            var originalPriceSeries = await priceSeriesRepository.GetPriceSeries(TestSeries.LNG_Dubai_MM1);

            await priceSeriesRepository.PatchPriceSeries(TestSeries.LNG_Dubai_MM1, x => x.ItemFrequency, null);

            var result = await priceSeriesSelectionClient.GetPriceSeriesWithFilters(
                             TestData.CommodityLng,
                             TestData.PriceCategoryAssessed,
                             TestData.RegionAsiaPacific,
                             TestData.TransactionTypeSpot,
                             TestData.FrequencyDaily)
                            .GetRawResponse();

            await priceSeriesRepository.PatchPriceSeries(TestSeries.LNG_Dubai_MM1, x => x.ItemFrequency, originalPriceSeries.ItemFrequency);

            result.MatchSnapshot();
        }
    }
}