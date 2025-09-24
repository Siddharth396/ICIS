namespace Authoring.Tests.Application.AbsolutePeriod
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Authoring.Tests.Functional.Tests.Common.Infrastructure;
    using FluentAssertions;

    using global::BusinessLayer.PriceEntry.Repositories;
    using global::BusinessLayer.PriceEntry.Repositories.Models;
    using global::Infrastructure.Services.PeriodGenerator;

    using Test.Infrastructure.Authoring;
    using Test.Infrastructure.Authoring.BusinessFlows.PriceEntryBusinessFlow;
    using Test.Infrastructure.Authoring.BusinessFlows.PriceEntryBusinessFlow.PriceSeriesItemBuilders;
    using Test.Infrastructure.Mongo.Repositories;
    using Test.Infrastructure.Stubs;
    using Test.Infrastructure.TestData;

    using Xunit;

    [Collection(WebApplicationCollection.Name)]
    public class AbsolutePeriodShould : WebApplicationTestBase
    {
        private const string ContentBlockId = "contentBlockId";

        private static readonly DateTime Now = TestData.Now;

        private readonly GenericRepository genericRepository;

        private readonly PeriodGeneratorServiceStub periodGeneratorServiceStub;

        private readonly PriceSeriesRepository priceSeriesRepository;

        public AbsolutePeriodShould(AuthoringBffApplicationFactory factory)
            : base(factory)
        {
            periodGeneratorServiceStub = GetService<PeriodGeneratorServiceStub>();
            this.priceSeriesRepository = GetService<PriceSeriesRepository>();

            genericRepository = GetService<GenericRepository>();
            var testClock = GetService<TestClock>();
            testClock.SetUtcNow(Now);
        }

        public static IEnumerable<object?[]> FiltersBasedOnSelectedPriceSeriesData =>
        [
          [
            TestSeries.Styrene_Europe_Monthly_Contract,
                new AbsolutePeriod()
                {
                    Code = "M2402",
                    PeriodCode = "M",
                    Label = "Feb 2024",
                    FromDate = new DateOnly(2024, 02, 01),
                    UntilDate = new DateOnly(2024, 02, 29)
                }
            ],
          [
            TestSeries.Caustic_Soda_Europe_Contract,
                new AbsolutePeriod()
                {
                    Code = "Q2402",
                    PeriodCode = "Q",
                    Label = "Q2 2024",
                    FromDate = new DateOnly(2024, 04, 01),
                    UntilDate = new DateOnly(2024, 06, 30)
                }
            ]
        ];

        [Fact]
        public async Task Only_AppliesFromDateTime_Should_be_Set_As_AssessedDatetime_When_Period_Generator_Return_No_Records_For_RelativeFulfilmentPeriod()
        {
            try
            {
                // Arrange
                var seriesId = TestSeries.LNG_China_MM2;
                var date = Now.AddYears(1);

                periodGeneratorServiceStub.KeepAbsolutePeriodsEmpty = true;
                periodGeneratorServiceStub.ReplaceAbsolutePeriods([]);

                // Act
                var businessFlow = new PriceEntryBusinessFlow(
                                    ContentBlockId,
                                    [seriesId],
                                    HttpClient)
                                   .SaveSingleValueWithReferencePriceSeriesItem(seriesId, date, SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice(20));

                await businessFlow.Execute();

                // Assert
                var priceSeriesItem = (await genericRepository.GetDocument<BasePriceItem>(
                        PriceSeriesItemRepository.CollectionName,
                        x => x.SeriesId == seriesId &&
                        x.AssessedDateTime == date)).First();

                priceSeriesItem.FulfilmentFromDate.Should().BeNull();
                priceSeriesItem.FulfilmentUntilDate.Should().BeNull();

                priceSeriesItem.AppliesFromDateTime.Should().NotBeNull().And.Be(date);
            }
            finally
            {
                periodGeneratorServiceStub.KeepAbsolutePeriodsEmpty = false;
            }
        }

        [Fact]
        public async Task No_Absolute_Period_Except_For_AppliesFromDateTime_Should_be_Set_For_PeriodLabelTypeCode_None()
        {
            // Arrange
            var seriesId = TestSeries.LNG_Reload_Spain_20_40D;

            // Act
            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, [seriesId], HttpClient)
            .SaveRangePriceSeriesItem(seriesId, Now, RangePriceSeriesItemBuilder.BuildDefaultValidSeriesItem());

            await businessFlow.Execute();

            // Assert
            var priceSeriesItem = (await genericRepository.GetDocument<BasePriceItem>(
                    PriceSeriesItemRepository.CollectionName,
                    x => x.SeriesId == seriesId &&
                    x.AssessedDateTime == Now)).First();

            priceSeriesItem.FulfilmentFromDate.Should().BeNull();
            priceSeriesItem.FulfilmentUntilDate.Should().BeNull();
            priceSeriesItem.AppliesUntilDateTime.Should().BeNull();

            priceSeriesItem.AppliesFromDateTime.Should().Be(Now);
        }

        [Fact]
        public async Task No_Absolute_Period_Should_be_Set_When_PeriodLabelTypeCode_Is_Invalid()
        {
            var seriesId = TestSeries.LNG_China_MM2;

            try
            {
                // Arrange
                await priceSeriesRepository.PatchPriceSeries(seriesId, x => x.PeriodLabelTypeCode!, "Invalid");

                // Act
                var businessFlow = new PriceEntryBusinessFlow(
                                    ContentBlockId,
                                    [seriesId],
                                    HttpClient)
                                   .SaveSingleValueWithReferencePriceSeriesItem(seriesId, Now, SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice(20));

                await businessFlow.Execute();

                // Assert
                var priceSeriesItem = (await genericRepository.GetDocument<BasePriceItem>(
                        PriceSeriesItemRepository.CollectionName,
                        x => x.SeriesId == seriesId &&
                        x.AssessedDateTime == Now)).First();

                priceSeriesItem.FulfilmentFromDate.Should().BeNull();
                priceSeriesItem.FulfilmentUntilDate.Should().BeNull();
            }
            finally
            {
                await priceSeriesRepository.PatchPriceSeries(seriesId, x => x.PeriodLabelTypeCode!, "plt-ffmt-time");
            }
        }

        [Theory]
        [MemberData(nameof(FiltersBasedOnSelectedPriceSeriesData))]
        public async Task Absolute_Period_Should_be_Set_For_Reference_Period_When_Previous_Published_PriceSeriesItem_Is_Available(string seriesId, AbsolutePeriod absolutePeriod)
        {
            var periodGeneratorServiceStub = GetService<PeriodGeneratorServiceStub>();

            try
            {
                // Arrange
                var yesterday = Now.AddDays(-1);

                // Act
                var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, [seriesId], HttpClient)
                    .SaveContentBlockDefinition()
                    .SaveRangePriceSeriesItem(seriesId, yesterday, RangePriceSeriesItemBuilder.BuildDefaultValidSeriesItem())
                    .InitiatePublish(yesterday)
                    .AcknowledgePublished(yesterday);

                await businessFlow.Execute();

                periodGeneratorServiceStub.ClearAbsolutePeriods();

                periodGeneratorServiceStub.ReplaceAbsolutePeriods([absolutePeriod]);

                await businessFlow.SaveRangePriceSeriesItem(seriesId, Now, RangePriceSeriesItemBuilder.BuildDefaultValidSeriesItem())
                                  .Execute();

                // Assert
                var priceSeriesItem = (await genericRepository.GetDocument<BasePriceItem>(
                        PriceSeriesItemRepository.CollectionName,
                        x => x.SeriesId == seriesId &&
                        x.AssessedDateTime == Now)).First();

                priceSeriesItem.FulfilmentFromDate.Should().BeNull();
                DateOnly.FromDateTime(priceSeriesItem.AppliesFromDateTime!.Value).Should().Be(absolutePeriod.FromDate);

                priceSeriesItem.FulfilmentUntilDate.Should().BeNull();
                DateOnly.FromDateTime(priceSeriesItem.AppliesUntilDateTime!.Value).Should().Be(absolutePeriod.UntilDate);
            }
            finally
            {
                periodGeneratorServiceStub.ClearAbsolutePeriods();
            }
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData(" ")]
        public async Task AppliesFromDateTime_Should_be_Set_As_AssessedDatetime_When_ReferencePeriodCode_Is(string? code)
        {
            var seriesId = TestSeries.Styrene_Europe_Monthly_Contract;

            try
            {
                // Arrange
                await priceSeriesRepository.PatchPriceSeries(seriesId, x => x.ReferencePeriod!.Code, code);

                // Act
                var businessFlow = new PriceEntryBusinessFlow(
                                    ContentBlockId,
                                    [seriesId],
                                    HttpClient).
                                    SaveRangePriceSeriesItem(seriesId, Now, RangePriceSeriesItemBuilder.BuildDefaultValidSeriesItem());

                await businessFlow.Execute();

                // Assert
                var priceSeriesItem = (await genericRepository.GetDocument<BasePriceItem>(
                        PriceSeriesItemRepository.CollectionName,
                        x => x.SeriesId == seriesId &&
                        x.AssessedDateTime == Now)).First();

                priceSeriesItem.AppliesUntilDateTime.Should().BeNull();

                priceSeriesItem.AppliesFromDateTime.Should().NotBeNull().And.Be(Now);
            }
            finally
            {
                await priceSeriesRepository.PatchPriceSeries(seriesId, x => x.ReferencePeriod!.Code, "M");
            }
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData(" ")]
        public async Task AppliesFromDateTime_Should_be_Set_As_AssessedDatetime_When_RelativeFulfilmentPeriod_Is(string? code)
        {
            var seriesId = TestSeries.LNG_China_MM2;

            try
            {
                // Arrange
                await priceSeriesRepository.PatchPriceSeries(seriesId, x => x.RelativeFulfilmentPeriod!.Code, code);

                // Act
                var businessFlow = new PriceEntryBusinessFlow(
                                    ContentBlockId,
                                    [seriesId],
                                    HttpClient).
                                    SaveRangePriceSeriesItem(seriesId, Now, RangePriceSeriesItemBuilder.BuildDefaultValidSeriesItem());

                await businessFlow.Execute();

                // Assert
                var priceSeriesItem = (await genericRepository.GetDocument<BasePriceItem>(
                        PriceSeriesItemRepository.CollectionName,
                        x => x.SeriesId == seriesId &&
                        x.AssessedDateTime == Now)).First();

                priceSeriesItem.FulfilmentFromDate.Should().BeNull();
                priceSeriesItem.FulfilmentUntilDate.Should().BeNull();
                priceSeriesItem.AppliesFromDateTime.Should().NotBeNull().And.Be(Now);
            }
            finally
            {
                await priceSeriesRepository.PatchPriceSeries(seriesId, x => x.RelativeFulfilmentPeriod!.Code, "MM2");
            }
        }

        [Fact]
        public async Task AppliesFromDateTime_Should_be_Set_As_AssessedDatetime_When_ReferencePeriodCode_Is_Not_Supported()
        {
            var seriesId = TestSeries.Styrene_Europe_Monthly_Contract;

            try
            {
                // Arrange
                var yesterday = Now.AddDays(-1);
                await priceSeriesRepository.PatchPriceSeries(seriesId, x => x.ReferencePeriod!.Code, "invalid");

                // Act
                var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, [seriesId], HttpClient)
                        .SaveContentBlockDefinition()
                        .SaveRangePriceSeriesItem(seriesId, yesterday, RangePriceSeriesItemBuilder.BuildDefaultValidSeriesItem())
                        .InitiatePublish(yesterday)
                        .AcknowledgePublished(yesterday)
                        .SaveRangePriceSeriesItem(seriesId, Now, RangePriceSeriesItemBuilder.BuildDefaultValidSeriesItem());

                await businessFlow.Execute();

                // Assert
                var priceSeriesItem = (await genericRepository.GetDocument<BasePriceItem>(
                        PriceSeriesItemRepository.CollectionName,
                        x => x.SeriesId == seriesId &&
                        x.AssessedDateTime == Now)).First();

                priceSeriesItem.AppliesFromDateTime.Should().NotBeNull().And.Be(Now);
                priceSeriesItem.AppliesUntilDateTime.Should().BeNull();
            }
            finally
            {
                await priceSeriesRepository.PatchPriceSeries(seriesId, x => x.ReferencePeriod!.Code, "M");
            }
        }
    }
}