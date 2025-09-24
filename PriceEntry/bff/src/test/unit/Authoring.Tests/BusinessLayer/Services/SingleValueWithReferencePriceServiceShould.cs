namespace Authoring.Tests.BusinessLayer.Services
{
    using System;

    using FluentAssertions;

    using global::BusinessLayer.PriceEntry.Repositories.Models;
    using global::BusinessLayer.PriceEntry.Services.SeriesItemTypes.SingleValueWithReference;

    using Xunit;

    public class SingleValueWithReferencePriceServiceShould
    {
        [Theory]
        [InlineData("Method1", "Method2", true)]
        [InlineData("Method1", null, true)]
        [InlineData("Method1", "Method1", false)]
        [InlineData(null, "Method1", false)]
        [InlineData(null, null, false)]
        public void Calculate_Correctly_If_Assessment_Method_Changed(
            string oldAssessmentMethod,
            string newAssessmentMethod,
            bool returnValue)
        {
            // Arrange
            var oldPriceItem =
                new PriceItemSingleValueWithReference
                {
                    AssessmentMethod = oldAssessmentMethod
                };

            var newPriceItem =
                new PriceItemSingleValueWithReference
                {
                    AssessmentMethod = newAssessmentMethod
                };

            // Act
            var result = SingleValueWithReferencePriceService.HasAssessmentMethodChanged(newPriceItem, oldPriceItem);

            // Assert
            result.Should().Be(returnValue);
        }

        [Fact]
        public void Reset_Price_Item_Fields_To_Null_When_Assessment_Method_Changes()
        {
            // Arrange
            var priceItem = new PriceItemSingleValueWithReference
            {
                Price = 10.0m,
                ReferencePrice = new ReferencePrice
                {
                    Datetime = DateTime.Now,
                    Market = "TTF",
                    PeriodLabel = "August 2024",
                    Price = 20m,
                    SeriesName = "ICIS Heren® Natural Gas EoD Assessment - TTF Conv. USD/MMBtu"
                },
                PremiumDiscount = 0.5m
            };

            // Act
            SingleValueWithReferencePriceService.ResetFieldsOnAssessmentMethodChanged(priceItem);

            // Assert
            priceItem.Price.Should().BeNull();
            priceItem.ReferencePrice.Should().BeNull();
            priceItem.PremiumDiscount.Should().BeNull();
        }
    }
}
