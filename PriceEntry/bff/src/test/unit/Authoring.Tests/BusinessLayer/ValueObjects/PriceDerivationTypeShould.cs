namespace Authoring.Tests.BusinessLayer.ValueObjects
{
    using FluentAssertions;

    using global::BusinessLayer.PriceEntry.ValueObjects;

    using Xunit;

    public class PriceDerivationTypeShould
    {
        public static TheoryData<PriceDerivationType, string, bool> PriceDerivationTypesValidationData =>
           new()
           {
                { PriceDerivationType.Average, "AVERAGE", true },
                { PriceDerivationType.Spread, "SPREAD", true },
                { PriceDerivationType.Converted, "CONVERTED", true },
                { PriceDerivationType.Index, "INDEX", true },
                { PriceDerivationType.Forecast, "FORECAST", true },
                { PriceDerivationType.Forecast, "forecast", false },
                { PriceDerivationType.Index, "index", false },
           };

        [Theory]
        [MemberData(nameof(PriceDerivationTypesValidationData))]
        public void Calculate_Correctly_When_Matches_Is_Called(PriceDerivationType priceDerivationType, string type, bool expected)
        {
            var actual = priceDerivationType.Matches(type);

            actual.Should().Be(expected);
        }
    }
}
