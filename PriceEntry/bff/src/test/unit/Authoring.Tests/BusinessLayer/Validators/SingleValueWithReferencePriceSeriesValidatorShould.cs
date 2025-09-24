namespace Authoring.Tests.BusinessLayer.Validators
{
    using System;

    using FluentAssertions;

    using global::BusinessLayer.PriceEntry.DTOs.Output;
    using global::BusinessLayer.PriceEntry.Validators;
    using global::BusinessLayer.PriceEntry.ValueObjects;

    using Xunit;

    public class SingleValueWithReferencePriceSeriesValidatorShould
    {
        public static TheoryData<PriceSeries> AllAssessmentMethodData =>
            new()
            {
                PriceSeriesBuilder.SingleValueWithReference.NewAssessed(),
                PriceSeriesBuilder.SingleValueWithReference.NewPremiumDiscount()
            };

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

        [Theory]
        [MemberData(nameof(AllAssessmentMethodData))]
        public void Validate_That_AssessmentMethod_Is_Invalid(PriceSeries priceSeries)
        {
            // Arrange
            var assessmentMethod = "Invalid Method";
            priceSeries.AssessmentMethod = assessmentMethod;

            // Act
            var result = PriceSeriesValidator.Validate(priceSeries);

            // Assert
            result.ShouldHaveValidationErrorFor<PriceSeries>(
                ps => ps.AssessmentMethod,
                $"Assessment method '{assessmentMethod}' is invalid.");
        }

        [Theory]
        [MemberData(nameof(AllAssessmentMethodData))]
        public void Validate_That_AssessmentMethod_Is_Required(PriceSeries priceSeries)
        {
            // Arrange
            priceSeries.AssessmentMethod = null;

            // Act
            var result = PriceSeriesValidator.Validate(priceSeries);

            // Assert
            result.ShouldHaveValidationErrorFor<PriceSeries>(
                ps => ps.AssessmentMethod,
                "Assessment method is required.");
        }

        [Theory]
        [MemberData(nameof(AllAssessmentMethodData))]
        public void Validate_That_DataUsed_Is_Required(PriceSeries priceSeries)
        {
            // Arrange
            priceSeries.DataUsed = null;

            // Act
            var result = PriceSeriesValidator.Validate(priceSeries);

            // Assert
            result.ShouldHaveValidationErrorFor<PriceSeries>(ps => ps.DataUsed, "Data used is required.");
        }

        [Theory]
        [MemberData(nameof(AllAssessmentMethodData))]
        public void Validate_That_DataUsed_Is_Valid(PriceSeries priceSeries)
        {
            // Arrange
            var dataUsed = "Invalid data";
            priceSeries.DataUsed = dataUsed;

            // Act
            var result = PriceSeriesValidator.Validate(priceSeries);

            // Assert
            result.ShouldHaveValidationErrorFor<PriceSeries>(ps => ps.DataUsed, $"Data used '{dataUsed}' is invalid.");
        }

        [Fact]
        public void Validate_That_PremiumDiscount_Is_Required_When_AssessmentMethod_Is_PremiumDiscount()
        {
            // Arrange
            var priceSeries = PriceSeriesBuilder.SingleValueWithReference.NewPremiumDiscount();
            priceSeries.PremiumDiscount = null;

            // Act
            var result = PriceSeriesValidator.Validate(priceSeries);

            // Assert
            result.ShouldHaveValidationErrorFor<PriceSeries>(ps => ps.PremiumDiscount, "Premium/Discount is required.");
        }

        [Fact]
        public void Validate_That_ReferencePrice_Is_Required_When_AssessmentMethod_Is_PremiumDiscount()
        {
            // Arrange
            var priceSeries = PriceSeriesBuilder.SingleValueWithReference.NewPremiumDiscount();
            priceSeries.ReferencePrice = null;

            // Act
            var result = PriceSeriesValidator.Validate(priceSeries);

            // Assert
            result.ShouldHaveValidationErrorFor<PriceSeries>(ps => ps.ReferencePrice, "Reference price is required.");
        }

        [Theory]
        [InlineData(-5)]
        [InlineData(0)]
        public void Validate_That_Price_Is_Valid_When_AssessmentMethod_Is_Assessed(double price)
        {
            // Arrange
            var priceSeries = PriceSeriesBuilder.SingleValueWithReference.NewAssessed();
            priceSeries.Price = Convert.ToDecimal(price);

            // Act
            var result = PriceSeriesValidator.Validate(priceSeries);

            // Assert
            result.ShouldHaveValidationErrorFor<PriceSeries>(ps => ps.Price, "Price must be greater than 0.");
        }

        [Theory]
        [InlineData(-5)]
        [InlineData(0)]
        public void Validate_That_ReferencePrice_Is_Valid_When_AssessmentMethod_Is_PremiumDiscount(
            double referencePrice)
        {
            // Arrange
            var priceSeries = PriceSeriesBuilder.SingleValueWithReference.NewPremiumDiscount();
            priceSeries.ReferencePrice!.Price = Convert.ToDecimal(referencePrice);

            // Act
            var result = PriceSeriesValidator.Validate(priceSeries);

            // Assert
            result.ShouldHaveValidationErrorFor<PriceSeries>(
                ps => ps.ReferencePrice,
                "Selected reference price could not be found for the date, please ensure it is added.");
        }
    }
}
