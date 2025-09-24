namespace Authoring.Tests.BusinessLayer.Services
{
    using System;
    using System.Threading.Tasks;

    using FluentAssertions;

    using global::BusinessLayer.Helpers;
    using global::BusinessLayer.PriceEntry.DTOs.Input;
    using global::BusinessLayer.PriceEntry.DTOs.Output;
    using global::BusinessLayer.PriceEntry.Repositories;
    using global::BusinessLayer.PriceEntry.Repositories.Models;
    using global::BusinessLayer.PriceEntry.Services;
    using global::BusinessLayer.PriceEntry.Services.Calculators.Periods;
    using global::BusinessLayer.PriceEntry.Services.SeriesItemTypes.SingleValueWithReference;
    using global::BusinessLayer.PriceSeriesSelection.Services;
    using global::Infrastructure.MongoDB.Transactions;
    using global::Infrastructure.Services.Workflow;

    using MongoDB.Driver;

    using NSubstitute;

    using Serilog;

    using Test.Infrastructure.TestData;

    using Xunit;

    using ReferencePrice = global::BusinessLayer.PriceEntry.Repositories.Models.ReferencePrice;

    public class ReferencePriceServiceShould
    {
        private static readonly DateTime AssessedDateTime = TestData.GasPricePublishedDateTime;

        private static readonly DateTime FulfilmentFromDateTime = TestData.GasPriceFulfilmentFromDateTime;

        private static readonly DateTime FulfilmentUntilDateTime = TestData.GasPriceFulfilmentUntilDateTime;

        private static readonly long AssessedDateTimestamp = AssessedDateTime.ToUnixTimeMilliseconds();

        private static readonly long FulfilmentFromTimestamp = FulfilmentFromDateTime.ToUnixTimeMilliseconds();

        private static readonly long FulfilmentUntilTimestamp = FulfilmentUntilDateTime.ToUnixTimeMilliseconds();

        private static readonly PriceItemSingleValueWithReference PriceItemPremiumDiscount = new()
        {
            Id = Guid.NewGuid().ToString(),
            SeriesId = TestSeries.LNG_Dubai_MM1,
            AssessedDateTime = AssessedDateTime,
            PremiumDiscount = 10,
            AssessmentMethod = "Premium/Discount",
            DataUsed = "Transaction",
            ReferencePrice = new ReferencePrice
            {
                Market = "TTF",
                Price = 10,
                Datetime = AssessedDateTime,
                PeriodLabel = "February 24",
            },
            Price = 20,
            FulfilmentFromDate = FulfilmentFromDateTime,
            FulfilmentUntilDate = FulfilmentUntilDateTime,
            Status = WorkflowStatus.Draft.Value
        };

        private static readonly PriceItemSingleValueWithReference PriceItemAssessed = new()
        {
            Id = Guid.NewGuid().ToString(),
            SeriesId = TestSeries.LNG_Argentina_MM1,
            AssessedDateTime = AssessedDateTime,
            AssessmentMethod = "Assessed",
            DataUsed = "Transaction",
            Price = 20,
            FulfilmentFromDate = FulfilmentFromDateTime,
            FulfilmentUntilDate = FulfilmentUntilDateTime,
            Status = WorkflowStatus.Draft.Value
        };

        private static readonly PriceItemSingleValue PriceItemSingleValue = new()
        {
            Id = Guid.NewGuid().ToString(),
            SeriesId = TestSeries.LNG_Japan_MM1,
            AssessedDateTime = AssessedDateTime,
            Price = 20,
            FulfilmentFromDate = FulfilmentFromDateTime,
            FulfilmentUntilDate = FulfilmentUntilDateTime,
            Status = WorkflowStatus.Draft.Value
        };

        private static readonly PriceItemRange PriceItemRange = new PriceItemRange
        {
            Id = Guid.NewGuid().ToString(),
            SeriesId = TestSeries.Petchem_8602999,
            AssessedDateTime = AssessedDateTime,
            PriceHigh = 20,
            PriceLow = 10,
            Status = WorkflowStatus.Draft.Value
        };

        private readonly ReferencePriceService referencePriceService;

        private readonly IPriceSeriesItemsDomainService priceSeriesItemsDomainService;

        public ReferencePriceServiceShould()
        {
            var priceSeriesService = Substitute.For<IPriceSeriesService>();
            var priceEntryService = Substitute.For<IAuthoringService>();
            var periodCalculator = Substitute.For<IPeriodCalculator>();
            priceSeriesItemsDomainService = Substitute.For<IPriceSeriesItemsDomainService>();

            priceEntryService.SavePriceEntryData(Arg.Any<PriceItemInput>())
                .Returns(new PriceEntryDataSaveResponse { Id = PriceItemPremiumDiscount.Id });

            var mongoDatabase = Substitute.For<IMongoDatabase>();
            var mongoContext = Substitute.For<IMongoContext>();
            var logger = Substitute.For<ILogger>();

            var referenceMarketRepository = new ReferenceMarketRepository(mongoDatabase, mongoContext);
            var gasPriceSeriesItemRepository = new GasPriceSeriesItemRepository(mongoDatabase, mongoContext);

            referencePriceService = new ReferencePriceService(
                               priceSeriesService,
                               referenceMarketRepository,
                               gasPriceSeriesItemRepository,
                               priceEntryService,
                               logger,
                               periodCalculator,
                               priceSeriesItemsDomainService);
        }

        [Fact]
        public async Task Update_Reference_Price_When_Gas_Price_Is_Published()
        {
            // Arrange
            var input = new GasPricePayload()
            {
                MarketCode = "TTF",
                AssessedDateTimestamp = AssessedDateTimestamp,
                FulfilmentFromTimestamp = FulfilmentFromTimestamp,
                FulfilmentUntilTimestamp = FulfilmentUntilTimestamp,
                Mid = 10,
            };

            priceSeriesItemsDomainService.GetPriceSeriesItems(Arg.Any<DateTime>(), Arg.Any<DateTime>(), Arg.Any<DateTime>())
                .Returns(
                [
                    PriceItemPremiumDiscount,
                    PriceItemAssessed,
                    PriceItemSingleValue,
                    PriceItemRange
                ]);

            // Act
            var result = await referencePriceService.UpdateGasReferencePrice(input);

            // Assert
            result.Should().HaveCount(1);
            result[0].Should().Be(PriceItemPremiumDiscount.Id);
        }
    }
}
