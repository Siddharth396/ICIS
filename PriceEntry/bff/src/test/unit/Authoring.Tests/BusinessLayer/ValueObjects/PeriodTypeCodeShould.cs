namespace Authoring.Tests.BusinessLayer.ValueObjects
{
    using FluentAssertions;

    using global::BusinessLayer.PriceEntry.ValueObjects;

    using Xunit;

    public class PeriodTypeCodeShould
    {
        public static TheoryData<PeriodTypeCode, string, bool> PeriodTypeCodesValidationData =>
            new()
            {
                { PeriodTypeCode.HalfMonth, "HM", true },
                { PeriodTypeCode.HalfMonth, "hm", false },
                { PeriodTypeCode.MidMonth, "MM", true },
                { PeriodTypeCode.MidMonth, "mm", false }
            };

        [Theory]
        [MemberData(nameof(PeriodTypeCodesValidationData))]
        public void Calculate_Correctly_When_Matches_Is_Called(PeriodTypeCode periodTypeCode, string code, bool expected)
        {
            var actual = periodTypeCode.Matches(code);

            actual.Should().Be(expected);
        }
    }
}
