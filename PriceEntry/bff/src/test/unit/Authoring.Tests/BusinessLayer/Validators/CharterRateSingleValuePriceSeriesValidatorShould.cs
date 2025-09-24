namespace Authoring.Tests.BusinessLayer.Validators
{
    using FluentAssertions;

    using global::BusinessLayer.PriceEntry.DTOs.Output;
    using global::BusinessLayer.PriceEntry.Validators;
    using global::BusinessLayer.PriceEntry.ValueObjects;

    using Xunit;

    public class CharterRateSingleValuePriceSeriesValidatorShould
    {
        [Fact]
        public void Allow_All_Fields_To_Be_Missing()
        {
            // Arrange
            var priceSeries = new PriceSeries
            {
                SeriesItemTypeCode = SeriesItemTypeCode.SingleValueWithReference
            };

            // Act
            var result = PriceSeriesValidator.Validate(priceSeries);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_That_Price_Is_Required()
        {
            // Arrange
            var priceSeries = PriceSeriesBuilder.CharterRateSingleValue.New();
            priceSeries.Price = null;

            // Act
            var result = PriceSeriesValidator.Validate(priceSeries);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ShouldHaveValidationErrorFor<PriceSeries>(series => series.Price, "Price is required.");
        }

        [Fact]
        public void Validate_That_DataUsed_Is_Required()
        {
            // Arrange
            var priceSeries = PriceSeriesBuilder.CharterRateSingleValue.New();
            priceSeries.DataUsed = null;

            // Act
            var result = PriceSeriesValidator.Validate(priceSeries);

            // Assert
            result.ShouldHaveValidationErrorFor<PriceSeries>(ps => ps.DataUsed, "Data used is required.");
        }

        [Fact]
        public void Validate_That_DataUsed_Is_Valid()
        {
            // Arrange
            var priceSeries = PriceSeriesBuilder.CharterRateSingleValue.New();
            var dataUsed = "Invalid data";
            priceSeries.DataUsed = dataUsed;

            // Act
            var result = PriceSeriesValidator.Validate(priceSeries);

            // Assert
            result.ShouldHaveValidationErrorFor<PriceSeries>(ps => ps.DataUsed, $"Data used '{dataUsed}' is invalid.");
        }
    }
}
