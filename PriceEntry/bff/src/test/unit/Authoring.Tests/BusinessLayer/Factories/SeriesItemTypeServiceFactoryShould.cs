namespace Authoring.Tests.BusinessLayer.Factories
{
    using System;

    using FluentAssertions;

    using global::BusinessLayer.PriceEntry.Services.Factories;
    using global::BusinessLayer.PriceEntry.ValueObjects;

    using NSubstitute;

    using Xunit;

    public class SeriesItemTypeServiceFactoryShould
    {
        [Fact]
        public void Throw_ArgumentException_For_Unknown_Type()
        {
            // Arrange
            var factory = new SeriesItemTypeServiceFactory(Substitute.For<IServiceProvider>());

            // Act
            Action action = () => factory.GetPriceItemService(new SeriesItemTypeCode("Unknown"));

            // Assert
            action.Should().Throw<ArgumentException>();
        }
    }
}
