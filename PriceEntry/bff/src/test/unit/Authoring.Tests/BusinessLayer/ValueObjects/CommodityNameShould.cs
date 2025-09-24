namespace Authoring.Tests.BusinessLayer.ValueObjects
{
    using FluentAssertions;

    using global::BusinessLayer.PriceEntry.ValueObjects;

    using Xunit;

    public class CommodityNameShould
    {
        public static TheoryData<CommodityName, string, bool> CommodityNamesValidationData =>
            new()
            {
                { CommodityName.LNG, "lng", true },
                { CommodityName.LNG, "LNG", false },
                { CommodityName.Melamine, "melamine", true },
                { CommodityName.Melamine, "Melamine", false },
                { CommodityName.Styrene, "styrene", true },
                { CommodityName.Styrene, "Styrene", false }
            };

        [Theory]
        [MemberData(nameof(CommodityNamesValidationData))]
        public void Calculate_Correctly_When_Matches_Is_Called(CommodityName commodityName, string name, bool expected)
        {
            var actual = commodityName.Matches(name);

            actual.Should().Be(expected);
        }
    }
}
