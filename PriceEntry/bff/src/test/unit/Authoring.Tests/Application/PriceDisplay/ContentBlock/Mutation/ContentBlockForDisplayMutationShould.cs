namespace Authoring.Tests.Application.PriceDisplay.ContentBlock.Mutation
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Authoring.Tests.Functional.Tests.Common.Infrastructure;

    using global::BusinessLayer.PriceDisplay.ContentBlock.DTOs.Input;

    using Snapshooter;
    using Snapshooter.Xunit;

    using Test.Infrastructure.Authoring;
    using Test.Infrastructure.Authoring.GraphQLClients.PriceDisplay;
    using Test.Infrastructure.Extensions;

    using Xunit;

    [Collection(WebApplicationCollection.Name)]
    public class ContentBlockForDisplayMutationShould : WebApplicationTestBase
    {
        private readonly ContentBlockForDisplayAuthoringGraphQLClient client;

        public ContentBlockForDisplayMutationShould(AuthoringBffApplicationFactory factory)
            : base(factory)
        {
            client = new ContentBlockForDisplayAuthoringGraphQLClient(GraphQLClient);
        }

        public static TheoryData<string, ContentBlockForDisplayInput> DifferentInstancesOfContentBlockInputForDisplay =>
            new()
            {
                {
                    "With_null_values",
                    new()
                    {
                        ContentBlockId = "contentBlockId", Title = null, Columns = null, Rows = null, SelectedFilters = null
                    }
                },
                {
                    "With_empty_values",
                    new()
                    {
                        ContentBlockId = "contentBlockId", Title = string.Empty, Columns = new List<ColumnForDisplayInput>(), Rows = new List<RowForDisplayInput>(), SelectedFilters = null
                    }
                },
                {
                    "With_title_only",
                    new()
                    {
                        ContentBlockId = "contentBlockId", Title = "title", Columns = null, Rows = null, SelectedFilters = null
                    }
                },
                {
                    "With_price_series_only",
                    new()
                    {
                        ContentBlockId = "contentBlockId",
                        Title = null,
                        Columns =
                        [
                            new() { Field = "field", DisplayOrder = 1, Hidden = false }
                        ],
                        Rows =
                        [
                            new() { PriceSeriesId = "priceSeriesId", DisplayOrder = 1, SeriesItemTypeCode = "seriesItemTypeCode" }
                        ],
                        SelectedFilters = new SelectedFiltersForDisplayInput()
                        {
                            IsInactiveIncluded = true,
                            SelectedCommodities = new List<string>() { "commodity1" },
                            SelectedAssessedFrequencies = null,
                            SelectedPriceCategories = null,
                            SelectedRegions = null,
                            SelectedTransactionTypes = null,
                        }
                    }
                },
                {
                    "With_different_values",
                    new()
                    {
                        ContentBlockId = "contentBlockId",
                        Title = "title",
                        Columns =
                        [
                            new() { Field = "field", DisplayOrder = 1, Hidden = false }
                        ],
                        Rows =
                        [
                            new() { PriceSeriesId = "priceSeriesId", DisplayOrder = 1, SeriesItemTypeCode = "seriesItemTypeCode" }
                        ],
                        SelectedFilters = new SelectedFiltersForDisplayInput()
                        {
                            IsInactiveIncluded = false,
                            SelectedCommodities = new List<string>() { "commodity1" },
                            SelectedAssessedFrequencies = null,
                            SelectedPriceCategories = null,
                            SelectedRegions = null,
                            SelectedTransactionTypes = null,
                        }
                    }
                },
                {
                    "With_multiple_columns",
                    new()
                    {
                        ContentBlockId = "contentBlockId",
                        Title = "title",
                        Columns =
                        [
                            new() { Field = "field1", DisplayOrder = 1, Hidden = false },
                            new() { Field = "field2", DisplayOrder = 2, Hidden = false },
                            new() { Field = "field3", DisplayOrder = 3, Hidden = false }
                        ],
                        Rows =
                        [
                            new() { PriceSeriesId = "priceSeriesId", DisplayOrder = 1, SeriesItemTypeCode = "seriesItemTypeCode" }
                        ],
                        SelectedFilters = new SelectedFiltersForDisplayInput()
                        {
                            IsInactiveIncluded = true,
                            SelectedCommodities = new List<string>() { "commodity1" },
                            SelectedAssessedFrequencies = null,
                            SelectedPriceCategories = null,
                            SelectedRegions = null,
                            SelectedTransactionTypes = null,
                        }
                    }
                },
                {
                    "With_multiple_rows",
                    new()
                    {
                        ContentBlockId = "contentBlockId",
                        Title = "title",
                        Columns =
                        [
                            new() { Field = "field", DisplayOrder = 1, Hidden = false }
                        ],
                        Rows =
                        [
                            new() { PriceSeriesId = "priceSeriesId1", DisplayOrder = 1, SeriesItemTypeCode = "seriesItemTypeCode1" },
                            new() { PriceSeriesId = "priceSeriesId2", DisplayOrder = 3, SeriesItemTypeCode = "seriesItemTypeCode2" },
                            new() { PriceSeriesId = "priceSeriesId3", DisplayOrder = 2, SeriesItemTypeCode = "seriesItemTypeCode3" }
                        ],
                        SelectedFilters = new SelectedFiltersForDisplayInput()
                        {
                            IsInactiveIncluded = true,
                            SelectedCommodities = new List<string>() { "commodity1", "commodity2" },
                            SelectedAssessedFrequencies = null,
                            SelectedPriceCategories = null,
                            SelectedRegions = new List<string>() { "Europe", "Asia" },
                            SelectedTransactionTypes = null,
                        }
                    }
                },
                {
                    "With_multiple_rows_and_columns",
                    new()
                    {
                        ContentBlockId = "contentBlockId",
                        Title = "title",
                        Columns =
                        [
                            new() { Field = "field1", DisplayOrder = 1, Hidden = false },
                            new() { Field = "field2", DisplayOrder = 2, Hidden = false },
                            new() { Field = "field3", DisplayOrder = 3, Hidden = false }
                        ],
                        Rows =
                        [
                            new() { PriceSeriesId = "priceSeriesId1", DisplayOrder = 1, SeriesItemTypeCode = "seriesItemTypeCode1" },
                            new() { PriceSeriesId = "priceSeriesId2", DisplayOrder = 3, SeriesItemTypeCode = "seriesItemTypeCode2" },
                            new() { PriceSeriesId = "priceSeriesId3", DisplayOrder = 2, SeriesItemTypeCode = "seriesItemTypeCode3" }
                        ],
                        SelectedFilters = new SelectedFiltersForDisplayInput()
                        {
                            IsInactiveIncluded = true,
                            SelectedCommodities = new List<string>() { "commodity1", "commodity2" },
                            SelectedAssessedFrequencies = new List<string>() { "weekly", "monthly" },
                            SelectedPriceCategories = new List<string>() { "priceCategory1" },
                            SelectedRegions = new List<string>() { "Europe", "Asia" },
                            SelectedTransactionTypes = new List<string>() { "transaction1" },
                        }
                    }
                }
            };

        [Fact]
        public async Task Increase_The_Version_On_Consecutive_Changes()
        {
            // Arrange
            var contentBlockInput = new ContentBlockForDisplayInput
            {
                ContentBlockId = "contentBlockId",
                Title = "title",
                Columns =
                [
                    new() { Field = "field", DisplayOrder = 1, Hidden = false }
                ],
                Rows =
                [
                    new() { PriceSeriesId = "priceSeriesId", DisplayOrder = 1, SeriesItemTypeCode = "seriesItemTypeCode" }
                ],
                SelectedFilters = new SelectedFiltersForDisplayInput()
                {
                    IsInactiveIncluded = true,
                    SelectedCommodities = new List<string>() { "commodity1" },
                    SelectedAssessedFrequencies = null,
                    SelectedPriceCategories = null,
                    SelectedRegions = null,
                    SelectedTransactionTypes = null,
                }
            };

            _ = await client.SaveContentBlockForDisplay(contentBlockInput);

            contentBlockInput.Title = "new title";

            // Act
            var saveResult = await client.SaveContentBlockForDisplay(contentBlockInput).GetRawResponse();
            var getResult = await client.GetContentBlockForDisplay(contentBlockInput.ContentBlockId).GetRawResponse();

            // Assert
            saveResult.MatchSnapshot(SnapshotNameExtension.Create("SaveContentBlockResult"));
            getResult.MatchSnapshot(SnapshotNameExtension.Create("GetContentBlockResult"));
        }

        [Theory]
        [MemberData(nameof(DifferentInstancesOfContentBlockInputForDisplay))]
        public async Task Save_Content_Block_For_Display_With_Different_Values_For_Input(
            string scenario,
            ContentBlockForDisplayInput contentBlockInput)
        {
            // Act
            await client.SaveContentBlockForDisplay(contentBlockInput);

            // Assert
            var result = await client.GetContentBlockForDisplay(contentBlockInput.ContentBlockId).GetRawResponse();

            result.MatchSnapshot(SnapshotNameExtension.Create(scenario));
        }

        [Fact]
        public async Task Save_Content_Block_For_Display_With_Version_One_For_The_First_Time()
        {
            var contentBlockInput = new ContentBlockForDisplayInput
            {
                ContentBlockId = "contentBlockId",
                Title = "title",
                Columns =
                [
                    new() { Field = "field", DisplayOrder = 1, Hidden = false }
                ],
                Rows =
                [
                    new() { PriceSeriesId = "priceSeriesId", DisplayOrder = 1, SeriesItemTypeCode = "seriesItemTypeCode" }
                ],
                SelectedFilters = new SelectedFiltersForDisplayInput()
                {
                    IsInactiveIncluded = true,
                    SelectedCommodities = new List<string>() { "commodity1" },
                    SelectedAssessedFrequencies = null,
                    SelectedPriceCategories = null,
                    SelectedRegions = null,
                    SelectedTransactionTypes = null,
                }
            };

            // Act
            var saveResult = await client.SaveContentBlockForDisplay(contentBlockInput).GetRawResponse();
            var getResult = await client.GetContentBlockForDisplay(contentBlockInput.ContentBlockId).GetRawResponse();

            // Assert
            saveResult.MatchSnapshot(SnapshotNameExtension.Create("SaveContentBlockResult"));
            getResult.MatchSnapshot(SnapshotNameExtension.Create("GetContentBlockResult"));
        }
    }
}
