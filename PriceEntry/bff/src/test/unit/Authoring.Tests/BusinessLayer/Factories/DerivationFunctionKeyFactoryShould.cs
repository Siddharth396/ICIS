namespace Authoring.Tests.BusinessLayer.Factories
{
    using System;

    using FluentAssertions;

    using global::BusinessLayer.PriceEntry.Services.Factories;
    using global::BusinessLayer.PriceEntry.ValueObjects;

    using Xunit;

    public class DerivationFunctionKeyFactoryShould
    {
        public static readonly object[][] ValidDerivationFunctionKeys =
        {
            new object[] { DerivationFunctionKey.RegionalAvg, DerivationFunctionKey.RegionalAvgFunctionKey },
            new object[] { DerivationFunctionKey.PeriodAvg, DerivationFunctionKey.PeriodAvgFunctionKey },
        };

        [Theory]
        [MemberData(nameof(ValidDerivationFunctionKeys))]
        public void Return_Correct_Derivation_Function_Key(string inputKey, DerivationFunctionKey expectedKey)
        {
            // Act
            var result = DerivationFunctionKeyFactory.GetDerivationFunctionKey(inputKey);

            // Assert
            result.Should().Be(expectedKey);
        }

        [Fact]
        public void Throw_ArgumentException_When_Unsupported_DerivationFunctionKey_Is_Provided()
        {
            // Arrange
            var unsupportedKey = "UnsupportedKey";

            // Act
            var action = new Action(() => DerivationFunctionKeyFactory.GetDerivationFunctionKey(unsupportedKey));

            // Assert
            action.Should().Throw<ArgumentException>().WithMessage($"Derivation function key {unsupportedKey} is not supported");
        }
    }
}
