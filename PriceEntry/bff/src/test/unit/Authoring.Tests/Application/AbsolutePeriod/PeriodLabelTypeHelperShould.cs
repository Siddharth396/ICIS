namespace Authoring.Tests.Application.AbsolutePeriod
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using FluentAssertions;

    using global::BusinessLayer.PriceEntry.ValueObjects;
    using global::BusinessLayer.PriceSeriesSelection.Repositories.Models;

    using NSubstitute;

    using Serilog;

    using Xunit;

    using static global::BusinessLayer.Helpers.PeriodLabelTypeHelper;

    public class PeriodLabelTypeHelperShould
    {
        private readonly ILogger loggerMock;

        public PeriodLabelTypeHelperShould()
        {
            loggerMock = Substitute.For<ILogger>();
        }

        public static IEnumerable<object?[]> FiltersBasedOnReferenceData =>
            [
                ["M", new DateOnly(2024, 2, 5)],
                ["Q", new DateOnly(2024, 4, 5)]
            ];

        [Theory]
        [MemberData(nameof(FiltersBasedOnReferenceData))]
        public void Return_Correct_ReferenceDate_When_LastPublishedAppliesFromDateTime_Is_Available_For_ReferencePeriod(string code, DateOnly expectedReferenceDate)
        {
            // Arrange
            var priceSeries = new PriceSeries
            {
                Id = "S1",
                PeriodLabelTypeCode = "plt-ref-time",
                ReferencePeriod = new ReferencePeriod { Code = code },
            };

            var assessedDateTime = new DateTime(2024, 1, 1);
            var lastPublishedAppliesFromDateTime = new DateTime(2024, 1, 5);

            // Act
            var result = GetAbsolutePeriodCalculationInput(
                                                            loggerMock,
                                                            priceSeries,
                                                            assessedDateTime,
                                                            lastPublishedAppliesFromDateTime);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedReferenceDate, result.Value.ReferenceDate);
        }

        [Fact]
        public void Return_Correct_ReferenceDate_And_PeriodCode_While_Getting_RelativeFulfilmentPeriodCalculationInput()
        {
            // Arrange
            var priceSeries = new PriceSeries
            {
                Id = "S1",
                PeriodLabelTypeCode = "plt-ffmt-time",
                RelativeFulfilmentPeriod = new RelativeFulfilmentPeriod { Code = "MM1" },
            };

            var assessedDateTime = new DateTime(2024, 1, 1);

            // Act
            var result = GetAbsolutePeriodCalculationInput(
                                                            loggerMock,
                                                            priceSeries,
                                                            assessedDateTime,
                                                            null);

            result.Should().Be((DateOnly.FromDateTime(assessedDateTime), "MM1"));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData(" ")]
        [InlineData("Invalid")]
        public void While_Getting_RelativeFulfilmentPeriodCalculationInput_Return_Null_When_fulfilmentPeriodCode_Is(string? fulfilmentPeriodCode)
        {
            // Arrange
            var priceSeries = new PriceSeries
            {
                Id = "S1",
                PeriodLabelTypeCode = "plt-ffmt-time",
                RelativeFulfilmentPeriod = fulfilmentPeriodCode is not null ? new RelativeFulfilmentPeriod { Code = fulfilmentPeriodCode } : null,
            };

            var assessedDateTime = new DateTime(2024, 1, 1);

            // Act
            var result = GetAbsolutePeriodCalculationInput(
                                                            loggerMock,
                                                            priceSeries,
                                                            assessedDateTime,
                                                            null);

            result.Should().BeNull();
        }

        [Fact]
        public void Return_AssessedDate_As_ReferenceDate_When_LastPublishedAppliesFromDateTime_Is_Null_For_ReferencePeriod()
        {
            // Arrange
            var priceSeries = new PriceSeries
            {
                Id = "S1",
                PeriodLabelTypeCode = "plt-ref-time",
                ReferencePeriod = new ReferencePeriod { Code = "M" },
            };

            var assessedDateTime = new DateTime(2024, 1, 1);

            // Act
            var result = GetAbsolutePeriodCalculationInput(
                                                            loggerMock,
                                                            priceSeries,
                                                            assessedDateTime,
                                                            null);

            result!.Value.ReferenceDate.Should().Be(DateOnly.FromDateTime(assessedDateTime));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData(" ")]
        [InlineData("Invalid")]
        public void Return_Null_When_ReferencePeriodCode_Is(string? referencePeriodCode)
        {
            // Arrange
            var priceSeries = new PriceSeries
            {
                Id = "S1",
                PeriodLabelTypeCode = "plt-ref-time",
                ReferencePeriod = referencePeriodCode is not null ? new ReferencePeriod { Code = referencePeriodCode } : null,
            };

            var assessedDateTime = new DateTime(2024, 1, 1);

            // Act
            var result = GetAbsolutePeriodCalculationInput(
                                                            loggerMock,
                                                            priceSeries,
                                                            assessedDateTime,
                                                            null);

            result.Should().BeNull();
        }

        [Theory]
        [InlineData("plt-none")]
        [InlineData("Invalid")]
        [InlineData(" ")]
        public void Return_Null_When_PeriodLabelTypeCode_Is(string? periodLabelTypeCode)
        {
            // Arrange
            var priceSeries = new PriceSeries
            {
                Id = "S1",
                PeriodLabelTypeCode = periodLabelTypeCode
            };

            var assessedDateTime = new DateTime(2024, 1, 1);

            // Act
            var result = GetAbsolutePeriodCalculationInput(
                                                            loggerMock,
                                                            priceSeries,
                                                            assessedDateTime,
                                                            null);

            result.Should().BeNull();
        }

        [Fact]
        public void Return_Null_When_ReferencePeriodCodeIsAllowed_But_NoCalculatorExists()
        {
            // Arrange
            var priceSeries = new PriceSeries
            {
                Id = "S1",
                PeriodLabelTypeCode = "plt-ref-time",
                ReferencePeriod = new ReferencePeriod { Code = "TEST" },
            };

            var assessedDateTime = new DateTime(2024, 1, 1);
            var lastPublishedAppliesFromDateTime = new DateTime(2024, 1, 5);

            var field = typeof(ReferencePeriodCode)
                       .GetField("AllowedPeriodCodes", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);

            if (field!.GetValue(null) is HashSet<string> set)
            {
                set.Add("TEST");
            }

            // Act
            var result = GetAbsolutePeriodCalculationInput(
                                                            loggerMock,
                                                            priceSeries,
                                                            assessedDateTime,
                                                            lastPublishedAppliesFromDateTime);

            result.Should().BeNull();
        }
    }
}
