namespace Authoring.Tests.BusinessLayer.Validators
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

    using global::BusinessLayer.ContentBlock.DTOs.Input;
    using global::BusinessLayer.PriceSeriesSelection.Repositories.Models;
    using global::BusinessLayer.PriceSeriesSelection.Validators;

    using Xunit;

    public class PriceSeriesInputValidatorShould
    {
        [Fact]
        public void Return_Error_When_Content_Block_Having_Price_Series_From_Multiple_Commodities_Are_Found()
        {
            // Arrange
            var priceSeries = new List<PriceSeries>
            {
                new() { Id = "1", Commodity = new Commodity { Guid = Guid.Parse("f63f3d4b-132b-4f32-8c95-b5ab8e061565"), Name = new Name() { English = "Commodity1" } }, SeriesItemTypeCode = "pi-range" },
                new() { Id = "2", Commodity = new Commodity { Guid = Guid.Parse("0111542d-90dd-4627-afe4-9e4d9bd7ae0a"), Name = new Name() { English = "Commodity2" } }, SeriesItemTypeCode = "pi-range" },
                new() { Id = "3", Commodity = new Commodity { Guid = Guid.Parse("1ed22a9e-24ac-465c-982c-3bf6b6a1d73f"), Name = new Name() { English = "Commodity3" } }, SeriesItemTypeCode = "pi-range" }
            };

            var priceSeriesGrids = new List<PriceSeriesGrid>
            {
                 new() { Id = "1", Title = "1",  PriceSeriesIds = new List<string> { "1" } },
                 new() { Id = "2", Title = "2",  PriceSeriesIds = new List<string> { "2", "3" } }
            };

            var validator = new PriceSeriesGridsValidator(priceSeries, priceSeriesGrids);

            // Act
            var result = validator.Validate();

            // Assert
            result.Should().NotBeNull();
            result.ErrorCodes.Should().Contain("MULTIPLE_COMMODITIES_IN_CONTENT_BLOCK");
        }

        [Fact]
        public void Return_Error_When_Price_Series_With_Different_Series_Item_Type_Codes_Are_Found()
        {
            // Arrange
            var priceSeries = new List<PriceSeries>
            {
                new() { Id = "1", Commodity = new Commodity { Guid = Guid.Parse("f63f3d4b-132b-4f32-8c95-b5ab8e061565"), Name = new Name() { English = "Commodity1" } }, SeriesItemTypeCode = "pi-range" },
                new() { Id = "2", Commodity = new Commodity { Guid = Guid.Parse("f63f3d4b-132b-4f32-8c95-b5ab8e061565"), Name = new Name() { English = "Commodity1" } }, SeriesItemTypeCode = "pi-single-with-ref" },
                new() { Id = "3", Commodity = new Commodity { Guid = Guid.Parse("f63f3d4b-132b-4f32-8c95-b5ab8e061565"), Name = new Name() { English = "Commodity1" } }, SeriesItemTypeCode = "pi-single-with-ref" }
            };

            var priceSeriesGrids = new List<PriceSeriesGrid>
            {
                 new() { Id = "1", Title = "1",  PriceSeriesIds = new List<string> { "1", "2", "3" } },
            };

            var validator = new PriceSeriesGridsValidator(priceSeries, priceSeriesGrids);

            // Act
            var result = validator.Validate();

            // Assert
            result.Should().NotBeNull();
            result.ErrorCodes.Should().Contain("MULTIPLE_SERIES_ITEM_TYPE_CODES");
            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public void Return_Error_When_Price_Series_With_Different_PublicationSchedules_Are_Found()
        {
            // Arrange
            var priceSeries = new List<PriceSeries>
            {
                new()
                {
                    Id = "1",
                    Commodity = new Commodity
                    {
                        Guid = Guid.Parse("f63f3d4b-132b-4f32-8c95-b5ab8e061565"),
                        Name = new Name() { English = "Commodity1" }
                    },
                    SeriesItemTypeCode = "pi-single-with-ref",
                    PublicationSchedules = new List<PublicationSchedule> { new() { ScheduleId = "1" } }
                },
                new()
                {
                    Id = "2",
                    Commodity = new Commodity
                    {
                        Guid = Guid.Parse("f63f3d4b-132b-4f32-8c95-b5ab8e061565"),
                        Name = new Name() { English = "Commodity1" }
                    },
                    SeriesItemTypeCode = "pi-single-with-ref",
                    PublicationSchedules = new List<PublicationSchedule> { new() { ScheduleId = "2" } }
                },
                new()
                {
                    Id = "3",
                    Commodity = new Commodity
                    {
                        Guid = Guid.Parse("f63f3d4b-132b-4f32-8c95-b5ab8e061565"),
                        Name = new Name() { English = "Commodity1" }
                    },
                    SeriesItemTypeCode = "pi-single-with-ref"
                }
            };

            var priceSeriesGrids = new List<PriceSeriesGrid>
            {
                 new() { Id = "1", Title = "1",  PriceSeriesIds = new List<string> { "1", "2", "3" } },
            };

            var validator = new PriceSeriesGridsValidator(priceSeries, priceSeriesGrids);

            // Act
            var result = validator.Validate();

            // Assert
            result.Should().NotBeNull();
            result.ErrorCodes.Should().Contain("MULTIPLE_SCHEDULES");
            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public void Return_Error_When_Grids_With_Duplicate_Price_Series_Are_Found()
        {
            // Arrange
            var priceSeries = new List<PriceSeries>
            {
                new()
                {
                    Id = "1",
                    Commodity = new Commodity
                    {
                        Guid = Guid.Parse("f63f3d4b-132b-4f32-8c95-b5ab8e061565"),
                        Name = new Name() { English = "Commodity1" }
                    },
                    SeriesItemTypeCode = "pi-single-with-ref",
                },
                new()
                {
                    Id = "2",
                    Commodity = new Commodity
                    {
                        Guid = Guid.Parse("f63f3d4b-132b-4f32-8c95-b5ab8e061565"),
                        Name = new Name() { English = "Commodity1" }
                    },
                    SeriesItemTypeCode = "pi-single-with-ref",
                },
                new()
                {
                    Id = "3",
                    Commodity = new Commodity
                    {
                        Guid = Guid.Parse("f63f3d4b-132b-4f32-8c95-b5ab8e061565"),
                        Name = new Name() { English = "Commodity1" }
                    },
                    SeriesItemTypeCode = "pi-single-with-ref"
                }
            };

            var priceSeriesGrids = new List<PriceSeriesGrid>
            {
                 new() { Id = "1", Title = "1",  PriceSeriesIds = new List<string> { "1", "2", "3" } },
                 new() { Id = "1", Title = "1",  PriceSeriesIds = new List<string> { "1" } },
            };

            var validator = new PriceSeriesGridsValidator(priceSeries, priceSeriesGrids);

            // Act
            var result = validator.Validate();

            // Assert
            result.Should().NotBeNull();
            result.ErrorCodes.Should().Contain("DUPLICATE_PRICE_SERIES_ID");
            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public void Return_Valid_Result_When_Single_Commodity_And_Single_Series_Item_Type_Code()
        {
            // Arrange
            var priceSeries = new List<PriceSeries>
            {
                new() { Id = "1", Commodity = new Commodity { Guid = Guid.Parse("f63f3d4b-132b-4f32-8c95-b5ab8e061565"), Name = new Name() { English = "Commodity1" } }, SeriesItemTypeCode = "pi-range" },
                new() { Id = "2", Commodity = new Commodity { Guid = Guid.Parse("f63f3d4b-132b-4f32-8c95-b5ab8e061565"), Name = new Name() { English = "Commodity1" } }, SeriesItemTypeCode = "pi-range" },
                new() { Id = "3", Commodity = new Commodity { Guid = Guid.Parse("f63f3d4b-132b-4f32-8c95-b5ab8e061565"), Name = new Name() { English = "Commodity1" } }, SeriesItemTypeCode = "pi-range" }
            };

            var priceSeriesGrids = new List<PriceSeriesGrid>
            {
                 new() { Id = "1", Title = "1",  PriceSeriesIds = new List<string> { "1" } },
                 new() { Id = "2", Title = "2",  PriceSeriesIds = new List<string> { "2", "3" } }
            };

            var validator = new PriceSeriesGridsValidator(priceSeries, priceSeriesGrids);

            // Act
            var result = validator.Validate();

            // Assert
            result.Should().NotBeNull();
            result.ErrorCodes.Should().BeEmpty();
            result.IsValid.Should().BeTrue();
        }
    }
}
