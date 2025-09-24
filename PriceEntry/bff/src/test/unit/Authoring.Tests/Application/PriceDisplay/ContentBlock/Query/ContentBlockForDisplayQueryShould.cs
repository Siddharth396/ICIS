namespace Authoring.Tests.Application.PriceDisplay.ContentBlock.Query
{
    using System.Threading.Tasks;

    using Authoring.Tests.Functional.Tests.Common.Infrastructure;

    using global::BusinessLayer.PriceDisplay.ContentBlock.DTOs.Input;
    using global::BusinessLayer.PriceEntry.ValueObjects;

    using Snapshooter;
    using Snapshooter.Xunit;

    using Test.Infrastructure.Authoring;
    using Test.Infrastructure.Authoring.GraphQLClients.PriceDisplay;
    using Test.Infrastructure.Extensions;
    using Test.Infrastructure.TestData;

    using Xunit;

    [Collection(WebApplicationCollection.Name)]
    public class ContentBlockForDisplayQueryShould : WebApplicationTestBase
    {
        private const string Single = SeriesItemTypeCode.SingleValue;

        private const string Range = SeriesItemTypeCode.Range;

        private const string SingleWithRef = SeriesItemTypeCode.SingleValueWithReference;

        private const string CharterRateSingle = SeriesItemTypeCode.CharterRateSingleValue;

        private readonly ContentBlockForDisplayAuthoringGraphQLClient contentBlockClient;

        public ContentBlockForDisplayQueryShould(AuthoringBffApplicationFactory factory)
            : base(factory)
        {
            contentBlockClient = new ContentBlockForDisplayAuthoringGraphQLClient(GraphQLClient);
        }

        public static TheoryData<string, ContentBlockForDisplayInput> DifferentInstancesOfContentBlockInput =>
          new()
          {
                {
                    "With_PIRange",
                    new ContentBlockForDisplayInput
                    {
                        ContentBlockId = "contentBlockId",
                        Title = "title",
                        Columns = null,
                        Rows = [new() { PriceSeriesId = TestSeries.Melamine_Asia_NE_Spot, DisplayOrder = 0, SeriesItemTypeCode = Range },
                                new() { PriceSeriesId = TestSeries.Melamine_China_Spot_Cif_2_6_Weeks, DisplayOrder = 1, SeriesItemTypeCode = Range }]
                    }
                },
                {
                    "With_PIRange_PISingle",
                    new ContentBlockForDisplayInput
                    {
                        ContentBlockId = "contentBlockId",
                        Title = "title",
                        Columns = null,
                        Rows = [new() { PriceSeriesId = TestSeries.Melamine_Asia_NE_Spot, DisplayOrder = 0, SeriesItemTypeCode = Single },
                                new() { PriceSeriesId = TestSeries.Melamine_China_Spot_Cif_2_6_Weeks, DisplayOrder = 1, SeriesItemTypeCode = Range }]
                    }
                },
                {
                    "With_SingleRef",
                    new ContentBlockForDisplayInput
                    {
                        ContentBlockId = "contentBlockId",
                        Title = "title",
                        Columns = null,
                        Rows = [new() { PriceSeriesId = TestSeries.Melamine_Asia_NE_Spot, DisplayOrder = 0, SeriesItemTypeCode = SingleWithRef }]
                    }
                },
                {
                    "With_CRI",
                    new ContentBlockForDisplayInput
                    {
                        ContentBlockId = "contentBlockId",
                        Title = "title",
                        Columns = null,
                        Rows = [new() { PriceSeriesId = TestSeries.CharterRates_Pacific_Prompt_Two_Stroke, DisplayOrder = 0, SeriesItemTypeCode = CharterRateSingle }]
                    }
                },
                {
                    "With_CRI_Range",
                    new ContentBlockForDisplayInput
                    {
                        ContentBlockId = "contentBlockId",
                        Title = "title",
                        Columns = null,
                        Rows = [new() { PriceSeriesId = TestSeries.CharterRates_Pacific_Prompt_Two_Stroke, DisplayOrder = 0, SeriesItemTypeCode = CharterRateSingle },
                                new() { PriceSeriesId = TestSeries.Melamine_China_Spot_Cif_2_6_Weeks, DisplayOrder = 1, SeriesItemTypeCode = Range }]
                    }
                },
                {
                    "With_CRI_REF_Range",
                    new ContentBlockForDisplayInput
                    {
                        ContentBlockId = "contentBlockId",
                        Title = "title",
                        Columns = null,
                        Rows = [new() { PriceSeriesId = TestSeries.CharterRates_Pacific_Prompt_Two_Stroke, DisplayOrder = 0, SeriesItemTypeCode = CharterRateSingle },
                                new() { PriceSeriesId = TestSeries.Melamine_Asia_NE_Spot, DisplayOrder = 0, SeriesItemTypeCode = SingleWithRef }]
                    }
                }
          };

        [Fact]
        public async Task Return_A_Specific_Version_Of_Content_Block()
        {
            var contentBlock = new ContentBlockForDisplayInput
            {
                ContentBlockId = "contentBlockId",
                Title = "title",
                Columns = null,
                Rows = null,
                SelectedFilters = null,
            };

            await contentBlockClient.SaveContentBlockForDisplay(contentBlock);

            contentBlock.Title = "new title";

            await contentBlockClient.SaveContentBlockForDisplay(contentBlock);

            var response = await contentBlockClient.GetContentBlockForDisplay(contentBlock.ContentBlockId, 1).GetRawResponse();

            response.MatchSnapshot();
        }

        [Fact]
        public async Task Return_A_Latest_Version_Of_Content_Block()
        {
            var contentBlock = new ContentBlockForDisplayInput
            {
                ContentBlockId = "contentBlockId",
                Title = "title",
                Columns = null,
                Rows = null,
                SelectedFilters = null,
            };

            await contentBlockClient.SaveContentBlockForDisplay(contentBlock);

            contentBlock.Title = "new title";

            await contentBlockClient.SaveContentBlockForDisplay(contentBlock);

            var response = await contentBlockClient.GetContentBlockForDisplay(contentBlock.ContentBlockId).GetRawResponse();

            response.MatchSnapshot();
        }

        [Fact]
        public async Task Return_Nothing_When_Content_Block_Does_Not_Exists()
        {
            var response = await contentBlockClient.GetContentBlockForDisplay("some-non-existing-id").GetRawResponse();

            response.MatchSnapshot();
        }

        [Theory]
        [MemberData(nameof(DifferentInstancesOfContentBlockInput))]
        public async Task Return_Full_Grid_Configuration_After_Price_Series_Are_Saved(
            string scenario,
            ContentBlockForDisplayInput contentBlockInput)
        {
            _ = await contentBlockClient.SaveContentBlockForDisplay(contentBlockInput);

            var response = await contentBlockClient
                              .GetContentBlockWithGridConfigurationOnly(contentBlockInput.ContentBlockId, 1)
                              .GetRawResponse();

            response.MatchSnapshot(SnapshotNameExtension.Create(scenario));
        }

        [Fact]
        public async Task Return_Null_Grid_Configuration_When_Price_Series_Are_Not_Available()
        {
            var contentBlockSaveRequest = new ContentBlockForDisplayInput
            {
                ContentBlockId = "contentBlockId1",
                Title = "title",
                Columns = null,
            };

            _ = await contentBlockClient.SaveContentBlockForDisplay(contentBlockSaveRequest);

            var response = await contentBlockClient
                              .GetContentBlockWithGridConfigurationOnly(contentBlockSaveRequest.ContentBlockId)
                              .GetRawResponse();

            response.MatchSnapshot();
        }

        [Fact]
        public async Task Return_Ordered_Grid_Configuration_After_Reordered_Columns_Are_Saved()
        {
            var contentBlockSaveRequest = new ContentBlockForDisplayInput
            {
                ContentBlockId = "contentBlockId",
                Title = "title",
                Columns = [new() { DisplayOrder = 0, Field = "priceSeriesName", Hidden = false }, new() { DisplayOrder = 1, Field = "priceDelta", Hidden = false }],
                Rows = [new() { PriceSeriesId = TestSeries.Melamine_Asia_NE_Spot, DisplayOrder = 0, SeriesItemTypeCode = Range },
                        new() { PriceSeriesId = TestSeries.Melamine_China_Spot_Cif_2_6_Weeks, DisplayOrder = 1, SeriesItemTypeCode = Range }],
            };

            _ = await contentBlockClient.SaveContentBlockForDisplay(contentBlockSaveRequest);

            var response = await contentBlockClient
                              .GetContentBlockWithGridConfigurationOnly(contentBlockSaveRequest.ContentBlockId, 1)
                              .GetRawResponse();

            response.MatchSnapshot();
        }
    }
}
