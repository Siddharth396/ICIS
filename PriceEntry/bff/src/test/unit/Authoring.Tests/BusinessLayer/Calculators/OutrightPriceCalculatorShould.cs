namespace Authoring.Tests.BusinessLayer.Calculators
{
    using FluentAssertions;

    using global::BusinessLayer.PriceEntry.Services.Calculators;

    using Xunit;

    public class OutrightPriceCalculatorShould
    {
        public static TheoryData<decimal?, decimal?, decimal?> TestData =>
            new()
            {
                { 10, 5, 15 },
                { 10, -5, 5 },
                { 10, -15, -5 },
                { 0, 0, 0 },
                { null, 5, null },
                { 10, null, null },
                { null, null, null }
            };

        [Theory]
        [MemberData(nameof(TestData))]
        public void Calculate_Correct_Outright_Price(decimal? price, decimal? offset, decimal? expectedPrice)
        {
            // Arrange
            var calculator = new OutrightPriceCalculator();

            // Act
            var adjustedPrice = calculator.GetOutrightPrice(price, offset);

            // Assert
            adjustedPrice.Should().Be(expectedPrice);
        }
    }
}
