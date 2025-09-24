namespace Authoring.Tests.BusinessLayer.Calculators
{
    using FluentAssertions;

    using global::BusinessLayer.PriceEntry.Services.Calculators;

    using Xunit;

    public class MidPriceCalculatorShould
    {
        [Theory]
        [InlineData(10, 20, 15)]
        [InlineData(10, 0, 5)]
        [InlineData(0, 0, 0)]
        [InlineData(-10, 10, 0)]
        [InlineData(null, null, null)]
        [InlineData(10, null, null)]
        [InlineData(null, 10, null)]
        public void Calculate_Mid_Price_Correctly(int? priceLow, int? priceHigh, int? expectedMidPrice)
        {
            // Arrange
            var calculator = new MidPriceCalculator();

            // Act
            var result = calculator.CalculateMidPrice(priceLow, priceHigh);

            // Assert
            result.Should().Be(expectedMidPrice);
        }
    }
}
