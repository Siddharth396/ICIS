namespace Authoring.Tests.Application.DataPackage.Mutation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Authoring.Tests.Application.ContentBlock.Query.ContentBlockWithPriceSeries;
    using Authoring.Tests.Functional.Tests.Common.Infrastructure;

    using FluentAssertions;

    using global::BusinessLayer.ContentPackageGroup.Repositories;
    using global::BusinessLayer.ContentPackageGroup.Repositories.Models;
    using global::BusinessLayer.DataPackage.Repositories;
    using global::BusinessLayer.DataPackage.Repositories.Models;
    using global::BusinessLayer.PriceEntry.DTOs.Input;
    using global::BusinessLayer.PriceEntry.Repositories;
    using global::BusinessLayer.PriceEntry.Repositories.Models;
    using global::BusinessLayer.PriceEntry.ValueObjects;
    using global::Infrastructure.Services.PeriodGenerator;

    using Snapshooter;
    using Snapshooter.Xunit;

    using Test.Infrastructure.Authoring;
    using Test.Infrastructure.Authoring.BusinessFlows.PriceEntryBusinessFlow;
    using Test.Infrastructure.Authoring.BusinessFlows.PriceEntryBusinessFlow.PriceSeriesItemBuilders;
    using Test.Infrastructure.Authoring.GraphQLClients.PriceEntry;
    using Test.Infrastructure.Extensions;
    using Test.Infrastructure.Mongo.Repositories;
    using Test.Infrastructure.Stubs;
    using Test.Infrastructure.TestData;

    using Xunit;

    [Collection(WebApplicationCollection.Name)]
    public class DataPackageMutationsShould : WebApplicationTestBase
    {
        private const string ContentBlockId = "contentBlockId";

        private static readonly string PriceSeriesId = TestSeries.Melamine_Asia_SE_Spot;

        private static readonly string[] SeriesIds = { TestSeries.LNG_China_HM1, TestSeries.LNG_China_HM2 };

        private static readonly DateTime Now = TestData.Now;

        private readonly ContentBlockGraphQLClient contentBlockClient;

        private readonly DataPackageWorkflowServiceStub dataPackageWorkflowServiceStub;

        private readonly GenericRepository genericRepository;

        public DataPackageMutationsShould(AuthoringBffApplicationFactory factory)
            : base(factory)
        {
            contentBlockClient = new ContentBlockGraphQLClient(GraphQLClient);
            var testClock = GetService<TestClock>();
            testClock.SetUtcNow(Now);

            dataPackageWorkflowServiceStub = GetService<DataPackageWorkflowServiceStub>();
            dataPackageWorkflowServiceStub.ClearRequests();
            genericRepository = GetService<GenericRepository>();
        }

        public static TheoryData<string, string, SeriesItemTypeCode, SeriesItem>
            ValidSeriesItemsForAdvancedCorrectionWorkflow =>
            new()
            {
                {
                    "PRICE_RANGE",
                    TestSeries.Melamine_Asia_SE_Spot,
                    SeriesItemTypeCode.RangeSeries,
                    new SeriesItem { PriceLow = 10, PriceHigh = 20 }
                },
                {
                    "SINGLE_PRICE",
                    TestSeries.Petchem_8603620,
                    SeriesItemTypeCode.SingleValueSeries,
                    new SeriesItem { Price = 10 }
                },
                {
                    "CHARTER_PRICE",
                    TestSeries.CharterRates_Pacific_Prompt_Two_Stroke,
                    SeriesItemTypeCode.CharterRateSingleValueSeries,
                    new SeriesItem { Price = 10, DataUsed = "Interpolation/extrapolation" }
                }
            };

        public static TheoryData<string, string, SeriesItemTypeCode, SeriesItem, SeriesItem>
            InvalidSeriesItemsForAdvancedCorrectionWorkflow =>
            new()
            {
                {
                    "PRICE_RANGE",
                    TestSeries.Melamine_Asia_SE_Spot,
                    SeriesItemTypeCode.RangeSeries,
                    new SeriesItem { PriceLow = 10, PriceHigh = 20 },
                    new SeriesItem { PriceLow = 20, PriceHigh = 10 }
                },
                {
                    "SINGLE_PRICE",
                    TestSeries.Petchem_8603620,
                    SeriesItemTypeCode.SingleValueSeries,
                    new SeriesItem { Price = 10 },
                    new SeriesItem { Price = 0 }
                },
                {
                    "CHARTER_PRICE",
                    TestSeries.CharterRates_Pacific_Prompt_Two_Stroke,
                    SeriesItemTypeCode.CharterRateSingleValueSeries,
                    new SeriesItem { Price = 10, DataUsed = "Interpolation/extrapolation" },
                    new SeriesItem { Price = 0, DataUsed = "INTERPOLATION/EXTRAPOLATION" }
                }
            };

        [Fact]
        public async Task Not_Create_Data_Package_When_Content_Block_Does_Not_Have_Series_Defined()
        {
            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, (ICollection<string>?)null, HttpClient)
               .SaveContentBlockDefinition()
               .InitiatePublish(Now);

            var result = await businessFlow.Execute().GetRawResponse();
            result.MatchSnapshot();
        }

        [Fact]
        public async Task Not_Create_Data_Package_When_Content_Block_Does_Not_Have_Some_Input_Series_Defined()
        {
            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, Enumerable.Empty<string>().ToList(), HttpClient)
               .SaveContentBlockDefinition()
               .AddSeriesIdToGrid(0, "LNG_China_HM3")
               .InitiatePublish(Now);

            var result = await businessFlow.Execute().GetRawResponse();

            // Assert
            result.MatchSnapshot();
        }

        [Fact]
        public async Task Not_Create_Data_Package_When_Content_Block_Not_Found()
        {
            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, SeriesIds, HttpClient)
               .InitiatePublish(Now);

            var result = await businessFlow.Execute().GetRawResponse();
            result.MatchSnapshot();
        }

        [Fact]
        public async Task Not_Create_Data_Package_When_Input_Price_Series_Is_Empty()
        {
            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, Enumerable.Empty<string>().ToList(), HttpClient)
               .SaveContentBlockDefinition()
               .InitiatePublish(Now);

            var result = await businessFlow.Execute().GetRawResponse();

            result.MatchSnapshot();
        }

        [Fact]
        public async Task Not_Create_Data_Package_When_There_Are_No_Price_Series_Items()
        {
            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, SeriesIds, HttpClient)
               .SaveContentBlockDefinition()
               .InitiatePublish(Now);

            var result = await businessFlow.Execute().GetRawResponse();

            result.MatchSnapshot();
        }

        [Fact]
        public async Task Not_Create_Data_Package_When_There_Are_Invalid_Price_Series_Items()
        {
            var invalidSeriesItem = SingleValueWithReferencePriceSeriesItemBuilder.BuildInvalidSeriesItem();
            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, SeriesIds, HttpClient)
               .SaveContentBlockDefinition()
               .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_China_HM1, Now, invalidSeriesItem)
               .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_China_HM2, Now, invalidSeriesItem)
               .InitiatePublish(Now);

            var result = await businessFlow.Execute().GetRawResponse();

            // Assert
            result.MatchSnapshot();
        }

        [Fact]
        public async Task Return_Error_When_Failing_To_Start_Workflow()
        {
            var dataPackageWorkflowServiceStub = GetService<DataPackageWorkflowServiceStub>();
            dataPackageWorkflowServiceStub.FailToStartWorkflowOnce();

            var seriesItem = SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithPremiumDiscount("TTF");
            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, SeriesIds, HttpClient)
               .SaveContentBlockDefinition()
               .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_China_HM1, Now, seriesItem)
               .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_China_HM2, Now, seriesItem)
               .InitiatePublish(Now);

            var result = await businessFlow.Execute().GetRawResponse();

            result.MatchSnapshot();
        }

        [Fact]
        public async Task Return_Error_When_Workflow_Failed_To_Publish()
        {
            var dataPackageWorkflowServiceStub = GetService<DataPackageWorkflowServiceStub>();
            dataPackageWorkflowServiceStub.FailToCallOnce();

            var seriesItem = SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithPremiumDiscount("TTF");
            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, SeriesIds, HttpClient)
               .SaveContentBlockDefinition()
               .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_China_HM1, Now, seriesItem)
               .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_China_HM2, Now, seriesItem)
               .InitiatePublish(Now);

            var result = await businessFlow.Execute().GetRawResponse();

            result.MatchSnapshot();
        }

        [Fact]
        public async Task Keep_Price_Series_Items_Status_To_DRAFT_While_Waiting_Acknowledgement()
        {
            var seriesItem = SingleValueWithReferencePriceSeriesItemBuilder.BuildDefaultValidSeriesItem();
            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, SeriesIds, HttpClient)
               .SaveContentBlockDefinition()
               .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_China_HM1, Now, seriesItem)
               .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_China_HM2, Now, seriesItem)
               .InitiatePublish(Now);

            var initiatePublishResponse = await businessFlow.Execute().GetRawResponse();

            var contentBlockWithPriceSeriesResponse = await contentBlockClient
                                                         .GetContentBlockWithPriceSeriesOnly(
                                                              ContentBlockId,
                                                              Now)
                                                         .GetRawResponse();
            var contentBlockWithWorkflowBusinessKey = await contentBlockClient
                                                         .GetContentBlockWithWorkflowBusinessKeyOnly(
                                                              ContentBlockId,
                                                              Now)
                                                         .GetRawResponse();

            // Assert
            initiatePublishResponse.MatchSnapshot(SnapshotNameExtension.Create("InitiatePublishResponse"));

            contentBlockWithPriceSeriesResponse.MatchSnapshotWithSeriesItemId(SnapshotNameExtension.Create("ContentBlockWithPriceSeries"));
            contentBlockWithWorkflowBusinessKey.MatchSnapshot(
                SnapshotNameExtension.Create("ContentBlockWithWorkflowBusinessKey"),
                options => options
                   .IsTypeField<Guid>("data.contentBlock.workflowBusinessKey")
                   .Assert(
                        option => Assert.DoesNotContain(
                            Guid.Empty,
                            option.Fields<Guid>("data.contentBlock.workflowBusinessKey"))));
        }

        [Fact]
        public async Task Set_Price_Series_Items_Status_To_PUBLISHED_When_Notified()
        {
            var seriesItem = SingleValueWithReferencePriceSeriesItemBuilder.BuildDefaultValidSeriesItem();
            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, SeriesIds, HttpClient)
               .SaveContentBlockDefinition()
               .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_China_HM1, Now, seriesItem)
               .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_China_HM2, Now, seriesItem)
               .InitiatePublish(Now)
               .AcknowledgePublished(Now);

            _ = await businessFlow.Execute();

            var contentBlockResponse = await contentBlockClient
                                          .GetContentBlockWithPriceSeriesOnly(ContentBlockId, Now)
                                          .GetRawResponse();

            contentBlockResponse.MatchSnapshotWithSeriesItemId();
        }

        [Fact]
        public async Task Create_Data_Package_With_Commentary()
        {
            var seriesItem = SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithPremiumDiscount("TTF");
            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, SeriesIds, HttpClient)
               .SaveContentBlockDefinition()
               .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_China_HM1, Now, seriesItem, string.Empty)
               .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_China_HM2, Now, seriesItem, string.Empty)
               .SaveCommentary(Now)
               .InitiatePublish(Now);

            var result = await businessFlow.Execute().GetRawResponse();

            result.MatchSnapshot();
        }

        [Fact]
        public async Task Create_Data_Package_Without_Commentary()
        {
            var seriesItem = SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithPremiumDiscount("TTF");
            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, SeriesIds, HttpClient)
               .SaveContentBlockDefinition()
               .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_China_HM1, Now, seriesItem)
               .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_China_HM2, Now, seriesItem)
               .InitiatePublish(Now);

            var result = await businessFlow.Execute().GetRawResponse();

            result.MatchSnapshot();
        }

        [Fact]
        public async Task Set_Price_Series_Items_Status_To_IN_DRAFT_When_Correction_Initiated_For_Simple_Workflow()
        {
            var seriesItem = SingleValueWithReferencePriceSeriesItemBuilder.BuildDefaultValidSeriesItem();
            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, SeriesIds, HttpClient)
               .SaveContentBlockDefinition()
               .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_China_HM1, Now, seriesItem)
               .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_China_HM2, Now, seriesItem)
               .InitiatePublish(Now)
               .AcknowledgePublished(Now)
               .InitiateCorrection(Now);

            _ = await businessFlow.Execute();

            var contentBlockResponse = await contentBlockClient
                                          .GetContentBlockWithPriceSeriesOnly(ContentBlockId, Now)
                                          .GetRawResponse();

            contentBlockResponse.MatchSnapshotWithSeriesItemId();
        }

        [Fact]
        public async Task Set_Price_Series_Items_Status_To_PUBLISHED_When_In_Correction_Flow_For_Simple_Workflow()
        {
            var seriesItem = SingleValueWithReferencePriceSeriesItemBuilder.BuildDefaultValidSeriesItem();
            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, SeriesIds, HttpClient)
               .SaveContentBlockDefinition()
               .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_China_HM1, Now, seriesItem)
               .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_China_HM2, Now, seriesItem)
               .InitiatePublish(Now)
               .AcknowledgePublished(Now)
               .InitiateCorrection(Now)
               .InitiatePublish(Now)
               .AcknowledgePublished(Now);

            _ = await businessFlow.Execute();

            var contentBlockResponse = await contentBlockClient
                                          .GetContentBlockWithPriceSeriesOnly(ContentBlockId, Now)
                                          .GetRawResponse();

            contentBlockResponse.MatchSnapshotWithSeriesItemId();
        }

        [Fact]
        public async Task Not_Initiate_Correction_When_Previous_Correction_Flow_Is_Not_Terminated_For_Simple_Workflow()
        {
            var seriesItem = SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithPremiumDiscount("TTF");
            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, SeriesIds, HttpClient)
               .SaveContentBlockDefinition()
               .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_China_HM1, Now, seriesItem)
               .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_China_HM2, Now, seriesItem)
               .InitiatePublish(Now)
               .AcknowledgePublished(Now)
               .InitiateCorrection(Now)
               .InitiateCorrection(Now);

            var response = await businessFlow.Execute().GetRawResponse();

            response.MatchSnapshot();
        }

        [Fact]
        public async Task Not_Initiate_Correction_When_Data_Package_Does_Not_Exists_For_Simple_Workflow()
        {
            var seriesItem = SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithPremiumDiscount("TTF");
            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, SeriesIds, HttpClient)
               .SaveContentBlockDefinition()
               .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_China_HM1, Now, seriesItem)
               .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_China_HM2, Now, seriesItem)
               .InitiateCorrection(Now);

            var response = await businessFlow.Execute().GetRawResponse();

            response.MatchSnapshot();
        }

        [Fact]
        public async Task Not_Create_Data_Package_When_Inputs_Are_Not_Published_For_Full_Month_Derived_Price()
        {
            string[] inputSeriesIds = { TestSeries.LNG_China_HM1, TestSeries.LNG_China_HM2, TestSeries.LNG_China_HM3 };

            var seriesItem = SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithPremiumDiscount("TTF");
            var halfMonthSeriesBusinessFlow = new PriceEntryBusinessFlow("HalfMonthContentBlockId", inputSeriesIds, HttpClient)
                .SaveContentBlockDefinition()
                .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_China_HM1, Now, seriesItem)
                .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_China_HM2, Now, seriesItem)
                .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_China_HM3, Now, seriesItem);

            await halfMonthSeriesBusinessFlow.Execute();

            string[] derivedSeriesIds = { TestSeries.LNG_China_MM1 };

            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, derivedSeriesIds, HttpClient)
                   .SaveContentBlockDefinition()
                   .InitiatePublish(Now);

            var result = await businessFlow.Execute().GetRawResponse();

            result.MatchSnapshot();
        }

        [Theory]
        [InlineData("LNG DES Argentina", TestSeries.LNG_Argentina_MM1)]
        [InlineData("LNG FOB Reload NWE", TestSeries.LNG_Reload_NWE_MM1)]
        public async Task Not_Create_Data_Package_When_Reference_Price_Is_Not_Published(string referenceMarketName, string referenceSeriesId)
        {
            string[] referenceSeriesIds = { referenceSeriesId };

            var referencePriceSeriesItem = SingleValueWithReferencePriceSeriesItemBuilder.BuildDefaultValidSeriesItem();
            var referencePriceBusinessFlow = new PriceEntryBusinessFlow("ReferencePriceContentBlockId", referenceSeriesIds, HttpClient)
                   .SaveContentBlockDefinition()
                   .SaveSingleValueWithReferencePriceSeriesItem(referenceSeriesId, Now, referencePriceSeriesItem);

            await referencePriceBusinessFlow.Execute();

            string[] seriesId = { TestSeries.LNG_Dubai_MM1 };

            var seriesItem = new SeriesItem
            {
                AssessmentMethod = AssessmentMethod.PremiumDiscount.Value,
                ReferenceMarketName = referenceMarketName,
                PremiumDiscount = 10,
                DataUsed = "Bid/offer"
            };

            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, seriesId, HttpClient)
                   .SaveContentBlockDefinition()
                   .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_Dubai_MM1, Now, seriesItem)
                   .InitiatePublish(Now);

            var result = await businessFlow.Execute().GetRawResponse();

            result.MatchSnapshot(SnapshotNameExtension.Create(referenceMarketName));
        }

        [Theory]
        [InlineData("LNG DES Argentina", TestSeries.LNG_Argentina_MM1)]
        [InlineData("LNG FOB Reload NWE", TestSeries.LNG_Reload_NWE_MM1)]
        public async Task Create_Data_Package_When_Reference_Price_Is_Published(string referenceMarketName, string referenceSeriesId)
        {
            string[] referenceSeriesIds = { referenceSeriesId };

            var referencePriceSeriesItem = SingleValueWithReferencePriceSeriesItemBuilder.BuildDefaultValidSeriesItem();
            var referencePriceBusinessFlow = new PriceEntryBusinessFlow("ReferencePriceContentBlockId", referenceSeriesIds, HttpClient)
                .SaveContentBlockDefinition()
                .SaveSingleValueWithReferencePriceSeriesItem(referenceSeriesId, Now, referencePriceSeriesItem)
                .InitiatePublish(Now)
                .AcknowledgePublished(Now);

            await referencePriceBusinessFlow.Execute();

            string[] seriesId = { TestSeries.LNG_Dubai_MM1 };

            var seriesItem = new SeriesItem
            {
                AssessmentMethod = AssessmentMethod.PremiumDiscount.Value,
                ReferenceMarketName = referenceMarketName,
                PremiumDiscount = 10,
                DataUsed = "Bid/offer"
            };

            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, seriesId, HttpClient)
                .SaveContentBlockDefinition()
                .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_Dubai_MM1, Now, seriesItem)
                .InitiatePublish(Now);

            var result = await businessFlow.Execute().GetRawResponse();

            result.MatchSnapshot(SnapshotNameExtension.Create(referenceMarketName));
        }

        [Theory]
        [InlineData("LNG EAX Index", TestSeries.LNG_EAX_Index_MM1)]
        [InlineData("LNG DES India", TestSeries.LNG_India_MM1)]
        public async Task Not_Create_Data_Package_When_EAX_India_Reference_Price_Is_Not_Published(string referenceMarketName, string referenceSeriesId)
        {
            string[] halfMonthInputSeriesIds =
            {
                TestSeries.LNG_India_HM1, TestSeries.LNG_India_HM2, TestSeries.LNG_India_HM3,
                TestSeries.LNG_China_HM1, TestSeries.LNG_China_HM2, TestSeries.LNG_China_HM3,
                TestSeries.LNG_Japan_HM1, TestSeries.LNG_Japan_HM2, TestSeries.LNG_Japan_HM3,
                TestSeries.LNG_South_Korea_HM1, TestSeries.LNG_South_Korea_HM2, TestSeries.LNG_South_Korea_HM3,
                TestSeries.LNG_Taiwan_HM1, TestSeries.LNG_Taiwan_HM2, TestSeries.LNG_Taiwan_HM3
            };

            var halfMonthSeriesItem = SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithPremiumDiscount("TTF");
            var halfMonthSeriesBusinessFlow = new PriceEntryBusinessFlow("HalfMonthContentBlockId", halfMonthInputSeriesIds, HttpClient)
                   .SaveContentBlockDefinition();

            foreach (var halfMonthInputSeriesId in halfMonthInputSeriesIds)
            {
                halfMonthSeriesBusinessFlow.SaveSingleValueWithReferencePriceSeriesItem(halfMonthInputSeriesId, Now, halfMonthSeriesItem);
            }

            halfMonthSeriesBusinessFlow
               .InitiatePublish(Now)
               .AcknowledgePublished(Now);

            await halfMonthSeriesBusinessFlow.Execute();

            string[] fullMonthInputSeriesIds = { TestSeries.LNG_China_MM1, TestSeries.LNG_Japan_MM1, TestSeries.LNG_South_Korea_MM1, TestSeries.LNG_Taiwan_MM1 };

            var fullMonthSeriesBusinessFlow = new PriceEntryBusinessFlow("FullMonthContentBlockId", fullMonthInputSeriesIds, HttpClient)
                   .SaveContentBlockDefinition();

            foreach (var fullMonthInputSeriesId in fullMonthInputSeriesIds)
            {
                halfMonthSeriesBusinessFlow.SaveSingleValueWithReferencePriceSeriesItem(fullMonthInputSeriesId, Now, new SeriesItem { Price = 10 });
            }

            halfMonthSeriesBusinessFlow
               .InitiatePublish(Now)
               .AcknowledgePublished(Now);

            await fullMonthSeriesBusinessFlow.Execute();

            string[] referenceSeriesIds = { referenceSeriesId };

            var referencePriceBusinessFlow = new PriceEntryBusinessFlow("EAXIndiaContentBlockId", referenceSeriesIds, HttpClient)
               .SaveContentBlockDefinition()
               .SaveSingleValuePriceSeriesItem(referenceSeriesId, Now, new SeriesItem { Price = 10 });

            await referencePriceBusinessFlow.Execute();

            string[] seriesId = { TestSeries.LNG_Dubai_MM1 };

            var seriesItem = new SeriesItem
            {
                AssessmentMethod = AssessmentMethod.PremiumDiscount.Value,
                ReferenceMarketName = referenceMarketName,
                PremiumDiscount = 10,
                DataUsed = "Bid/offer"
            };

            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, seriesId, HttpClient)
                   .SaveContentBlockDefinition()
                   .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_Dubai_MM1, Now, seriesItem)
                   .InitiatePublish(Now);

            var result = await businessFlow.Execute().GetRawResponse();

            result.MatchSnapshot(SnapshotNameExtension.Create(referenceMarketName));
        }

        [Theory]
        [InlineData("LNG EAX Index", TestSeries.LNG_EAX_Index_MM1)]
        [InlineData("LNG DES India", TestSeries.LNG_India_MM1)]
        public async Task Create_Data_Package_When_EAX_India_Reference_Price_Is_Published(string referenceMarketName, string referenceSeriesId)
        {
            string[] halfMonthInputSeriesIds =
            {
                TestSeries.LNG_India_HM1, TestSeries.LNG_India_HM2, TestSeries.LNG_India_HM3,
                TestSeries.LNG_China_HM1, TestSeries.LNG_China_HM2, TestSeries.LNG_China_HM3,
                TestSeries.LNG_Japan_HM1, TestSeries.LNG_Japan_HM2, TestSeries.LNG_Japan_HM3,
                TestSeries.LNG_South_Korea_HM1, TestSeries.LNG_South_Korea_HM2, TestSeries.LNG_South_Korea_HM3,
                TestSeries.LNG_Taiwan_HM1, TestSeries.LNG_Taiwan_HM2, TestSeries.LNG_Taiwan_HM3
            };

            var halfMonthSeriesItem = SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithPremiumDiscount("TTF");
            var halfMonthSeriesBusinessFlow = new PriceEntryBusinessFlow("HalfMonthContentBlockId", halfMonthInputSeriesIds, HttpClient)
                   .SaveContentBlockDefinition();

            foreach (var halfMonthInputSeriesId in halfMonthInputSeriesIds)
            {
                halfMonthSeriesBusinessFlow.SaveSingleValueWithReferencePriceSeriesItem(halfMonthInputSeriesId, Now, halfMonthSeriesItem);
            }

            halfMonthSeriesBusinessFlow
               .InitiatePublish(Now)
               .AcknowledgePublished(Now);

            await halfMonthSeriesBusinessFlow.Execute();

            string[] fullMonthInputSeriesIds =
            {
                TestSeries.LNG_China_MM1,
                TestSeries.LNG_Japan_MM1,
                TestSeries.LNG_South_Korea_MM1,
                TestSeries.LNG_Taiwan_MM1
            };

            var fullMonthSeriesBusinessFlow = new PriceEntryBusinessFlow("FullMonthContentBlockId", fullMonthInputSeriesIds, HttpClient)
                   .SaveContentBlockDefinition();

            foreach (var fullMonthInputSeriesId in fullMonthInputSeriesIds)
            {
                fullMonthSeriesBusinessFlow.SaveSingleValuePriceSeriesItem(fullMonthInputSeriesId, Now, new SeriesItem { Price = 10 });
            }

            fullMonthSeriesBusinessFlow
               .InitiatePublish(Now)
               .AcknowledgePublished(Now);

            await fullMonthSeriesBusinessFlow.Execute();

            string[] referenceSeriesIds = { referenceSeriesId };

            var referencePriceBusinessFlow = new PriceEntryBusinessFlow("EAXIndiaContentBlockId", referenceSeriesIds, HttpClient)
                   .SaveContentBlockDefinition()
                   .SaveSingleValuePriceSeriesItem(referenceSeriesId, Now, new SeriesItem { Price = 10 })
                   .InitiatePublish(Now)
                   .AcknowledgePublished(Now);

            await referencePriceBusinessFlow.Execute();

            string[] seriesId = { TestSeries.LNG_Dubai_MM1 };

            var seriesItem = new SeriesItem
            {
                AssessmentMethod = AssessmentMethod.PremiumDiscount.Value,
                ReferenceMarketName = referenceMarketName,
                PremiumDiscount = 10,
                DataUsed = "Bid/offer"
            };

            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, seriesId, HttpClient)
                   .SaveContentBlockDefinition()
                   .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_Dubai_MM1, Now, seriesItem)
                   .InitiatePublish(Now);

            var result = await businessFlow.Execute().GetRawResponse();

            result.MatchSnapshot(SnapshotNameExtension.Create(referenceMarketName));
        }

        [Fact]
        public async Task Create_Data_Package_When_Inputs_Are_Published_For_Full_Month_Derived_Price()
        {
            string[] inputSeriesIds =
            {
                TestSeries.LNG_China_HM1,
                TestSeries.LNG_China_HM2,
                TestSeries.LNG_China_HM3
            };

            var halfMonthSeriesBusinessFlow = new PriceEntryBusinessFlow("HalfMonthContentBlockId", inputSeriesIds, HttpClient)
                   .SaveContentBlockDefinition();

            foreach (var inputSeriesId in inputSeriesIds)
            {
                halfMonthSeriesBusinessFlow.SaveSingleValueWithReferencePriceSeriesItem(inputSeriesId, Now, SingleValueWithReferencePriceSeriesItemBuilder.BuildDefaultValidSeriesItem());
            }

            halfMonthSeriesBusinessFlow
               .InitiatePublish(Now)
               .AcknowledgePublished(Now);

            await halfMonthSeriesBusinessFlow.Execute();

            string[] derivedSeriesIds = { TestSeries.LNG_China_MM1 };

            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, derivedSeriesIds, HttpClient)
                   .SaveContentBlockDefinition()
                   .SaveSingleValuePriceSeriesItem(TestSeries.LNG_China_MM1, Now, SingleValuePriceSeriesItemBuilder.BuildDefaultValidSeriesItem())
                   .InitiatePublish(Now);

            var result = await businessFlow.Execute().GetRawResponse();

            result.MatchSnapshot();
        }

        [Fact]
        public async Task Not_Create_Data_Package_When_Inputs_Are_Not_Published_For_EAX_Derived_Price()
        {
            string[] inputSeriesIds =
            {
                TestSeries.LNG_China_HM1,
                TestSeries.LNG_Japan_HM1,
                TestSeries.LNG_South_Korea_HM1,
                TestSeries.LNG_Taiwan_HM1
            };

            var eaxSeriesBusinessFlow = new PriceEntryBusinessFlow("EAXContentBlockId", inputSeriesIds, HttpClient)
                   .SaveContentBlockDefinition();

            foreach (var inputSeriesId in inputSeriesIds)
            {
                eaxSeriesBusinessFlow.SaveSingleValueWithReferencePriceSeriesItem(inputSeriesId, Now, SingleValueWithReferencePriceSeriesItemBuilder.BuildDefaultValidSeriesItem());
            }

            await eaxSeriesBusinessFlow.Execute();

            string[] derivedSeriesIds = { TestSeries.LNG_EAX_Index_HM2 };

            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, derivedSeriesIds, HttpClient)
                   .SaveContentBlockDefinition()
                   .InitiatePublish(Now);

            var result = await businessFlow.Execute().GetRawResponse();

            result.MatchSnapshot();
        }

        [Fact]
        public async Task Create_Data_Package_When_Inputs_Are_Published_For_EAX_Derived_Price()
        {
            string[] inputSeriesIds =
            {
                TestSeries.LNG_China_HM1,
                TestSeries.LNG_Japan_HM1,
                TestSeries.LNG_South_Korea_HM1,
                TestSeries.LNG_Taiwan_HM1
            };

            var eaxSeriesBusinessFlow = new PriceEntryBusinessFlow("EAXContentBlockId", inputSeriesIds, HttpClient)
                   .SaveContentBlockDefinition();

            foreach (var inputSeriesId in inputSeriesIds)
            {
                eaxSeriesBusinessFlow.SaveSingleValueWithReferencePriceSeriesItem(inputSeriesId, Now, SingleValueWithReferencePriceSeriesItemBuilder.BuildDefaultValidSeriesItem());
            }

            eaxSeriesBusinessFlow
               .InitiatePublish(Now)
               .AcknowledgePublished(Now);

            await eaxSeriesBusinessFlow.Execute();

            string[] derivedSeriesIds = { TestSeries.LNG_EAX_Index_HM2 };

            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, derivedSeriesIds, HttpClient)
                   .SaveContentBlockDefinition()
                   .SaveSingleValuePriceSeriesItem(TestSeries.LNG_EAX_Index_HM2, Now, SingleValuePriceSeriesItemBuilder.BuildDefaultValidSeriesItem())
                   .InitiatePublish(Now);

            var result = await businessFlow.Execute().GetRawResponse();

            result.MatchSnapshot();
        }

        [Fact]
        public async Task Set_Price_Series_Items_Status_To_CORRECTION_DRAFT_When_Correction_Initiated_For_Simple_Workflow()
        {
            var seriesItem = SingleValueWithReferencePriceSeriesItemBuilder.BuildDefaultValidSeriesItem();
            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, SeriesIds, HttpClient)
               .SaveContentBlockDefinition()
               .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_China_HM1, Now, seriesItem)
               .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_China_HM2, Now, seriesItem)
               .InitiatePublish(Now)
               .AcknowledgePublished(Now)
               .InitiateCorrection(Now)
               .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_China_HM1, Now, seriesItem);

            _ = await businessFlow.Execute();

            var contentBlockResponse = await contentBlockClient
                                          .GetContentBlockWithPriceSeriesOnly(ContentBlockId, Now)
                                          .GetRawResponse();

            contentBlockResponse.MatchSnapshotWithSeriesItemId();
        }

        [Fact]
        public async Task Error_When_Content_Block_Not_Found_When_Initiate_Advanced_Correction()
        {
            var seriesItem = SingleValueWithReferencePriceSeriesItemBuilder.BuildDefaultValidSeriesItem();
            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, new List<string> { PriceSeriesId }, HttpClient)
               .InitiateCorrection(Now);
            var response = await businessFlow.Execute().GetRawResponse();

            response.MatchSnapshot();
        }

        [Fact]
        public async Task Error_When_DataPackage_Not_Found_When_Initiate_Advanced_Correction()
        {
            var seriesItem = SingleValueWithReferencePriceSeriesItemBuilder.BuildDefaultValidSeriesItem();
            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, new List<string> { PriceSeriesId }, HttpClient)
               .SaveContentBlockDefinition()
               .SaveRangePriceSeriesItem(PriceSeriesId, Now, seriesItem)
               .InitiateCorrection(Now);

            var response = await businessFlow.Execute().GetRawResponse();

            response.MatchSnapshot();
        }

        [Fact]
        public async Task Error_When_Initiating_Advance_Correction_Workflow_Failed()
        {
            var series = TestSeries.Melamine_Asia_SE_Spot;
            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, new List<string> { series }, HttpClient)
                   .SaveContentBlockDefinition()
                   .SavePriceSeriesItem(
                        series,
                        Now,
                        SeriesItemTypeCode.RangeSeries,
                        new SeriesItem { PriceLow = 10, PriceHigh = 20 })
                    .InitiateSendForReview(Now)
                    .AcknowledgeSendForReview(Now)
                    .InitiateStartReview(Now)
                    .AcknowledgeStartReview(Now)
                    .InitiateApproval(Now)
                    .AcknowledgeApproval(Now)
                    .AcknowledgePublished(Now);
            _ = await businessFlow.Execute();

            var dataPackageWorkflowServiceStub = GetService<DataPackageWorkflowServiceStub>();
            dataPackageWorkflowServiceStub.FailToStartWorkflowOnce();

            var businessFlowCorrection = new PriceEntryBusinessFlow(ContentBlockId, new List<string> { series }, HttpClient)
                .InitiateCorrection(Now);

            var response = await businessFlowCorrection.Execute().GetRawResponse();

            response.MatchSnapshot();
        }

        [Fact]
        public async Task Publish_Price_Series_When_Fulfilment_Period_Is_1545D_And_Use_Month_Plus_One_As_Reference_Price()
        {
            // Arrange
            var lngReloadNweSeriesItem = new SeriesItem
            {
                AssessmentMethod = AssessmentMethod.Assessed.Value,
                Price = 105,
                DataUsed = "Bid/offer"
            };

            var lngReloadNweBusinessFlow = new PriceEntryBusinessFlow("cbLngReloadNwe", new List<string> { TestSeries.LNG_Reload_NWE_MM1 }, HttpClient)
               .SaveContentBlockDefinition()
               .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_Reload_NWE_MM1, Now, lngReloadNweSeriesItem)
               .InitiatePublish(Now)
               .AcknowledgePublished(Now);

            await lngReloadNweBusinessFlow.Execute();

            var seriesItem = new SeriesItem
            {
                AssessmentMethod = AssessmentMethod.PremiumDiscount.Value,
                ReferenceMarketName = "LNG FOB Reload NWE",
                PremiumDiscount = 5,
                DataUsed = "Bid/offer"
            };

            var periodGeneratorServiceStub = GetService<PeriodGeneratorServiceStub>();
            periodGeneratorServiceStub.ReplaceAbsolutePeriods(new List<AbsolutePeriod>
            {
                new AbsolutePeriod
                     {
                        Code = "M2402",
                        PeriodCode = "M1",
                        Label = "February 2024",
                        FromDate = new DateOnly(2024, 02, 01),
                        UntilDate = new DateOnly(2024, 02, 29)
                     },
            });

            // Act
            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, new List<string> { TestSeries.LNG_Reload_Spain_Month1 }, HttpClient)
               .SaveContentBlockDefinition()
               .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_Reload_Spain_Month1, Now, seriesItem)
               .InitiatePublish(Now)
               .AcknowledgePublished(Now);

            await businessFlow.Execute();

            var response = await contentBlockClient
                              .GetContentBlockWithPriceSeriesOnly(ContentBlockId, Now)
                              .GetRawResponse();

            periodGeneratorServiceStub.ClearAbsolutePeriods();

            response.MatchSnapshotWithSeriesItemId();
        }

        [Fact]
        public async Task Create_Data_Package_For_Charter_Rates()
        {
            string[] charterRateSeriesIds =
            [
                TestSeries.CharterRates_Pacific_Prompt_Steam,
                TestSeries.CharterRates_Pacific_Prompt_Two_Stroke
            ];

            var seriesItem = CharterRateSingleValuePriceSeriesItemBuilder.BuildDefaultValidSeriesItem();
            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, charterRateSeriesIds, HttpClient)
               .SaveContentBlockDefinition()
               .SaveCharterRateSingleValuePriceSeriesItem(TestSeries.CharterRates_Pacific_Prompt_Steam, Now, seriesItem)
               .SaveCharterRateSingleValuePriceSeriesItem(TestSeries.CharterRates_Pacific_Prompt_Two_Stroke, Now, seriesItem)
               .InitiatePublish(Now);

            var result = await businessFlow.Execute().GetRawResponse();

            result.MatchSnapshot();
        }

        [Fact]
        public async Task Not_Create_Data_Package_When_Charter_Rate_Series_Are_Invalid()
        {
            string[] charterRateSeriesIds =
            [
                TestSeries.CharterRates_Pacific_Prompt_Steam,
                TestSeries.CharterRates_Pacific_Prompt_Two_Stroke
            ];

            var invalidSeriesItem = CharterRateSingleValuePriceSeriesItemBuilder.BuildInvalidSeriesItem();
            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, charterRateSeriesIds, HttpClient)
               .SaveContentBlockDefinition()
               .SaveCharterRateSingleValuePriceSeriesItem(TestSeries.CharterRates_Pacific_Prompt_Steam, Now, invalidSeriesItem)
               .SaveCharterRateSingleValuePriceSeriesItem(TestSeries.CharterRates_Pacific_Prompt_Two_Stroke, Now, invalidSeriesItem)
               .InitiatePublish(Now);

            var result = await businessFlow.Execute().GetRawResponse();

            result.MatchSnapshot();
        }

        [Theory]
        [MemberData(nameof(ValidSeriesItemsForAdvancedCorrectionWorkflow))]
        public async Task
            Throw_Error_For_Data_Package_In_Advanced_Correction_Workflow_When_Status_Transitioned_With_No_Pending_Changes_To_Prices_Or_Commentary(
                string scenario,
                string seriesId,
                SeriesItemTypeCode seriesItemTypeCode,
                SeriesItem seriesItem)
        {
            var assessedDateTime = Now;
            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, new List<string>() { seriesId }, HttpClient)
               .SaveContentBlockDefinition()
               .SavePriceSeriesItem(seriesId, Now, seriesItemTypeCode, seriesItem)
               .InitiateSendForReview(assessedDateTime)
               .AcknowledgeSendForReview(assessedDateTime)
               .InitiateStartReview(assessedDateTime)
               .AcknowledgeStartReview(assessedDateTime)
               .InitiateApproval(assessedDateTime)
               .AcknowledgeApproval(assessedDateTime)
               .AcknowledgePublished(assessedDateTime)
               .InitiateCorrection(assessedDateTime)
               .AdvancedCorrectionInitiateSendForReview(assessedDateTime);

            var result = await businessFlow.Execute().GetRawResponse();

            result.MatchSnapshotWithSeriesItemId(SnapshotNameExtension.Create(scenario));
        }

        [Theory]
        [MemberData(nameof(InvalidSeriesItemsForAdvancedCorrectionWorkflow))]
        public async Task
            Throw_Error_For_Data_Package_In_Advanced_Correction_Workflow_When_Status_Transitioned_With_Invalid_Correction(
                string scenario,
                string seriesId,
                SeriesItemTypeCode seriesItemTypeCode,
                SeriesItem seriesItem,
                SeriesItem correctedSeriesItem)
        {
            var assessedDateTime = Now;
            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, new List<string>() { seriesId }, HttpClient)
               .SaveContentBlockDefinition()
               .SavePriceSeriesItem(seriesId, Now, seriesItemTypeCode, seriesItem)
               .InitiateSendForReview(assessedDateTime)
               .AcknowledgeSendForReview(assessedDateTime)
               .InitiateStartReview(assessedDateTime)
               .AcknowledgeStartReview(assessedDateTime)
               .InitiateApproval(assessedDateTime)
               .AcknowledgeApproval(assessedDateTime)
               .AcknowledgePublished(assessedDateTime)
               .InitiateCorrection(assessedDateTime)
               .SavePriceSeriesItem(seriesId, Now, seriesItemTypeCode, correctedSeriesItem, "correction")
               .AdvancedCorrectionInitiateSendForReview(assessedDateTime);

            var result = await businessFlow.Execute().GetRawResponse();

            result.MatchSnapshotWithSeriesItemId(SnapshotNameExtension.Create(scenario));
        }

        [Fact]
        public async Task Send_PublishOnDate_To_Workflow_When_Price_Is_Published_On_Schedule_Date()
        {
            var periodGeneratorServiceStub = GetService<PeriodGeneratorServiceStub>();

            var schedulePublishDate = TestData.Now;

            var events = new List<Event>
            {
                new()
                {
                    EventTime = schedulePublishDate.AddHours(8).AddMinutes(30)
                }
            };

            periodGeneratorServiceStub.SetEvents(events);

            var businessFlow =
               new PriceEntryBusinessFlow(
                   ContentBlockId,
                   new List<string>
                   {
                        TestSeries.Melamine_China_Spot_Cif_2_6_Weeks,
                        TestSeries.Melamine_Asia_SE_Spot,
                   },
                   HttpClient)
               .SaveContentBlockDefinition()
               .SavePriceSeriesItem(TestSeries.Melamine_China_Spot_Cif_2_6_Weeks, Now, SeriesItemTypeCode.RangeSeries, new SeriesItem { PriceLow = 10, PriceHigh = 20 })
               .SavePriceSeriesItem(TestSeries.Melamine_Asia_SE_Spot, Now, SeriesItemTypeCode.RangeSeries, new SeriesItem { PriceLow = 10, PriceHigh = 20 })
               .PublishWithAdvanceWorkflow(schedulePublishDate);

            await businessFlow.Execute();

            periodGeneratorServiceStub.ClearEvents();

            dataPackageWorkflowServiceStub.Requests.MatchSnapshotForWorkflowRequests();
        }

        [Fact]
        public async Task Send_PublishOnDate_As_CurrentDateTime_To_Workflow_When_Publishing_On_Non_Schedule_Date()
        {
            var periodGeneratorServiceStub = GetService<PeriodGeneratorServiceStub>();

            var schedulePublishDate = TestData.Now;

            var events = new List<Event>();

            periodGeneratorServiceStub.SetEvents(events);

            var businessFlow =
               new PriceEntryBusinessFlow(
                   ContentBlockId,
                   new List<string>
                   {
                        TestSeries.CharterRates_Pacific_Prompt_Two_Stroke,
                        TestSeries.CharterRates_Pacific_Prompt_Steam,
                   },
                   HttpClient)
               .SaveContentBlockDefinition()
               .SavePriceSeriesItem(TestSeries.CharterRates_Pacific_Prompt_Two_Stroke, Now, SeriesItemTypeCode.CharterRateSingleValueSeries, new SeriesItem { Price = 10, DataUsed = "Bid/offer" })
               .SavePriceSeriesItem(TestSeries.CharterRates_Pacific_Prompt_Steam, Now, SeriesItemTypeCode.CharterRateSingleValueSeries, new SeriesItem { Price = 10, DataUsed = "Bid/offer" })
               .PublishWithAdvanceWorkflow(schedulePublishDate);

            await businessFlow.Execute();

            periodGeneratorServiceStub.ClearEvents();

            dataPackageWorkflowServiceStub.Requests.MatchSnapshotForWorkflowRequests();
        }

        [Fact]
        public async Task Create_Data_Package_When_Input_Prices_And_Derived_Ones_Are_In_The_Same_Content_Block_And_Input_Prices_Are_Not_Published_While_Publishing()
        {
            var priceSeriesIds = ContentBlockQueryHelper.CreatePriceSeriesIds([TestSeries.LNG_India_HM1, TestSeries.LNG_India_HM2], [TestSeries.LNG_India_MM1]);

            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, priceSeriesIds, HttpClient)
               .SaveContentBlockDefinition()
               .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_India_HM1, Now, SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice())
               .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_India_HM2, Now, SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice())
               .SaveSingleValuePriceSeriesItem(TestSeries.LNG_India_MM1, Now, SingleValuePriceSeriesItemBuilder.BuildDefaultValidSeriesItem())
               .InitiatePublish(Now);

            var result = await businessFlow.Execute().GetRawResponse();

            result.MatchSnapshot();
        }

        [Fact]
        public async Task Create_Data_Package_When_Reference_Prices_Has_Not_Published_Derived_Price_In_Same_ContentBlock_While_Publishing()
        {
            var priceSeriesIds = ContentBlockQueryHelper.CreatePriceSeriesIds([TestSeries.LNG_India_HM1, TestSeries.LNG_India_HM2], [TestSeries.LNG_India_MM1], [TestSeries.LNG_Dubai_MM1]);

            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, priceSeriesIds, HttpClient)
               .SaveContentBlockDefinition()
               .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_India_HM1, Now, SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice())
               .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_India_HM2, Now, SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice())
               .SaveSingleValuePriceSeriesItem(TestSeries.LNG_India_MM1, Now, SingleValuePriceSeriesItemBuilder.BuildDefaultValidSeriesItem())
               .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_Dubai_MM1, Now, SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithPremiumDiscount("LNG DES India"))
               .InitiatePublish(Now);

            var result = await businessFlow.Execute().GetRawResponse();

            result.MatchSnapshot();
        }

        [Fact]
        public async Task Create_Data_Package_When_Multiple_Regions_Input_Prices_And_Derived_Ones_Are_In_The_Same_Content_Block_And_Input_Prices_Are_Not_Published_While_Publishing()
        {
            var priceSeriesIds = ContentBlockQueryHelper.CreatePriceSeriesIds(
                [
                    TestSeries.LNG_India_HM1, TestSeries.LNG_India_HM2,
                    TestSeries.LNG_Japan_HM1, TestSeries.LNG_Japan_HM2
                ],
                [
                    TestSeries.LNG_India_MM1,
                    TestSeries.LNG_Japan_MM1
                ]);

            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, priceSeriesIds, HttpClient)
               .SaveContentBlockDefinition()
               .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_India_HM1, Now, SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice())
               .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_India_HM2, Now, SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice())
               .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_Japan_HM1, Now, SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice())
               .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_Japan_HM2, Now, SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice())
               .InitiatePublish(Now);

            var result = await businessFlow.Execute().GetRawResponse();

            result.MatchSnapshot();
        }

        [Fact]
        public async Task Create_Data_Package_When_EAX_HalfMonth_And_FullMonth_Series_Are_Added_In_Same_Grid()
        {
            string[] inputPrices = [
                    TestSeries.LNG_China_HM1, TestSeries.LNG_China_HM2,
                TestSeries.LNG_Japan_HM1, TestSeries.LNG_Japan_HM2,
                TestSeries.LNG_South_Korea_HM1, TestSeries.LNG_South_Korea_HM2,
                TestSeries.LNG_Taiwan_HM2, TestSeries.LNG_Taiwan_HM1
                ];

            var priceSeriesIds = ContentBlockQueryHelper.CreatePriceSeriesIds(
                inputPrices,
                [TestSeries.LNG_China_MM1, TestSeries.LNG_Japan_MM1, TestSeries.LNG_South_Korea_MM1, TestSeries.LNG_Taiwan_MM1],
                [TestSeries.LNG_EAX_Index_MM1]);

            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, priceSeriesIds, HttpClient)
               .SaveContentBlockDefinition();

            foreach (var inputPrice in inputPrices)
            {
                businessFlow.SaveSingleValueWithReferencePriceSeriesItem(inputPrice, Now, SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice());
            }

            businessFlow.InitiatePublish(Now);

            var result = await businessFlow.Execute().GetRawResponse();

            result.MatchSnapshot();
        }

        [Fact]
        public async Task Create_Data_Package_With_Some_Derivation_Inputs_Published_Elsewhere_And_Rest_In_The_Same_Block()
        {
            var someSeriesPublishedElsewhereSeriesIds = ContentBlockQueryHelper.CreatePriceSeriesIds([TestSeries.LNG_China_HM4]);
            var publishedFlow =
                new PriceEntryBusinessFlow(
                        "PublishedInputsInOtherContentBlock",
                        someSeriesPublishedElsewhereSeriesIds,
                        HttpClient)
                   .SaveSingleValueWithReferencePriceSeriesItem(
                        TestSeries.LNG_China_HM4,
                        Now,
                        SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice())
                   .SaveContentBlockDefinition()
                   .InitiatePublish(Now)
                   .AcknowledgePublished(Now);
            await publishedFlow.Execute();

            string[] inputPrices = [
                TestSeries.LNG_China_HM1, TestSeries.LNG_China_HM2, TestSeries.LNG_China_HM3
            ];

            var priceSeriesIds = ContentBlockQueryHelper.CreatePriceSeriesIds(
                inputPrices,
                [TestSeries.LNG_China_MM1, TestSeries.LNG_China_MM2]);

            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, priceSeriesIds, HttpClient)
               .SaveContentBlockDefinition();

            foreach (var inputPrice in inputPrices)
            {
                businessFlow.SaveSingleValueWithReferencePriceSeriesItem(inputPrice, Now, SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice());
            }

            businessFlow.InitiatePublish(Now);

            var result = await businessFlow.Execute().GetRawResponse();

            result.MatchSnapshot();
        }

        [Fact]
        public async Task Not_Create_Data_Package_When_Second_Derivation_Input_Not_In_Same_Content_Block_And_Not_Published()
        {
            // second derivation input defined in another content block and not published
            await new PriceEntryBusinessFlow(
                    "NotPublishedInputsInOtherContentBlock",
                    ContentBlockQueryHelper.CreatePriceSeriesIds([TestSeries.LNG_China_HM4]),
                    HttpClient)
               .SaveContentBlockDefinition()
               .SaveSingleValueWithReferencePriceSeriesItem(
                    TestSeries.LNG_China_HM4,
                    Now,
                    SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice())
               .Execute();

            string[] inputPrices = [
                TestSeries.LNG_China_HM1, TestSeries.LNG_China_HM2, TestSeries.LNG_China_HM3
            ];

            var priceSeriesIds = ContentBlockQueryHelper.CreatePriceSeriesIds(
                inputPrices,
                [TestSeries.LNG_China_MM1, TestSeries.LNG_China_MM2]);

            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, priceSeriesIds, HttpClient)
               .SaveContentBlockDefinition();

            foreach (var inputPrice in inputPrices)
            {
                businessFlow.SaveSingleValueWithReferencePriceSeriesItem(inputPrice, Now, SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice());
            }

            businessFlow.InitiatePublish(Now);

            var result = await businessFlow.Execute().GetRawResponse();

            result.MatchSnapshot();
        }

        [Fact]
        public async Task Not_Create_Data_Package_When_Derivation_Inputs_Not_In_Same_Grid_And_Not_Published()
        {
            string[] inputPrices = [
                    TestSeries.LNG_China_HM1, TestSeries.LNG_China_HM2,
                TestSeries.LNG_Japan_HM1, TestSeries.LNG_Japan_HM2,
                TestSeries.LNG_South_Korea_HM1, TestSeries.LNG_South_Korea_HM2,
                TestSeries.LNG_Taiwan_HM2, TestSeries.LNG_Taiwan_HM1
                ];

            var priceSeriesIds = ContentBlockQueryHelper.CreatePriceSeriesIds(
                inputPrices,
                [TestSeries.LNG_EAX_Index_MM1]);

            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, priceSeriesIds, HttpClient)
               .SaveContentBlockDefinition();

            foreach (var inputPrice in inputPrices)
            {
                businessFlow.SaveSingleValueWithReferencePriceSeriesItem(inputPrice, Now, SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice());
            }

            businessFlow.InitiatePublish(Now);

            var result = await businessFlow.Execute().GetRawResponse();

            result.MatchSnapshot();
        }

        [Fact]
        public async Task Not_Create_Data_Package_When_Reference_Price_Is_Not_Published_And_Not_In_Same_ContentBlock()
        {
            string[] premiumDiscountPrices = [TestSeries.LNG_Dubai_MM1, TestSeries.LNG_Dubai_MM2];
            string[] assessedPrices = [TestSeries.LNG_India_HM1, TestSeries.LNG_India_HM2, TestSeries.LNG_India_HM3, TestSeries.LNG_India_HM5, TestSeries.LNG_India_HM6];
            string[] referencePrices = [TestSeries.LNG_India_MM1];

            var priceSeriesIds = ContentBlockQueryHelper.CreatePriceSeriesIds(premiumDiscountPrices, assessedPrices, referencePrices);

            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, priceSeriesIds, HttpClient)
               .SaveContentBlockDefinition();

            foreach (var premiumDiscountPrice in premiumDiscountPrices)
            {
                businessFlow.SaveSingleValueWithReferencePriceSeriesItem(premiumDiscountPrice, Now, SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithPremiumDiscount("LNG DES India"));
            }

            foreach (var assessedPrice in assessedPrices)
            {
                businessFlow.SaveSingleValueWithReferencePriceSeriesItem(assessedPrice, Now, SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice());
            }

            businessFlow.InitiatePublish(Now);

            var result = await businessFlow.Execute().GetRawResponse();

            result.MatchSnapshot();
        }

        [Fact]
        public async Task Create_DataPackage_And_Publish_Assessed_And_Derived_Price_Together_In_Same_ContentBlock()
        {
            var priceSeriesIds = ContentBlockQueryHelper.CreatePriceSeriesIds(
                [TestSeries.LNG_China_HM1, TestSeries.LNG_China_HM2], [TestSeries.LNG_China_MM1]);

            var businessFlow =
                  new PriceEntryBusinessFlow(ContentBlockId, priceSeriesIds, HttpClient)
                  .SaveContentBlockDefinition()
                  .SaveSingleValueWithReferencePriceSeriesItem(
                        TestSeries.LNG_China_HM1,
                        Now,
                        SingleValueWithReferencePriceSeriesItemBuilder.BuildDefaultValidSeriesItem())
                  .SaveSingleValueWithReferencePriceSeriesItem(
                        TestSeries.LNG_China_HM2,
                        Now,
                        SingleValueWithReferencePriceSeriesItemBuilder.BuildDefaultValidSeriesItem())
                  .PublishWithAdvanceWorkflow(Now);

            await businessFlow.Execute();

            var response = await contentBlockClient
                              .GetContentBlockWithPriceSeriesOnly(ContentBlockId, Now)
                              .GetRawResponse();

            response.MatchSnapshotWithSeriesItemId();
        }

        [Fact]
        public async Task Create_Data_Package_When_Use_Month_Plus_One_Period_And_Having_Reference_Price_Selected()
        {
            var indiaPriceSeriesIds = ContentBlockQueryHelper.CreatePriceSeriesIds(
                [TestSeries.LNG_India_HM1, TestSeries.LNG_India_HM2],
                [TestSeries.LNG_India_MM1]);

            var assessedPriceSeriesIds = ContentBlockQueryHelper.CreatePriceSeriesIds([TestSeries.LNG_Reload_Spain_Month1]);

            var indiaBusinessFlow = new PriceEntryBusinessFlow(ContentBlockId, indiaPriceSeriesIds, HttpClient)
                .SaveContentBlockDefinition()
                .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_India_HM1, Now, SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice())
                .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_India_HM2, Now, SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice())
                .PublishWithAdvanceWorkflow(Now);

            await indiaBusinessFlow.Execute();

            var spainFobBusinessFlow = new PriceEntryBusinessFlow("spainFobContentBlockId", assessedPriceSeriesIds, HttpClient)
                .SaveContentBlockDefinition()
                .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_Reload_Spain_Month1, Now, SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithPremiumDiscount("LNG DES India"))
                .InitiateSendForReview(Now);

            var result = await spainFobBusinessFlow.Execute().GetRawResponse();

            result.MatchSnapshot();
        }

        [Fact]
        public async Task Recalculate_Period_Label_For_Reference_Monthly_Series_When_Sending_For_Review()
        {
            var periodGeneratorServiceStub = GetService<PeriodGeneratorServiceStub>();

            try
            {
                // Arrange
                var mayDate = new DateTime(2025, 5, 30, 0, 0, 0, DateTimeKind.Utc);
                var marchDate = new DateTime(2025, 3, 31, 0, 0, 0, DateTimeKind.Utc);

                periodGeneratorServiceStub.ReplaceAbsolutePeriods(
                [
                    new AbsolutePeriod
                    {
                        Code = "M2505",
                        PeriodCode = "M",
                        Label = "May 2025",
                        FromDate = new DateOnly(2025, 05, 01),
                        UntilDate = new DateOnly(2025, 05, 31)
                    }
                ]);

                // Act
                var businessFlow = new PriceEntryBusinessFlow(
                        ContentBlockId,
                        [
                            TestSeries.Caustic_Soda_Italy_Contract
                        ],
                        HttpClient)
                   .SaveContentBlockDefinition()
                   .SaveRangePriceSeriesItem(
                        TestSeries.Caustic_Soda_Italy_Contract,
                        mayDate,
                        RangePriceSeriesItemBuilder.BuildDefaultValidSeriesItem(10, 20));
                await businessFlow.Execute();

                periodGeneratorServiceStub.ReplaceAbsolutePeriods(
                [
                    new AbsolutePeriod
                    {
                        Code = "M2503",
                        PeriodCode = "M",
                        Label = "March 2025",
                        FromDate = new DateOnly(2025, 03, 01),
                        UntilDate = new DateOnly(2025, 03, 31)
                    }
                ]);

                businessFlow.SaveRangePriceSeriesItem(
                        TestSeries.Caustic_Soda_Italy_Contract,
                        marchDate,
                        RangePriceSeriesItemBuilder.BuildDefaultValidSeriesItem(5, 5))
                   .PublishWithAdvanceWorkflow(marchDate);
                await businessFlow.Execute();

                // Check that the period_label is persisted in the database as "May 2025"
                var mayPriceSeriesBeforeUpdate = (await genericRepository.GetDocument<PriceSeriesItem>(
                                                      PriceSeriesItemRepository.CollectionName,
                                                      x => x.SeriesId == TestSeries.Caustic_Soda_Italy_Contract,
                                                      x => x.AssessedDateTime == mayDate)).First();

                mayPriceSeriesBeforeUpdate.PeriodLabel.Should().Be("May 2025");

                // Now initiate the send for review, which should recalculate the period label to "Apr 2024"
                // as that's the next available period
                periodGeneratorServiceStub.ReplaceAbsolutePeriods(
                [
                    new AbsolutePeriod
                    {
                        Code = "M2504",
                        PeriodCode = "M",
                        Label = "Apr 2025",
                        FromDate = new DateOnly(2025, 04, 30),
                        UntilDate = new DateOnly(2025, 04, 30)
                    }
                ]);

                businessFlow.InitiateSendForReview(mayDate);

                await businessFlow.Execute();

                // Assert
                var mayPriceSeriesAfterUpdate = (await genericRepository.GetDocument<PriceSeriesItem>(
                                                     PriceSeriesItemRepository.CollectionName,
                                                     x => x.SeriesId == TestSeries.Caustic_Soda_Italy_Contract,
                                                     x => x.AssessedDateTime == mayDate)).First();

                mayPriceSeriesAfterUpdate.PeriodLabel.Should().Be("Apr 2025");
            }
            finally
            {
                periodGeneratorServiceStub.ClearAbsolutePeriods();
            }
        }

        [Fact]
        public async Task Recalculate_Period_Label_For_Reference_Quarterly_Series_When_Sending_For_Review()
        {
            var periodGeneratorServiceStub = GetService<PeriodGeneratorServiceStub>();

            try
            {
                // Arrange
                var julyDate = new DateTime(2025, 7, 30, 0, 0, 0, DateTimeKind.Utc);
                var marchDate = new DateTime(2025, 3, 31, 0, 0, 0, DateTimeKind.Utc);

                periodGeneratorServiceStub.ReplaceAbsolutePeriods(
                [
                    new AbsolutePeriod
                    {
                        Code = "Q2503",
                        PeriodCode = "Q",
                        Label = "Q3 2025",
                        FromDate = new DateOnly(2025, 07, 01),
                        UntilDate = new DateOnly(2025, 09, 30)
                    }
                ]);

                // Act
                var businessFlow =
                new PriceEntryBusinessFlow(
                    ContentBlockId,
                    [
                         TestSeries.Melamine_Quarterly_Contract_Europe
                    ],
                    HttpClient)
                   .SaveContentBlockDefinition()
                   .SaveRangePriceSeriesItem(TestSeries.Melamine_Quarterly_Contract_Europe, julyDate, RangePriceSeriesItemBuilder.BuildDefaultValidSeriesItem(10, 20));

                await businessFlow.Execute();

                periodGeneratorServiceStub.ReplaceAbsolutePeriods(
                [
                    new AbsolutePeriod
                    {
                        Code = "Q2501",
                        PeriodCode = "Q",
                        Label = "Q1 2025",
                        FromDate = new DateOnly(2025, 01, 01),
                        UntilDate = new DateOnly(2025, 03, 31)
                    }
                ]);

                businessFlow.SaveRangePriceSeriesItem(TestSeries.Melamine_Quarterly_Contract_Europe, marchDate, RangePriceSeriesItemBuilder.BuildDefaultValidSeriesItem(5, 5))
                            .PublishWithAdvanceWorkflow(marchDate);

                await businessFlow.Execute();

                var mayPriceSeriesBeforeUpdate = (await genericRepository.GetDocument<PriceSeriesItem>(
                                                      PriceSeriesItemRepository.CollectionName,
                                                      x => x.SeriesId == TestSeries.Melamine_Quarterly_Contract_Europe,
                                                      x => x.AssessedDateTime == julyDate)).First();

                mayPriceSeriesBeforeUpdate.PeriodLabel.Should().Be("Q3 2025");

                periodGeneratorServiceStub.ReplaceAbsolutePeriods(
                [
                    new()
                    {
                        Code = "Q2502",
                        PeriodCode = "Q",
                        Label = "Q2 2025",
                        FromDate = new DateOnly(2025, 04, 01),
                        UntilDate = new DateOnly(2025, 06, 30)
                    }
                ]);

                businessFlow.InitiateSendForReview(julyDate);

                await businessFlow.Execute();

                // Assert
                var latestReadyForReviewPriceSeries = (await genericRepository.GetDocument<PriceSeriesItem>(
                                                           PriceSeriesItemRepository.CollectionName,
                                                           x => x.SeriesId == TestSeries.Melamine_Quarterly_Contract_Europe,
                                                           x => x.AssessedDateTime == julyDate)).First();

                latestReadyForReviewPriceSeries.PeriodLabel.Should().Be("Q2 2025");
            }
            finally
            {
                periodGeneratorServiceStub.ClearAbsolutePeriods();
            }
        }

        [Fact]
        public async Task Assign_SequenceId_From_ContentPackageGroup_When_Creating_DataPackage()
        {
            ICollection<string> priceSeriesIds = [TestSeries.LNG_China_HM1, TestSeries.LNG_China_HM2];

            var businessFlow =
            new PriceEntryBusinessFlow(
                ContentBlockId,
                priceSeriesIds,
                HttpClient)
               .SaveContentBlockDefinition()
               .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_China_HM1, Now, SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice())
               .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_China_HM2, Now, SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice(20))
               .PublishWithAdvanceWorkflow(Now);

            await businessFlow.Execute();

            var contentPackageGroup = (await genericRepository.GetDocument<ContentPackageGroup>(
                                          ContentPackageGroupRepository.CollectionName,
                                          x => x.PriceSeriesIds.Count == priceSeriesIds.Count &&
                                               x.PriceSeriesIds.All(priceSeriesId => priceSeriesIds.Contains(priceSeriesId)))).First();

            var dataPackage = (await genericRepository.GetDocument<DataPackage>(
                                    DataPackageRepository.CollectionName,
                                    x => x.ContentBlock.Id == ContentBlockId &&
                                         x.AssessedDateTime == Now)).First();

            dataPackage.SequenceId.Should().Be(contentPackageGroup.SequenceId);
        }
    }
}
