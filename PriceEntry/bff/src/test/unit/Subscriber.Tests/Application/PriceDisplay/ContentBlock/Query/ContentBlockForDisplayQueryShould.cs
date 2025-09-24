namespace Subscriber.Tests.Application.PriceDisplay.ContentBlock.Query
{
    using System.Threading.Tasks;

    using BusinessLayer.PriceDisplay.ContentBlock.DTOs.Input;
    using BusinessLayer.PriceEntry.ValueObjects;

    using Snapshooter;
    using Snapshooter.Xunit;

    using Subscriber.Tests.Infrastructure;

    using Test.Infrastructure.Authoring;
    using Test.Infrastructure.Authoring.GraphQLClients.PriceDisplay;
    using Test.Infrastructure.Extensions;
    using Test.Infrastructure.Subscriber;
    using Test.Infrastructure.Subscriber.GraphQLClients.PriceDisplay;
    using Test.Infrastructure.TestData;

    using Xunit;

    [Collection(WebApplicationCollection.Name)]
    public class ContentBlockForDisplayQueryShould : WebApplicationTestBase
    {
        private readonly ContentBlockForDisplaySubscriberGraphQLClient subscriberContentBlockClient;
        private readonly ContentBlockForDisplayAuthoringGraphQLClient authoringContentBlockClient;

        public ContentBlockForDisplayQueryShould(
            SubscriberBffApplicationFactory subscriberFactory,
            AuthoringBffApplicationFactory authoringFactory)
            : base(subscriberFactory, authoringFactory)
        {
            subscriberContentBlockClient = new ContentBlockForDisplaySubscriberGraphQLClient(SubscriberGraphQLClient);
            authoringContentBlockClient = new ContentBlockForDisplayAuthoringGraphQLClient(AuthoringGraphQLClient);
        }

        public static TheoryData<string, ContentBlockForDisplayInput> DifferentInstancesOfContentBlockInput =>
            new()
            {
                {
                    "With_PIRange", new ContentBlockForDisplayInput
                    {
                        ContentBlockId = "contentBlockId",
                        Title = "title",
                        Columns = null,
                        Rows =
                        [
                            new RowForDisplayInput
                            {
                                PriceSeriesId = TestSeries.Melamine_Asia_NE_Spot,
                                DisplayOrder = 0,
                                SeriesItemTypeCode = SeriesItemTypeCode.Range
                            },
                            new RowForDisplayInput
                            {
                                PriceSeriesId = TestSeries.Melamine_China_Spot_Cif_2_6_Weeks,
                                DisplayOrder = 1,
                                SeriesItemTypeCode = SeriesItemTypeCode.Range
                            }
                        ]
                    }
                },
                {
                    "With_PIRange_PISingle", new ContentBlockForDisplayInput
                    {
                        ContentBlockId = "contentBlockId",
                        Title = "title",
                        Columns = null,
                        Rows =
                        [
                            new RowForDisplayInput
                            {
                                PriceSeriesId = TestSeries.Melamine_Asia_NE_Spot,
                                DisplayOrder = 0,
                                SeriesItemTypeCode = SeriesItemTypeCode.SingleValue
                            },
                            new RowForDisplayInput
                            {
                                PriceSeriesId = TestSeries.Melamine_China_Spot_Cif_2_6_Weeks,
                                DisplayOrder = 1,
                                SeriesItemTypeCode = SeriesItemTypeCode.Range
                            }
                        ]
                    }
                },
                {
                    "With_SingleRef",
                    new ContentBlockForDisplayInput
                    {
                        ContentBlockId = "contentBlockId",
                        Title = "title",
                        Columns = null,
                        Rows =
                        [
                            new RowForDisplayInput
                            {
                                PriceSeriesId = TestSeries.Melamine_Asia_NE_Spot,
                                DisplayOrder = 0,
                                SeriesItemTypeCode = SeriesItemTypeCode.SingleValueWithReference
                            }
                        ]
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

            var authoringSaveResponse = await authoringContentBlockClient.SaveContentBlockForDisplay(contentBlock).GetRawResponse();
            authoringSaveResponse.MatchSnapshot(SnapshotNameExtension.Create("AuthoringSaveResponse_ContentBlockV1"));

            contentBlock.Title = "new title";
            _ = await authoringContentBlockClient.SaveContentBlockForDisplay(contentBlock);

            var subscriberResponse = await subscriberContentBlockClient.GetContentBlockForDisplay(contentBlock.ContentBlockId, 1).GetRawResponse();

            subscriberResponse.MatchSnapshot(SnapshotNameExtension.Create("SubscriberGetResponse"));
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
                SelectedFilters = null
            };

            _ = await authoringContentBlockClient.SaveContentBlockForDisplay(contentBlock);

            contentBlock.Title = "new title 1";
            _ = await authoringContentBlockClient.SaveContentBlockForDisplay(contentBlock);

            contentBlock.Title = "new title 2";
            var authoringSaveResponse = await authoringContentBlockClient.SaveContentBlockForDisplay(contentBlock).GetRawResponse();

            authoringSaveResponse.MatchSnapshot(SnapshotNameExtension.Create("AuthoringSaveResponse_ContentBlock_Latest"));

            var subscriberResponse = await subscriberContentBlockClient.GetContentBlockForDisplay(contentBlock.ContentBlockId).GetRawResponse();

            subscriberResponse.MatchSnapshot(SnapshotNameExtension.Create("SubscriberGetResponse"));
        }

        [Fact]
        public async Task Return_Nothing_When_Content_Block_Does_Not_Exists()
        {
            var response = await subscriberContentBlockClient.GetContentBlockForDisplay("some-non-existing-id").GetRawResponse();

            response.MatchSnapshot();
        }

        [Theory]
        [MemberData(nameof(DifferentInstancesOfContentBlockInput))]
        public async Task Return_Full_Grid_Configuration_After_Price_Series_Are_Saved(
            string scenario,
            ContentBlockForDisplayInput contentBlockInput)
        {
            _ = await authoringContentBlockClient.SaveContentBlockForDisplay(contentBlockInput);

            var response = await subscriberContentBlockClient
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

            _ = await authoringContentBlockClient.SaveContentBlockForDisplay(contentBlockSaveRequest);

            var response = await subscriberContentBlockClient
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
                Rows = [new() { PriceSeriesId = TestSeries.Melamine_Asia_NE_Spot, DisplayOrder = 0, SeriesItemTypeCode = SeriesItemTypeCode.Range },
                        new() { PriceSeriesId = TestSeries.Melamine_China_Spot_Cif_2_6_Weeks, DisplayOrder = 1, SeriesItemTypeCode = SeriesItemTypeCode.Range }],
            };

            _ = await authoringContentBlockClient.SaveContentBlockForDisplay(contentBlockSaveRequest);

            var response = await subscriberContentBlockClient
                              .GetContentBlockWithGridConfigurationOnly(contentBlockSaveRequest.ContentBlockId, 1)
                              .GetRawResponse();

            response.MatchSnapshot();
        }
    }
}
