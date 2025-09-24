namespace Test.Infrastructure.Common
{
    using FluentAssertions;

    using NetArchTest.Rules;

    public static class TestResultExtensions
    {
        public static void ShouldBeSuccessful(this TestResult result)
        {
            if (!result.IsSuccessful)
            {
                result.FailingTypeNames.Should().BeEmpty();
            }
        }
    }
}