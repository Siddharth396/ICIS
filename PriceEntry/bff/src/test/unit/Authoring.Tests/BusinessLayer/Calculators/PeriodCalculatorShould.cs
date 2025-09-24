namespace Authoring.Tests.BusinessLayer.Calculators
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using FluentAssertions;

    using global::BusinessLayer.PriceEntry.Services.Calculators.Periods;

    using global::Infrastructure.Services.PeriodGenerator;

    using Microsoft.Extensions.Caching.Memory;

    using NSubstitute;

    using Xunit;

    public class PeriodCalculatorShould
    {
        private readonly PeriodCalculator periodCalculator;

        private readonly IPeriodGeneratorService periodGeneratorService;

        public PeriodCalculatorShould()
        {
            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            periodGeneratorService = Substitute.For<IPeriodGeneratorService>();
            periodCalculator = new PeriodCalculator(memoryCache, periodGeneratorService);
        }

        [Fact]
        public async Task Request_Items_From_PeriodGeneratorService_When_Not_Cached()
        {
            // Arrange
            var referenceDate = new DateOnly(2022, 1, 1);
            var fulfilmentFromDate = new DateOnly(2022, 1, 1);
            var fulfilmentUntilDate = new DateOnly(2022, 1, 1);

            var input = new PeriodCalculatorInput
            {
                ReferenceDate = referenceDate, PeriodCodes = new List<string> { "HM1", "MM2" }
            };

            MockPeriodGeneratorServiceResponse(referenceDate, fulfilmentFromDate, fulfilmentUntilDate);

            // Act
            var result = await periodCalculator.CalculatePeriods(input);

            // Assert
            result.Should().HaveCount(2);
            result.Should()
               .ContainEquivalentOf(
                    new PeriodCalculatorOutputItem
                    {
                        ReferenceDate = referenceDate,
                        Code = "2H2503",
                        PeriodCode = "HM1",
                        Label = "Period 1",
                        FromDate = fulfilmentFromDate,
                        UntilDate = fulfilmentUntilDate
                    });

            result.Should()
               .ContainEquivalentOf(
                    new PeriodCalculatorOutputItem
                    {
                        ReferenceDate = referenceDate,
                        Code = "MM2505",
                        PeriodCode = "MM2",
                        Label = "Period 2",
                        FromDate = fulfilmentFromDate,
                        UntilDate = fulfilmentUntilDate
                    });
            await periodGeneratorService.Received(1).GeneratePeriods(Arg.Any<DateOnly>(), Arg.Any<List<string>>());
        }

        [Fact]
        public async Task Return_Cached_Items_When_Available()
        {
            // Arrange
            var referenceDate = new DateOnly(2022, 1, 1);
            var fromDate = new DateOnly(2022, 1, 1);
            var untilDate = new DateOnly(2022, 1, 1);

            var input = new PeriodCalculatorInput
            {
                ReferenceDate = referenceDate, PeriodCodes = new List<string> { "HM1", "MM2" }
            };

            MockPeriodGeneratorServiceResponse(referenceDate, fromDate, untilDate);

            // Ensure items are cached on first request
            _ = await periodCalculator.CalculatePeriods(input);

            // Act
            var result = await periodCalculator.CalculatePeriods(input);

            // Assert
            result.Should().HaveCount(2);
            result.Should()
               .ContainEquivalentOf(
                    new PeriodCalculatorOutputItem
                    {
                        Code = "2H2503",
                        ReferenceDate = referenceDate,
                        PeriodCode = "HM1",
                        Label = "Period 1",
                        FromDate = fromDate,
                        UntilDate = untilDate
                    });
            result.Should()
               .ContainEquivalentOf(
                    new PeriodCalculatorOutputItem
                    {
                        Code = "MM2505",
                        ReferenceDate = referenceDate,
                        PeriodCode = "MM2",
                        Label = "Period 2",
                        FromDate = fromDate,
                        UntilDate = untilDate
                    });
            await periodGeneratorService.Received(1).GeneratePeriods(Arg.Any<DateOnly>(), Arg.Any<List<string>>());
        }

        [Fact]
        public async Task Return_Items_Only_For_Allowed_Period_Codes()
        {
            // Arrange
            var referenceDate = new DateOnly(2022, 1, 1);
            var fulfilmentFromDate = new DateOnly(2022, 1, 1);
            var fulfilmentUntilDate = new DateOnly(2022, 1, 1);

            var input = new PeriodCalculatorInput
            {
                ReferenceDate = referenceDate,
                PeriodCodes = new List<string> { "HM1", "MM2", "INVALID" }
            };

            MockPeriodGeneratorServiceResponse(referenceDate, fulfilmentFromDate, fulfilmentUntilDate);

            // Ensure items are cached on first request
            _ = await periodCalculator.CalculatePeriods(input);

            // Act
            var result = await periodCalculator.CalculatePeriods(input);

            // Assert
            result.Should().HaveCount(2);
            result.Should()
               .ContainEquivalentOf(
                    new PeriodCalculatorOutputItem
                    {
                        Code = "2H2503",
                        ReferenceDate = referenceDate,
                        PeriodCode = "HM1",
                        Label = "Period 1",
                        FromDate = fulfilmentFromDate,
                        UntilDate = fulfilmentUntilDate
                    });
            result.Should()
               .ContainEquivalentOf(
                    new PeriodCalculatorOutputItem
                    {
                        Code = "MM2505",
                        ReferenceDate = referenceDate,
                        PeriodCode = "MM2",
                        Label = "Period 2",
                        FromDate = fulfilmentFromDate,
                        UntilDate = fulfilmentUntilDate
                    });
            await periodGeneratorService.Received(1).GeneratePeriods(Arg.Any<DateOnly>(), Arg.Any<List<string>>());
        }

        private void MockPeriodGeneratorServiceResponse(DateOnly referenceDate, DateOnly fulfilmentFromDate, DateOnly fulfilmentUntilDate)
        {
            var generatedItem1 = new AbsolutePeriod
            {
                PeriodCode = "HM1",
                Code = "2H2503",
                Label = "Period 1",
                FromDate = fulfilmentFromDate,
                UntilDate = fulfilmentUntilDate
            };

            var generatedItem2 = new AbsolutePeriod
            {
                Code = "MM2505",
                PeriodCode = "MM2",
                Label = "Period 2",
                FromDate = fulfilmentFromDate,
                UntilDate = fulfilmentUntilDate
            };

            var generatedItems = new PeriodGeneratorOutput
            {
                ReferenceDate = referenceDate,
                AbsolutePeriods = new List<AbsolutePeriod> { generatedItem1, generatedItem2 }
            };

            periodGeneratorService.GeneratePeriods(Arg.Any<DateOnly>(), Arg.Any<List<string>>())
               .Returns(generatedItems);
        }
    }
}
