namespace Authoring.Tests.BusinessLayer.ValueObjects
{
    using FluentAssertions;

    using global::BusinessLayer.PriceEntry.ValueObjects;

    using Xunit;

    public class AssessmentMethodShould
    {
        public static TheoryData<AssessmentMethod, string, bool> AssessmentMethodsValidationData =>
            new()
            {
                { AssessmentMethod.PremiumDiscount, "Premium/Discount", true },
                { AssessmentMethod.PremiumDiscount, "premium/discount", false },
                { AssessmentMethod.PremiumDiscount, "Assessed", false },
                { AssessmentMethod.Assessed, "Assessed", true },
                { AssessmentMethod.Assessed, "assessed", false },
                { AssessmentMethod.Assessed, "PremiumDiscount", false }
            };

        [Theory]
        [MemberData(nameof(AssessmentMethodsValidationData))]
        public void Calculate_Correctly_When_Matches_Is_Called(AssessmentMethod assessmentMethod, string method, bool expected)
        {
            var actual = assessmentMethod.Matches(method);

            actual.Should().Be(expected);
        }
    }
}
