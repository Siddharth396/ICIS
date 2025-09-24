namespace Authoring.Tests.BusinessLayer.Validators
{
    using FluentAssertions;

    using global::BusinessLayer.PriceEntry.DTOs.Output;
    using global::BusinessLayer.PriceEntry.Validators;

    using Xunit;

    public class RangePriceSeriesValidatorShould
    {
        [Fact]
        public void Allow_Missing_Prices()
        {
            // Arrange
            var priceSeries = PriceSeriesBuilder.Range.New();
            priceSeries.PriceLow = null;
            priceSeries.PriceHigh = null;

            // Act
            var result = PriceSeriesValidator.Validate(priceSeries);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_That_PriceLow_Is_Required()
        {
            // Arrange
            var priceSeries = PriceSeriesBuilder.Range.New();
            priceSeries.PriceLow = null;

            // Act
            var result = PriceSeriesValidator.Validate(priceSeries);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ShouldHaveValidationErrorFor<PriceSeries>(series => series.PriceLow, "Price low is required.");
        }

        [Fact]
        public void Validate_That_PriceLow_Is_Greater_Than_Zero()
        {
            // Arrange
            var priceSeries = PriceSeriesBuilder.Range.New();
            priceSeries.PriceLow = 0;

            // Act
            var result = PriceSeriesValidator.Validate(priceSeries);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ShouldHaveValidationErrorFor<PriceSeries>(series => series.PriceLow, "Price low must be greater than 0.");
        }

        [Fact]
        public void Validate_That_PriceHigh_Is_Required()
        {
            // Arrange
            var priceSeries = PriceSeriesBuilder.Range.New();
            priceSeries.PriceHigh = null;

            // Act
            var result = PriceSeriesValidator.Validate(priceSeries);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ShouldHaveValidationErrorFor<PriceSeries>(series => series.PriceHigh, "Price high is required.");
        }

        [Fact]
        public void Validate_That_PriceHigh_Is_Greater_Than_Zero()
        {
            // Arrange
            var priceSeries = PriceSeriesBuilder.Range.New();
            priceSeries.PriceHigh = 0;

            // Act
            var result = PriceSeriesValidator.Validate(priceSeries);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ShouldHaveValidationErrorFor<PriceSeries>(series => series.PriceHigh, "Price high must be greater than 0.");
        }

        [Fact]
        public void Validate_That_PriceHigh_Is_Greater_Than_PriceLow()
        {
            // Arrange
            var priceSeries = PriceSeriesBuilder.Range.New();
            priceSeries.PriceLow = 10;
            priceSeries.PriceHigh = 5;

            // Act
            var result = PriceSeriesValidator.Validate(priceSeries);

            // Assert
            result.ShouldHaveValidationErrorFor<PriceSeries>(ps => ps.PriceHigh, "Price low must be less or equal than price high.");
        }
    }
}
