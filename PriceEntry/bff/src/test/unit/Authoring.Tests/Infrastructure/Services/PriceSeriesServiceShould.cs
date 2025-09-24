namespace Authoring.Tests.Infrastructure.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using global::BusinessLayer.PriceEntry.Repositories.Models;
    using global::BusinessLayer.PriceEntry.ValueObjects;
    using global::BusinessLayer.PriceSeriesSelection.Repositories.Models;

    using Xunit;

    using Name = global::BusinessLayer.PriceEntry.Repositories.Models.Name;
    using PriceCategory = global::BusinessLayer.PriceSeriesSelection.Repositories.Models.PriceCategory;
    using PriceDerivationType = global::BusinessLayer.PriceEntry.ValueObjects.PriceDerivationType;
    using PriceDerivationTypeModel = global::BusinessLayer.PriceEntry.Repositories.Models.PriceDerivationType;

    public class PriceSeriesServiceShould
    {
        public static IEnumerable<object[]> GetTestData = new[]
        {
            new object[] { PriceCategoryCode.Derived.Value, PriceDerivationType.Index.Value, DerivationFunctionKey.RegionalAvgFunctionKey.Value, "Asia East", true },
            new object[] { PriceCategoryCode.Derived.Value, PriceDerivationType.Index.Value, DerivationFunctionKey.RegionalAvgFunctionKey.Value, "MENA", false },
            new object[] { PriceCategoryCode.Derived.Value, PriceDerivationType.Spread.Value, DerivationFunctionKey.RegionalAvgFunctionKey.Value, "Asia East", false },
            new object[] { PriceCategoryCode.Derived.Value, PriceDerivationType.Index.Value, string.Empty, "Asia East", false },
            new object[] { PriceCategoryCode.Assessed.Value, string.Empty, string.Empty, "Asia East", false }
        };

        [Theory]
        [MemberData(nameof(GetTestData))]
        public async Task Should_Return_True_If_Valid_Derived_Series(string priceCategoryCode, string priceDerivationType, string derivationFunctionKey, string locationName, bool shouldCalculate)
        {
            // Arrange
            var inputDerivedSeries = GetPriceSeries(priceCategoryCode, priceDerivationType, derivationFunctionKey, locationName);

            // Act
            var result = PriceSeries.IsValidDerivedSeries(inputDerivedSeries);

            // Assert
            Assert.Equal(shouldCalculate, result);
        }

        private PriceSeries GetPriceSeries(
            string priceCategoryCode,
            string priceDerivationType,
            string derivationFunctionKey,
            string locationName)
        {
            return new PriceSeries
            {
                PriceCategory = new PriceCategory() { Code = priceCategoryCode },
                PriceDerivationType = new PriceDerivationTypeModel() { Code = priceDerivationType },
                DerivationInputs = string.IsNullOrWhiteSpace(derivationFunctionKey)
                   ? null
                   :
                   [
                       new DerivationInput
                       {
                           DerivationFunctionKey = derivationFunctionKey,
                           ParameterIndex = 0,
                           SeriesId = "some-input-series-id",
                           ParameterBaseWeight = 1
                       }

                   ],
                Location = GetLocation(locationName),
                SeriesItemTypeCode = SeriesItemTypeCode.SingleValueWithReference
            };
        }

        private Location GetLocation(string locationName)
        {
            switch (locationName)
            {
                case "Asia East":
                    return new Location()
                    {
                        Name = new Name() { English = locationName },
                        Guid = new Guid("f5705134-98a1-487d-ba79-399438c34214"),
                    };
                case "MENA":
                    return new Location()
                    {
                        Name = new Name() { English = locationName },
                        Guid = new Guid("3be32bc1-449d-4065-9db6-b3b43b7c29ca"),
                    };
                default:
                    return new Location()
                    {
                        Name = new Name() { English = locationName },
                        Guid = new Guid("f5705134-98a1-487d-ba79-399438c34214"),
                    };
            }
        }
    }
}
