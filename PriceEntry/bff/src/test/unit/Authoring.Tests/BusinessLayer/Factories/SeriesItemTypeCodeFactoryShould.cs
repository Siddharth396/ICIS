namespace Authoring.Tests.BusinessLayer.Factories
{
    using System;

    using FluentAssertions;

    using global::BusinessLayer.PriceEntry.Services.Factories;
    using global::BusinessLayer.PriceEntry.ValueObjects;

    using Xunit;

    public class SeriesItemTypeCodeFactoryShould
    {
        public static readonly object[][] ValidSeriesItemTypeCodeData =
        {
            new object[] { SeriesItemTypeCode.SingleValueWithReference, SeriesItemTypeCode.SingleValueWithReferenceSeries },
            new object[] { SeriesItemTypeCode.Range, SeriesItemTypeCode.RangeSeries }
        };

        [Theory]
        [MemberData(nameof(ValidSeriesItemTypeCodeData))]
        public void Return_Correct_SeriesItemTypeCode(string inputCode, SeriesItemTypeCode expectedCode)
        {
            // Act
            var result = SeriesItemTypeCodeFactory.GetSeriesItemTypeCode(inputCode);

            // Assert
            result.Should().Be(expectedCode);
        }

        [Fact]
        public void Throw_ArgumentException_When_Unsupported_Code_Is_Provided()
        {
            // Arrange
            var unsupportedCode = "UnsupportedCode";

            // Act
            var action = new Action(() => SeriesItemTypeCodeFactory.GetSeriesItemTypeCode(unsupportedCode));

            // Assert
            action.Should().Throw<ArgumentException>().WithMessage($"Series item type code {unsupportedCode} is not supported");
        }
    }
}
