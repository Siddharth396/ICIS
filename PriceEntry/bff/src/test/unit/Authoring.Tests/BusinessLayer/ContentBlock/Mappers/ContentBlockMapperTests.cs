namespace Authoring.Tests.BusinessLayer.ContentBlock.Mappers
{
    using System;

    using FluentAssertions;

    using global::BusinessLayer.Common.Mappers;
    using global::BusinessLayer.ContentBlock.Mappers;
    using global::BusinessLayer.ContentBlock.Repositories.Models;
    using global::Infrastructure.MongoDB.Models;

    using NSubstitute;

    using Test.Infrastructure.TestData;

    using Xunit;

    using PriceSeriesGridDatabaseModel = global::BusinessLayer.ContentBlock.Repositories.Models.PriceSeriesGrid;
    using PriceSeriesGridOutputModel = global::BusinessLayer.ContentBlock.DTOs.Output.PriceSeriesGrid;

    public class ContentBlockMapperTests
    {
        private readonly IModelMapper<PriceSeriesGridDatabaseModel, PriceSeriesGridOutputModel> priceSeriesGridMapper;

        public ContentBlockMapperTests()
        {
            priceSeriesGridMapper = Substitute.For<IModelMapper<PriceSeriesGridDatabaseModel, PriceSeriesGridOutputModel>>();
        }

        [Fact]
        public void Map_New_ContentBlock_Model_To_ContentBlockDefinition()
        {
            // Arrange
            var contentBlock = new ContentBlock
            {
                Id = "Id",
                ContentBlockId = "contentBlockId",
                Version = 1,
                Title = "Test Title",
                PriceSeriesGrids =
                [
                    new PriceSeriesGridDatabaseModel
                    {
                        PriceSeriesGridId = "PriceSeriesGridId",
                        Title = "Test Title",
                        PriceSeriesIds = [TestSeries.LNG_China_HM1, TestSeries.LNG_China_HM2, TestSeries.LNG_China_HM3],
                        SeriesItemTypeCode = "pi-single-ref",
                    }
                ],
                LastModified = new AuditInfo
                {
                    User = "TestUser",
                    Timestamp = new DateTime(2025, 2, 28, 15, 0, 0)
                }
            };

            priceSeriesGridMapper
               .Map(Arg.Any<PriceSeriesGridDatabaseModel>())
               .Returns(
                    new PriceSeriesGridOutputModel
                    {
                        Id = "PriceSeriesGridId",
                        Title = "Test Grid Title",
                        PriceSeriesIds = [TestSeries.LNG_China_HM1, TestSeries.LNG_China_HM2, TestSeries.LNG_China_HM3],
                        SeriesItemTypeCode = "pi-single-ref"
                    });

            var contentBlockMapper = new ContentBlockMapper(priceSeriesGridMapper);

            // Act
            var result = contentBlockMapper.Map(contentBlock);

            // Assert
            result.PriceSeriesGrids.Should().NotBeNull();
            result.PriceSeriesGrids.Should().HaveCount(1);

            result.PriceSeriesGrids?[0].PriceSeriesIds.Should().HaveCount(3);
            result.PriceSeriesGrids?[0].SeriesItemTypeCode.Should().Be("pi-single-ref");
            result.PriceSeriesGrids?[0].Id.Should().Be("PriceSeriesGridId");
            result.PriceSeriesGrids?[0].Title.Should().Be("Test Grid Title");
        }

        [Fact]
        public void Map_New_ContentBlock_Model_With_Multiple_Grids_To_ContentBlockDefinition()
        {
            // Arrange
            var priceSeriesGrid1 = new PriceSeriesGridDatabaseModel
            {
                PriceSeriesGridId = "PriceSeriesGridId1",
                Title = "Test Grid Title 1",
                PriceSeriesIds = [TestSeries.LNG_China_HM1, TestSeries.LNG_China_HM2, TestSeries.LNG_China_HM3],
                SeriesItemTypeCode = "pi-single-ref",
            };

            var priceSeriesGrid2 = new PriceSeriesGridDatabaseModel
            {
                PriceSeriesGridId = "PriceSeriesGridId2",
                Title = "Test Grid Title 2",
                PriceSeriesIds = [TestSeries.LNG_China_MM1, TestSeries.LNG_China_MM2],
                SeriesItemTypeCode = "pi-single",
            };

            var contentBlock = new ContentBlock
            {
                Id = "Id",
                ContentBlockId = "contentBlockId",
                Version = 1,
                Title = "Test Title",
                PriceSeriesGrids = [priceSeriesGrid1, priceSeriesGrid2],
                LastModified = new AuditInfo
                {
                    User = "TestUser",
                    Timestamp = new DateTime(2025, 2, 28, 15, 0, 0)
                }
            };

            priceSeriesGridMapper
               .Map(priceSeriesGrid1)
               .Returns(
                    new PriceSeriesGridOutputModel
                    {
                        Id = "PriceSeriesGridId1",
                        Title = "Test Grid Title 1",
                        PriceSeriesIds = [TestSeries.LNG_China_HM1, TestSeries.LNG_China_HM2, TestSeries.LNG_China_HM3],
                        SeriesItemTypeCode = "pi-single-ref"
                    });

            priceSeriesGridMapper
               .Map(priceSeriesGrid2)
               .Returns(
                    new PriceSeriesGridOutputModel
                    {
                        Id = "PriceSeriesGridId2",
                        Title = "Test Grid Title 2",
                        PriceSeriesIds = [TestSeries.LNG_China_MM1, TestSeries.LNG_China_MM2],
                        SeriesItemTypeCode = "pi-single"
                    });

            var contentBlockMapper = new global::BusinessLayer.ContentBlock.Mappers.ContentBlockMapper(priceSeriesGridMapper);

            // Act
            var result = contentBlockMapper.Map(contentBlock);

            // Assert
            result.PriceSeriesGrids.Should().NotBeNull();
            result.PriceSeriesGrids.Should().HaveCount(2);

            result.PriceSeriesGrids?[0].PriceSeriesIds.Should().HaveCount(3);
            result.PriceSeriesGrids?[0].SeriesItemTypeCode.Should().Be("pi-single-ref");
            result.PriceSeriesGrids?[0].Id.Should().Be("PriceSeriesGridId1");
            result.PriceSeriesGrids?[0].Title.Should().Be("Test Grid Title 1");

            result.PriceSeriesGrids?[1].PriceSeriesIds.Should().HaveCount(2);
            result.PriceSeriesGrids?[1].SeriesItemTypeCode.Should().Be("pi-single");
            result.PriceSeriesGrids?[1].Id.Should().Be("PriceSeriesGridId2");
            result.PriceSeriesGrids?[1].Title.Should().Be("Test Grid Title 2");
        }
    }
}
