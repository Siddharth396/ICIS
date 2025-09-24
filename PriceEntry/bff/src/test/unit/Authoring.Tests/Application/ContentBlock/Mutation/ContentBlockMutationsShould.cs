namespace Authoring.Tests.Application.ContentBlock.Mutation
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Authoring.Tests.Functional.Tests.Common.Infrastructure;

    using global::BusinessLayer.ContentBlock.DTOs.Input;
    using global::BusinessLayer.ContentPackageGroup.Repositories;
    using global::BusinessLayer.ContentPackageGroup.Repositories.Models;

    using Snapshooter;
    using Snapshooter.Xunit;

    using Test.Infrastructure.Authoring;
    using Test.Infrastructure.Authoring.GraphQLClients.PriceEntry;
    using Test.Infrastructure.Extensions;
    using Test.Infrastructure.Mongo.Repositories;
    using Test.Infrastructure.TestData;

    using Xunit;

    [Collection(WebApplicationCollection.Name)]
    public class ContentBlockMutationsShould : WebApplicationTestBase
    {
        private readonly ContentBlockGraphQLClient contentBlockClient;
        private readonly GenericRepository genericRepository;

        public ContentBlockMutationsShould(AuthoringBffApplicationFactory factory)
            : base(factory)
        {
            contentBlockClient = new ContentBlockGraphQLClient(GraphQLClient);
            genericRepository = GetService<GenericRepository>();
        }

        public static TheoryData<string, ContentBlockInput> DifferentInstancesOfContentBlockInput =>
            new()
            {
                {
                    "With_null_values",
                    new ContentBlockInput { ContentBlockId = "contentBlockId", Title = null, PriceSeriesGrids = null }
                },
                {
                    "With_empty_values",
                    new ContentBlockInput
                    {
                        ContentBlockId = "contentBlockId",
                        Title = string.Empty,
                        PriceSeriesGrids = []
                    }
                },
                {
                    "With_different_values",
                    new ContentBlockInput
                    {
                        ContentBlockId = "contentBlockId",
                        Title = "title",
                        PriceSeriesGrids =
                        [
                            new PriceSeriesGrid { Title = "PriceSeriesGridOne", PriceSeriesIds = null }
                        ]
                    }
                },
                {
                    "With_title_only",
                    new ContentBlockInput { ContentBlockId = "contentBlockId", Title = "title", PriceSeriesGrids = null }
                },
                {
                    "With_price_series_only",
                    new ContentBlockInput
                    {
                        ContentBlockId = "contentBlockId",
                        Title = null,
                        PriceSeriesGrids =
                        [
                            new PriceSeriesGrid
                            {
                                Title = null,
                                PriceSeriesIds =
                                [
                                    TestSeries.LNG_China_HM1, TestSeries.LNG_China_HM2
                                ]
                            }
                        ]
                    }
                },
                {
                    "With_multiple_price_series_grids",
                    new ContentBlockInput
                    {
                        ContentBlockId = "contentBlockId",
                        Title = null,
                        PriceSeriesGrids =
                        [
                            new PriceSeriesGrid
                            {
                                Title = "PriceSeriesGridOne",
                                PriceSeriesIds =
                                [
                                    TestSeries.LNG_China_HM1,
                                    TestSeries.LNG_China_HM2,
                                    TestSeries.LNG_China_HM3,
                                    TestSeries.LNG_China_HM4
                                ]
                            },

                            new PriceSeriesGrid
                            {
                                Title = null,
                                PriceSeriesIds =
                                [
                                    TestSeries.LNG_China_MM1, TestSeries.LNG_China_MM2
                                ]
                            }
                        ]
                    }
                },
                {
                    "With_different_schedules",
                    new ContentBlockInput
                    {
                        ContentBlockId = "contentBlockId",
                        Title = null,
                        PriceSeriesGrids =
                        [
                            new PriceSeriesGrid
                            {
                                Title = "PriceSeriesGridOne",
                                PriceSeriesIds =
                                [
                                    TestSeries.LNG_China_HM1,
                                    TestSeries.LNG_China_HM2,
                                    TestSeries.LNG_China_HM3,
                                    TestSeries.LNG_China_HM4
                                ]
                            },

                            new PriceSeriesGrid
                            {
                                Title = null,
                                PriceSeriesIds =
                                [
                                    TestSeries.LNG_Truck_Guangdong_Publication_Schedules
                                ]
                            }
                        ]
                    }
                },
                {
                    "With_duplicate_price_series",
                    new ContentBlockInput
                    {
                        ContentBlockId = "contentBlockId",
                        Title = null,
                        PriceSeriesGrids =
                        [
                            new PriceSeriesGrid
                            {
                                Title = "PriceSeriesGridOne",
                                PriceSeriesIds =
                                [
                                    TestSeries.LNG_China_HM1,
                                    TestSeries.LNG_China_HM2,
                                    TestSeries.LNG_China_HM3,
                                    TestSeries.LNG_China_HM4
                                ]
                            },

                            new PriceSeriesGrid
                            {
                                Title = null, PriceSeriesIds = [TestSeries.LNG_China_HM2]
                            }
                        ]
                    }
                },
                {
                    "With_multiple_series_item_type_code",
                    new ContentBlockInput
                    {
                        ContentBlockId = "contentBlockId",
                        Title = null,
                        PriceSeriesGrids =
                        [
                            new PriceSeriesGrid
                            {
                                Title = "PriceSeriesGridOne",
                                PriceSeriesIds =
                                [
                                    TestSeries.LNG_China_HM1, TestSeries.LNG_China_MM1
                                ]
                            }

                        ]
                    }
                },
                {
                    "With_multiple_commodities_in_same_grid",
                    new ContentBlockInput
                    {
                        ContentBlockId = "contentBlockId",
                        Title = null,
                        PriceSeriesGrids =
                        [
                            new PriceSeriesGrid
                            {
                                Title = "PriceSeriesGridOne",
                                PriceSeriesIds =
                                [
                                    TestSeries.Styrene_Dagu_Spot_China_N, TestSeries.Melamine_Monthly_Contract_Europe
                                ]
                            }

                        ]
                    }
                },
                {
                    "With_multiple_commodities_in_different_grid",
                    new ContentBlockInput
                    {
                        ContentBlockId = "contentBlockId",
                        Title = null,
                        PriceSeriesGrids =
                        [
                            new PriceSeriesGrid
                            {
                                Title = "PriceSeriesGridOne",
                                PriceSeriesIds = [TestSeries.Styrene_Dagu_Spot_China_N]
                            },

                            new PriceSeriesGrid
                            {
                                Title = "PriceSeriesGridOne",
                                PriceSeriesIds = [TestSeries.Melamine_Monthly_Contract_Europe]
                            }

                        ]
                    }
                },
                {
                    "With_same_schedule",
                    new ContentBlockInput
                    {
                        ContentBlockId = "contentBlockId",
                        Title = null,
                        PriceSeriesGrids =
                        [
                            new PriceSeriesGrid
                            {
                                Title = "test 1",
                                PriceSeriesIds =
                                [
                                    TestSeries.Melamine_China_Spot_Cif_2_6_Weeks,
                                    TestSeries.Melamine_Asia_SE_Spot
                                ]
                            },
                            new PriceSeriesGrid
                            {
                                Title = "test 2",
                                PriceSeriesIds =
                                [
                                    TestSeries.Melamine_China_Spot_FOB,
                                ]
                            }
                        ]
                    }
                }
            };

        [Fact]
        public async Task Increase_The_Version_On_Consecutive_Changes()
        {
            // Arrange
            var contentBlockInput = new ContentBlockInput
            {
                ContentBlockId = "contentBlockId",
                Title = "title",
                PriceSeriesGrids = new List<PriceSeriesGrid>()
            };

            _ = await contentBlockClient.SaveContentBlock(contentBlockInput);

            // act
            var result = await contentBlockClient.SaveContentBlock(contentBlockInput).GetRawResponse();

            // assert
            result.MatchSnapshot();
        }

        [Theory]
        [MemberData(nameof(DifferentInstancesOfContentBlockInput))]
        public async Task Save_Content_Block_With_Different_Values_For_Input(
            string scenario,
            ContentBlockInput contentBlockInput)
        {
            // act
            var result = await contentBlockClient.SaveContentBlock(contentBlockInput).GetRawResponse();

            result.MatchSnapshot(SnapshotNameExtension.Create(scenario));
        }

        [Fact]
        public async Task Save_Content_Block_With_Version_One_For_The_First_Time()
        {
            var contentBlockInput = new ContentBlockInput
            {
                ContentBlockId = "contentBlockId",
                Title = "title",
                PriceSeriesGrids = []
            };

            var result = await contentBlockClient.SaveContentBlock(contentBlockInput).GetRawResponse();

            result.MatchSnapshot();
        }

        [Fact]
        public async Task Validate_Input_Price_Series()
        {
            // Arrange
            var contentBlockInput = new ContentBlockInput
            {
                ContentBlockId = "contentBlockId",
                Title = "title",
                PriceSeriesGrids =
                [
                    new PriceSeriesGrid { Title = "PriceSeriesGridOne", PriceSeriesIds = [TestSeries.LNG_China_HM1] },
                    new PriceSeriesGrid { Title = null, PriceSeriesIds = [TestSeries.LNG_China_HM2] }
                ]
            };

            // act
            var result = await contentBlockClient.SaveContentBlock(contentBlockInput).GetRawResponse();

            // assert
            result.MatchSnapshot();
        }

        [Fact]
        public async Task Validate_Content_Package_Group_When_Content_Block_Update_Title_Only()
        {
            // Arrange
            List<string> priceSeriesIds = [TestSeries.LNG_China_HM1, TestSeries.LNG_China_HM2];
            var contentBlockInput = new ContentBlockInput
            {
                ContentBlockId = "contentBlockId",
                Title = "title",
                PriceSeriesGrids =
                [
                    new PriceSeriesGrid { Title = "PriceSeriesGridOne", PriceSeriesIds = priceSeriesIds }
                ]
            };
            await contentBlockClient.SaveContentBlock(contentBlockInput);
            contentBlockInput.Title = "updatedTitle";
            await contentBlockClient.SaveContentBlock(contentBlockInput);

            // act
            var result = (await genericRepository.GetDocument<ContentPackageGroup>(
                                          ContentPackageGroupRepository.CollectionName,
                                          x => x.PriceSeriesIds.Count == priceSeriesIds.Count &&
                                               x.PriceSeriesIds.All(priceSeriesId => priceSeriesIds.Contains(priceSeriesId)))).First();

            // assert
            result.MatchSnapshotIgnoringId();
        }

        [Fact]
        public async Task Validate_Content_Package_Group_When_Content_Block_Without_Price_Series()
        {
            // Arrange
            List<string> priceSeriesIds = [];
            var contentBlockInput = new ContentBlockInput
            {
                ContentBlockId = "contentBlockId",
                Title = "title",
                PriceSeriesGrids =
                [
                    new PriceSeriesGrid { Title = "PriceSeriesGridOne", PriceSeriesIds = priceSeriesIds }
                ]
            };
            await contentBlockClient.SaveContentBlock(contentBlockInput);

            // act
            var result = (await genericRepository.GetDocument<ContentPackageGroup>(
                                          ContentPackageGroupRepository.CollectionName,
                                          x => x.PriceSeriesIds.Count == priceSeriesIds.Count &&
                                               x.PriceSeriesIds.All(priceSeriesId => priceSeriesIds.Contains(priceSeriesId)))).FirstOrDefault();

            // assert
            Assert.Null(result);
        }

        [Fact]
        public async Task Validate_Content_Package_Group_When_Multiple_Content_Block_With_Different_Price_Series()
        {
            // Arrange
            List<string> priceSeriesIds1 = [TestSeries.LNG_China_HM1, TestSeries.LNG_China_HM2];
            List<string> priceSeriesIds2 = [TestSeries.LNG_India_HM1, TestSeries.LNG_India_HM2];
            var contentBlockInput1 = new ContentBlockInput
            {
                ContentBlockId = "contentBlockId1",
                Title = "title1",
                PriceSeriesGrids =
                [
                    new PriceSeriesGrid { Title = "PriceSeriesGridOne", PriceSeriesIds = priceSeriesIds1 }
                ]
            };
            var contentBlockInput2 = new ContentBlockInput
            {
                ContentBlockId = "contentBlockId2",
                Title = "title2",
                PriceSeriesGrids =
                [
                    new PriceSeriesGrid { Title = "PriceSeriesGridOne", PriceSeriesIds = priceSeriesIds2 }
                ]
            };
            await contentBlockClient.SaveContentBlock(contentBlockInput1);
            await contentBlockClient.SaveContentBlock(contentBlockInput2);

            // act
            var result1 = (await genericRepository.GetDocument<ContentPackageGroup>(
                                          ContentPackageGroupRepository.CollectionName,
                                          x => x.PriceSeriesIds.Count == priceSeriesIds1.Count &&
                                               x.PriceSeriesIds.All(priceSeriesId => priceSeriesIds1.Contains(priceSeriesId)))).First();

            var result2 = (await genericRepository.GetDocument<ContentPackageGroup>(
                                          ContentPackageGroupRepository.CollectionName,
                                          x => x.PriceSeriesIds.Count == priceSeriesIds2.Count &&
                                               x.PriceSeriesIds.All(priceSeriesId => priceSeriesIds2.Contains(priceSeriesId)))).First();

            // assert
            result1.MatchSnapshotIgnoringId(SnapshotNameExtension.Create(contentBlockInput1.ContentBlockId));
            result2.MatchSnapshotIgnoringId(SnapshotNameExtension.Create(contentBlockInput2.ContentBlockId));
        }

        [Fact]
        public async Task Validate_Content_Package_Group_When_Multiple_Content_Block_With_Same_Price_Series()
        {
            // Arrange
            List<string> priceSeriesIds = [TestSeries.LNG_India_HM1, TestSeries.LNG_India_HM2];
            var contentBlockInput1 = new ContentBlockInput
            {
                ContentBlockId = "contentBlockId1",
                Title = "title1",
                PriceSeriesGrids =
                [
                    new PriceSeriesGrid { Title = "PriceSeriesGridOne", PriceSeriesIds = priceSeriesIds }
                ]
            };
            var contentBlockInput2 = new ContentBlockInput
            {
                ContentBlockId = "contentBlockId2",
                Title = "title2",
                PriceSeriesGrids =
                [
                    new PriceSeriesGrid { Title = "PriceSeriesGridOne", PriceSeriesIds = priceSeriesIds }
                ]
            };
            await contentBlockClient.SaveContentBlock(contentBlockInput1);
            await contentBlockClient.SaveContentBlock(contentBlockInput2);

            // act
            var result = (await genericRepository.GetDocument<ContentPackageGroup>(
                                          ContentPackageGroupRepository.CollectionName,
                                          x => x.PriceSeriesIds.Count == priceSeriesIds.Count &&
                                               x.PriceSeriesIds.All(priceSeriesId => priceSeriesIds.Contains(priceSeriesId)))).First();

            // assert
            result.MatchSnapshotIgnoringId();
        }
    }
}