namespace Authoring.Tests.BusinessLayer.Calculators
{
    using FluentAssertions;

    using global::BusinessLayer.PriceEntry.Services.Calculators;

    using Xunit;

    public class DeltaCalculatorShould
    {
        private readonly DeltaCalculator deltaCalculator;

        public DeltaCalculatorShould()
        {
            deltaCalculator = new DeltaCalculator();
        }

        public static TheoryData<decimal, decimal, decimal, decimal> GetDeltaPriceTestData()
        {
            return new TheoryData<decimal, decimal, decimal, decimal>
            {
                { 10, 5, 5, 100 },
                { 5, 10, -5, -50 },
                { 0, 5, -5, -100 },
                { 5, 0, 5, 500 }
            };
        }

        public static TheoryData<decimal?, decimal?> GetNullInputTestData()
        {
            return new TheoryData<decimal?, decimal?> { { null, null }, { 10, null }, { null, 5 } };
        }

        [Theory]
        [MemberData(nameof(GetDeltaPriceTestData))]
        public void Calculate_Correct_Delta_Price_When_All_Input_Fields_Are_Not_Null(
            decimal currentPrice,
            decimal previousPrice,
            decimal expectedPriceDelta,
            decimal expectedPriceDeltaPercentage)
        {
            // Act
            var result = deltaCalculator.GetPriceDelta(currentPrice, previousPrice);

            // Assert
            result.PriceDelta.Should().Be(expectedPriceDelta);
            result.PriceDeltaPercentage.Should().Be(expectedPriceDeltaPercentage);
        }

        [Theory]
        [MemberData(nameof(GetNullInputTestData))]
        public void Return_Null_For_All_Values_When_Any_Input_Field_Is_Null(
            decimal? currentPrice,
            decimal? previousPrice)
        {
            // Act
            var result = deltaCalculator.GetPriceDelta(currentPrice, previousPrice);

            // Assert
            result.PriceDelta.Should().BeNull();
            result.PriceDeltaPercentage.Should().BeNull();
        }
    }
}
