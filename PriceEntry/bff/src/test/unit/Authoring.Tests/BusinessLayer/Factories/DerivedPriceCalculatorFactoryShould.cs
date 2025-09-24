namespace Authoring.Tests.BusinessLayer.Factories
{
    using System;

    using FluentAssertions;

    using global::BusinessLayer.PriceEntry.Services.Factories;
    using global::BusinessLayer.PriceEntry.ValueObjects;

    using NSubstitute;

    using Xunit;

    public class DerivedPriceCalculatorFactoryShould
    {
        [Fact]
        public void Throw_ArgumentException_For_Unknown_Type()
        {
            // Arrange
            var factory = new DerivedPriceCalculatorFactory(Substitute.For<IServiceProvider>());

            // Act
            Action action = () => factory.GetDerivedPriceCalculator(new DerivationFunctionKey("Unknown"));

            // Assert
            action.Should().Throw<ArgumentException>();
        }
    }
}
