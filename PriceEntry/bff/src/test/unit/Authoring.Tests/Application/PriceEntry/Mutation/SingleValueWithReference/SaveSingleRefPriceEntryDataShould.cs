namespace Authoring.Tests.Application.PriceEntry.Mutation.SingleValueWithReference
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Authoring.Tests.Application.ContentBlock.Query.ContentBlockWithPriceSeries;
    using Authoring.Tests.Functional.Tests.Common.Infrastructure;

    using global::BusinessLayer.PriceEntry.DTOs.Input;
    using global::BusinessLayer.PriceEntry.Repositories.Models;
    using global::BusinessLayer.PriceEntry.ValueObjects;

    using global::Infrastructure.Services.PeriodGenerator;

    using Microsoft.Extensions.DependencyInjection;

    using Snapshooter;
    using Snapshooter.Xunit;

    using Test.Infrastructure.Authoring;
    using Test.Infrastructure.Authoring.BusinessFlows.PriceEntryBusinessFlow;
    using Test.Infrastructure.Authoring.BusinessFlows.PriceEntryBusinessFlow.PriceSeriesItemBuilders;
    using Test.Infrastructure.Authoring.GraphQLClients.PriceEntry;
    using Test.Infrastructure.Extensions;
    using Test.Infrastructure.Services;
    using Test.Infrastructure.Stubs;
    using Test.Infrastructure.TestData;

    using Xunit;

    [Collection(WebApplicationCollection.Name)]
    public class SaveSingleRefPriceEntryDataShould : WebApplicationTestBase
    {
        public static IEnumerable<object[]> DeltaCases = new[]
        {
            new object[]
            {
                "1 Before Rollover Date",
                new DateTime(2024, 9, 12, 0, 0, 0, DateTimeKind.Utc),
                new object[] { 10M, 40M, 30M, 49M, 25M },
                new DateTime(2024, 9, 13, 0, 0, 0, DateTimeKind.Utc),
                new object[] { 5M, 20M, 10M, 99M, 45M }
            },
            new object[]
            {
                "2 On Rollover Date",
                new DateTime(2024, 9, 13, 0, 0, 0, DateTimeKind.Utc),
                new object[] { 5M, 20M, 10M, 99M, 45M },
                new DateTime(2024, 9, 16, 0, 0, 0, DateTimeKind.Utc),
                new object[] { 25M, 60M, 87M, 55M, 35M }
            },
            new object[]
            {
                "3 After Rollover Date",
                new DateTime(2024, 9, 16, 0, 0, 0, DateTimeKind.Utc),
                new object[] { 25M, 60M, 87M, 55M, 35M },
                new DateTime(2024, 9, 17, 0, 0, 0, DateTimeKind.Utc),
                new object[] { 30M, 55M, 50M, 40M, 89M }
            },
            new object[]
            {
                "4 No Prices Entered On Assessed Date",
                new DateTime(2024, 9, 17, 0, 0, 0, DateTimeKind.Utc),
                new object[] { 30M, 55M, 50M, 40M, 89M },
                new DateTime(2024, 9, 18, 0, 0, 0, DateTimeKind.Utc),
                new object[] { }
            },
            new object[]
            {
                "5 No Prices Entered On Interim Date",
                new DateTime(2024, 9, 17, 0, 0, 0, DateTimeKind.Utc),
                new object[] { 30M, 55M, 50M, 40M, 89M },
                new DateTime(2024, 9, 19, 0, 0, 0, DateTimeKind.Utc),
                new object[] { }
            }
        };

        public static IEnumerable<object[]> LNGFOBReloadNWECases = new[]
        {
            new object[]
            {
                new DateTime(2024, 01, 10, 0, 0, 0, DateTimeKind.Utc),
                TestSeries.LNG_Reload_NWE_MM1,
                "past_date",
                100,
            },
            new object[]
            {
                new DateTime(2025, 06, 10, 0, 0, 0, DateTimeKind.Utc),
                TestSeries.LNG_Reload_NWE_20_40D,
                "future_date",
                200
            },
            new object[]
            {
                new DateTime(2025, 05, 12, 0, 0, 0, DateTimeKind.Utc),
                TestSeries.LNG_Reload_NWE_MM1,
                "termination_date_with_old_series",
                400
            },
            new object[]
            {
                new DateTime(2025, 05, 11, 0, 0, 0, DateTimeKind.Utc),
                TestSeries.LNG_Reload_NWE_MM1,
                "day_before_termination_date",
                500
            },
            new object[]
            {
                new DateTime(2025, 05, 13, 0, 0, 0, DateTimeKind.Utc),
                TestSeries.LNG_Reload_NWE_20_40D,
                "day_after_termination_date",
                600
            }
        };

        private const string ContentBlockId = "contentBlockId";

        private const string PriceSeriesId = TestSeries.LNG_Dubai_MM1;

        private static readonly DateTime Now = TestData.Now;

        private readonly TestClock testClock;

        private readonly ContentBlockGraphQLClient contentBlockClient;

        public SaveSingleRefPriceEntryDataShould(AuthoringBffApplicationFactory factory)
            : base(factory)
        {
            contentBlockClient = new ContentBlockGraphQLClient(GraphQLClient);
            testClock = factory.Services.GetRequiredService<TestClock>();
            testClock.Reset();
        }

        [Fact]
        public async Task Allow_To_Remove_All_Entered_Values()
        {
            var seriesItem = new SeriesItem
            {
                AssessmentMethod = AssessmentMethod.PremiumDiscount.Value,
                ReferenceMarketName = "TTF",
                PremiumDiscount = 5,
                Price = 15,
                DataUsed = "Bid/offer"
            };

            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, new List<string> { PriceSeriesId }, HttpClient)
               .SaveContentBlockDefinition()
               .SaveSingleValueWithReferencePriceSeriesItem(PriceSeriesId, Now, seriesItem)
               .SaveSingleValueWithReferencePriceSeriesItem(PriceSeriesId, Now, new SeriesItem());
            await businessFlow.Execute();

            var response = await contentBlockClient
                              .GetContentBlockWithPriceSeriesOnly(ContentBlockId, Now)
                              .GetRawResponse();

            response.MatchSnapshotWithSeriesItemId();
        }

        [Fact]
        public async Task Allow_To_Remove_Price_Value_For_Assessed_Prices()
        {
            var seriesItem = new SeriesItem { AssessmentMethod = AssessmentMethod.Assessed.Value, Price = 10 };

            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, new List<string> { PriceSeriesId }, HttpClient)
               .SaveContentBlockDefinition()
               .SaveSingleValueWithReferencePriceSeriesItem(PriceSeriesId, Now, seriesItem);
            await businessFlow.Execute();

            seriesItem.Price = null;

            businessFlow.SaveSingleValueWithReferencePriceSeriesItem(PriceSeriesId, Now, seriesItem);
            await businessFlow.Execute();

            var response = await contentBlockClient
                              .GetContentBlockWithPriceSeriesOnly(ContentBlockId, Now)
                              .GetRawResponse();

            response.MatchSnapshotWithSeriesItemId();
        }

        [Fact]
        public async Task Calculate_Price_Value_For_Positive_PremiumDiscount()
        {
            var seriesItem = new SeriesItem
            {
                AssessmentMethod = AssessmentMethod.PremiumDiscount.Value,
                ReferenceMarketName = "TTF",
                PremiumDiscount = 1.23m,
                DataUsed = "Spread"
            };

            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, new List<string> { PriceSeriesId }, HttpClient)
               .SaveContentBlockDefinition()
               .SaveSingleValueWithReferencePriceSeriesItem(PriceSeriesId, Now, seriesItem);
            await businessFlow.Execute();

            var response = await contentBlockClient
                              .GetContentBlockWithPriceSeriesOnly(ContentBlockId, Now)
                              .GetRawResponse();

            response.MatchSnapshotWithSeriesItemId();
        }

        [Fact]
        public async Task Calculate_Price_Value_For_Negative_PremiumDiscount()
        {
            var seriesItem = new SeriesItem
            {
                AssessmentMethod = AssessmentMethod.PremiumDiscount.Value,
                ReferenceMarketName = "TTF",
                PremiumDiscount = -0.99m,
                DataUsed = "Spread"
            };

            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, new List<string> { PriceSeriesId }, HttpClient)
               .SaveContentBlockDefinition()
               .SaveSingleValueWithReferencePriceSeriesItem(PriceSeriesId, Now, seriesItem);
            await businessFlow.Execute();

            var response = await contentBlockClient
                              .GetContentBlockWithPriceSeriesOnly(ContentBlockId, Now)
                              .GetRawResponse();

            response.MatchSnapshotWithSeriesItemId();
        }

        [Fact]
        public async Task Calculate_The_Price_Delta_Based_On_Previous_Price_For_PremiumDiscount()
        {
            // arrange
            var yesterday = Now.AddDays(-1);

            var seriesItem = new SeriesItem
            {
                AssessmentMethod = AssessmentMethod.PremiumDiscount.Value,
                ReferenceMarketName = "TTF",
                PremiumDiscount = 0,
                DataUsed = "Bid/offer"
            };

            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, new List<string> { PriceSeriesId }, HttpClient)
                .SaveContentBlockDefinition()
                .SaveSingleValueWithReferencePriceSeriesItem(PriceSeriesId, yesterday, seriesItem)
                .InitiatePublish(yesterday)
                .AcknowledgePublished(yesterday);
            await businessFlow.Execute();

            // act
            var today = yesterday.AddDays(1);
            seriesItem.PremiumDiscount = 5;

            businessFlow.SaveSingleValueWithReferencePriceSeriesItem(PriceSeriesId, today, seriesItem);
            await businessFlow.Execute();

            // assert
            var response = await contentBlockClient
                              .GetContentBlockWithPriceSeriesOnly(ContentBlockId, today)
                              .GetRawResponse();

            response.MatchSnapshotWithSeriesItemId();
        }

        [Fact]
        public async Task Reset_Price_Fields_When_Assessment_Method_Changes()
        {
            var seriesItem = new SeriesItem
            {
                AssessmentMethod = AssessmentMethod.PremiumDiscount.Value,
                ReferenceMarketName = "TTF",
                PremiumDiscount = 5,
                DataUsed = "Bid/offer"
            };

            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, new List<string> { PriceSeriesId }, HttpClient)
                .SaveContentBlockDefinition()
                .SaveSingleValueWithReferencePriceSeriesItem(PriceSeriesId, Now, seriesItem);
            await businessFlow.Execute();

            seriesItem.AssessmentMethod = AssessmentMethod.Assessed.Value;

            businessFlow.SaveSingleValueWithReferencePriceSeriesItem(PriceSeriesId, Now, seriesItem);
            await businessFlow.Execute();

            var response = await contentBlockClient
                              .GetContentBlockWithPriceSeriesOnly(ContentBlockId, Now)
                              .GetRawResponse();

            response.MatchSnapshotWithSeriesItemId();
        }

        [Fact]
        public async Task Save_The_Price_Entry_Data()
        {
            var seriesItem = new SeriesItem
            {
                AssessmentMethod = AssessmentMethod.PremiumDiscount.Value,
                ReferenceMarketName = "TTF",
                PremiumDiscount = 5,
                DataUsed = "Bid/offer"
            };

            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, new List<string> { PriceSeriesId }, HttpClient)
               .SaveContentBlockDefinition()
               .SaveSingleValueWithReferencePriceSeriesItem(PriceSeriesId, Now, seriesItem);
            await businessFlow.Execute();

            var response = await contentBlockClient
                              .GetContentBlockWithPriceSeriesOnly(ContentBlockId, Now)
                              .GetRawResponse();

            response.MatchSnapshotWithSeriesItemId();
        }

        [Fact]
        public async Task Save_The_Price_Entry_Data_With_Absolute_Period()
        {
            var seriesItem = new SeriesItem
            {
                AssessmentMethod = AssessmentMethod.PremiumDiscount.Value,
                ReferenceMarketName = "TTF",
                PremiumDiscount = 5,
                DataUsed = "Bid/offer"
            };

            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, new List<string> { PriceSeriesId }, HttpClient)
               .SaveContentBlockDefinition()
               .SaveSingleValueWithReferencePriceSeriesItem(PriceSeriesId, Now, seriesItem);

            var response = await businessFlow.Execute().GetRawResponse();

            response.MatchSnapshotForPriceEntryUpdate();
        }

        [Fact]
        public async Task Verify_Data_Updated()
        {
            var seriesItem = new SeriesItem
            {
                AssessmentMethod = AssessmentMethod.PremiumDiscount.Value,
                ReferenceMarketName = "TTF",
                PremiumDiscount = 5,
                DataUsed = "Bid/offer"
            };

            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, new List<string> { PriceSeriesId }, HttpClient)
               .SaveContentBlockDefinition()
               .SaveSingleValueWithReferencePriceSeriesItem(PriceSeriesId, Now, seriesItem);
            await businessFlow.Execute();

            seriesItem.PremiumDiscount = 10;

            businessFlow.SaveSingleValueWithReferencePriceSeriesItem(PriceSeriesId, Now, seriesItem);
            await businessFlow.Execute();

            var response = await contentBlockClient
                              .GetContentBlockWithPriceSeriesOnly(ContentBlockId, Now)
                              .GetRawResponse();

            response.MatchSnapshotWithSeriesItemId();
        }

        [Theory]
        [InlineData("LNG DES Argentina", TestSeries.LNG_Argentina_MM1)]
        [InlineData("LNG FOB Reload NWE", TestSeries.LNG_Reload_NWE_MM1)]
        public async Task Save_Price_Value_When_Reference_Market_Is_Argentina_NWE(string referenceMarketName, string seriesId)
        {
            var referencePriceSeriesItem = new SeriesItem
            {
                AssessmentMethod = AssessmentMethod.Assessed.Value,
                Price = 20,
                DataUsed = "Bid/offer"
            };

            var referencePriceBusinessFlow = new PriceEntryBusinessFlow(ContentBlockId, new List<string> { seriesId }, HttpClient)
               .SaveSingleValueWithReferencePriceSeriesItem(seriesId, Now, referencePriceSeriesItem);
            await referencePriceBusinessFlow.Execute();

            // Act
            var assessedSeriesItem = new SeriesItem
            {
                AssessmentMethod = AssessmentMethod.PremiumDiscount.Value,
                ReferenceMarketName = referenceMarketName,
                PremiumDiscount = 10,
                DataUsed = "Bid/offer"
            };

            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, new List<string> { PriceSeriesId }, HttpClient)
               .SaveContentBlockDefinition()
               .SaveSingleValueWithReferencePriceSeriesItem(PriceSeriesId, Now, assessedSeriesItem);
            await businessFlow.Execute();

            var response = await contentBlockClient
                              .GetContentBlockWithPriceSeriesOnly(ContentBlockId, Now)
                              .GetRawResponse();

            response.MatchSnapshotWithSeriesItemId(SnapshotNameExtension.Create(referenceMarketName));
        }

        [Theory]
        [InlineData("LNG EAX Index", TestSeries.LNG_EAX_Index_MM1)]
        [InlineData("LNG DES India", TestSeries.LNG_India_MM1)]
        public async Task Save_Price_Value_When_Reference_Market_Is_EAX_India(string referenceMarketName, string seriesId)
        {
            // Arrange
            var referencePriceSeriesItem = new SeriesItem
            {
                AssessmentMethod = AssessmentMethod.Assessed.Value,
                Price = 20,
                DataUsed = "Bid/offer"
            };

            var referencePriceBusinessFlow = new PriceEntryBusinessFlow(string.Empty, new List<string> { seriesId }, HttpClient)
               .SaveSingleValuePriceSeriesItem(seriesId, Now, referencePriceSeriesItem);
            await referencePriceBusinessFlow.Execute();

            // Act
            var assessedSeriesItem = new SeriesItem
            {
                AssessmentMethod = AssessmentMethod.PremiumDiscount.Value,
                ReferenceMarketName = referenceMarketName,
                PremiumDiscount = 10,
                DataUsed = "Bid/offer"
            };

            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, new List<string> { PriceSeriesId }, HttpClient)
                .SaveContentBlockDefinition()
                .SaveSingleValueWithReferencePriceSeriesItem(PriceSeriesId, Now, assessedSeriesItem);
            await businessFlow.Execute();

            var response = await contentBlockClient
                              .GetContentBlockWithPriceSeriesOnly(ContentBlockId, Now)
                              .GetRawResponse();

            response.MatchSnapshotWithSeriesItemId(SnapshotNameExtension.Create(referenceMarketName));
        }

        [Fact]
        public async Task Save_Price_Delta_Type()
        {
            var seriesItem = new SeriesItem
            {
                AssessmentMethod = AssessmentMethod.PremiumDiscount.Value,
                ReferenceMarketName = "TTF",
                PremiumDiscount = 5,
                DataUsed = "Bid/offer"
            };

            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, new List<string> { PriceSeriesId }, HttpClient)
               .SaveContentBlockDefinition()
               .SaveSingleValueWithReferencePriceSeriesItem(PriceSeriesId, Now, seriesItem);
            var response = await businessFlow.Execute().GetRawResponse();

            response.MatchSnapshotForPriceEntryUpdate();
        }

        [Fact]
        public async Task Calculate_Full_Month_Derived_Price_Based_On_Half_Month_Average()
        {
            // arrange
            var fullMonthContentBlockId = "FullMonthContentBlockId";

            var fullMonthBusinessFlow = new PriceEntryBusinessFlow(
                    fullMonthContentBlockId,
                    new List<string> { TestSeries.LNG_China_MM1 },
                    HttpClient)
               .SaveContentBlockDefinition();
            await fullMonthBusinessFlow.Execute();

            var halfMonth1SeriesItem = new SeriesItem { AssessmentMethod = AssessmentMethod.Assessed.Value, Price = 100, DataUsed = "Bid/offer" };
            var halfMonth2SeriesItem = new SeriesItem { AssessmentMethod = AssessmentMethod.Assessed.Value, Price = 150, DataUsed = "Bid/offer" };
            var halfMonth3SeriesItem = new SeriesItem { AssessmentMethod = AssessmentMethod.Assessed.Value, Price = 250, DataUsed = "Bid/offer" };

            var halfMonthBusinessFlow = new PriceEntryBusinessFlow(
                    ContentBlockId,
                    new List<string> { TestSeries.LNG_China_HM1, TestSeries.LNG_China_HM2, TestSeries.LNG_China_HM3 },
                    HttpClient)
               .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_China_HM1, Now, halfMonth1SeriesItem)
               .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_China_HM2, Now, halfMonth2SeriesItem)
               .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_China_HM3, Now, halfMonth3SeriesItem);
            await halfMonthBusinessFlow.Execute();

            // act
            var response = await contentBlockClient
                              .GetContentBlockWithPriceSeriesOnly(fullMonthContentBlockId, Now)
                              .GetRawResponse();

            // assert
            response.MatchSnapshotWithSeriesItemId();
        }

        [Fact]
        public async Task Calculate_EAX_Derived_Price_Based_On_Half_Month_Average()
        {
            // arrange
            var eaxContentBlockId = "EAXContentBlockId";

            var fullMonthBusinessFlow = new PriceEntryBusinessFlow(
                    eaxContentBlockId,
                    new List<string> { TestSeries.LNG_EAX_Index_HM2 },
                    HttpClient)
               .SaveContentBlockDefinition();
            await fullMonthBusinessFlow.Execute();

            var halfMonthChinaSeriesItem = new SeriesItem { AssessmentMethod = AssessmentMethod.Assessed.Value, Price = 100, DataUsed = "Bid/offer" };
            var halfMonthJapanSeriesItem = new SeriesItem { AssessmentMethod = AssessmentMethod.Assessed.Value, Price = 150, DataUsed = "Bid/offer" };
            var halfMonthSouthKoreaSeriesItem = new SeriesItem { AssessmentMethod = AssessmentMethod.Assessed.Value, Price = 250, DataUsed = "Bid/offer" };
            var halfMonthTaiwanSeriesItem = new SeriesItem { AssessmentMethod = AssessmentMethod.Assessed.Value, Price = 250, DataUsed = "Bid/offer" };

            var halfMonthBusinessFlow = new PriceEntryBusinessFlow(
                    ContentBlockId,
                    new List<string> { TestSeries.LNG_China_HM1, TestSeries.LNG_Japan_HM1, TestSeries.LNG_South_Korea_HM1, TestSeries.LNG_Taiwan_HM1 },
                    HttpClient)
               .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_China_HM1, Now, halfMonthChinaSeriesItem)
               .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_Japan_HM1, Now, halfMonthJapanSeriesItem)
               .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_South_Korea_HM1, Now, halfMonthSouthKoreaSeriesItem)
               .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_Taiwan_HM1, Now, halfMonthTaiwanSeriesItem);
            await halfMonthBusinessFlow.Execute();

            // act
            var response = await contentBlockClient
                              .GetContentBlockWithPriceSeriesOnly(eaxContentBlockId, Now)
                              .GetRawResponse();

            // assert
            response.MatchSnapshotWithSeriesItemId();
        }

        [Fact]
        public async Task Update_Lng_Reference_Price_When_Price_Series_Item_Is_Saved()
        {
            // Arrange
            var halfMonthIndiaSeriesItem = new SeriesItem
            {
                AssessmentMethod = AssessmentMethod.PremiumDiscount.Value,
                PremiumDiscount = 100,
                ReferenceMarketName = "LNG EAX Index",
                DataUsed = "Bid/offer"
            };

            var indiaHM1ContentBlockId = "IndiaHM1ContentBlockId";

            var halfMonthIndiaSeriesWithReferencePriceFlow = new PriceEntryBusinessFlow(
                   indiaHM1ContentBlockId,
                   new List<string> { TestSeries.LNG_India_HM1 },
                   HttpClient)
              .SaveContentBlockDefinition()
              .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_India_HM1, Now, halfMonthIndiaSeriesItem);

            await halfMonthIndiaSeriesWithReferencePriceFlow.Execute();

            var halfMonthEaxSeriesWithReferencePriceFlow = new PriceEntryBusinessFlow(
                    "EAXContentBlockId",
                    new List<string> { TestSeries.LNG_EAX_Index_HM2 },
                    HttpClient)
               .SaveContentBlockDefinition();

            await halfMonthEaxSeriesWithReferencePriceFlow.Execute();

            var halfMonthChinaSeriesItem = new SeriesItem { AssessmentMethod = AssessmentMethod.Assessed.Value, Price = 100, DataUsed = "Bid/offer" };
            var halfMonthJapanSeriesItem = new SeriesItem { AssessmentMethod = AssessmentMethod.Assessed.Value, Price = 200, DataUsed = "Bid/offer" };
            var halfMonthSouthKoreaSeriesItem = new SeriesItem { AssessmentMethod = AssessmentMethod.Assessed.Value, Price = 300, DataUsed = "Bid/offer" };
            var halfMonthTaiwanSeriesItem = new SeriesItem { AssessmentMethod = AssessmentMethod.Assessed.Value, Price = 400, DataUsed = "Bid/offer" };

            var halfMonthBusinessFlow = new PriceEntryBusinessFlow(
                    ContentBlockId,
                    new List<string> { TestSeries.LNG_China_HM1, TestSeries.LNG_Japan_HM1, TestSeries.LNG_South_Korea_HM1, TestSeries.LNG_Taiwan_HM1 },
                    HttpClient)
               .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_China_HM1, Now, halfMonthChinaSeriesItem)
               .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_Japan_HM1, Now, halfMonthJapanSeriesItem)
               .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_South_Korea_HM1, Now, halfMonthSouthKoreaSeriesItem)
               .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_Taiwan_HM1, Now, halfMonthTaiwanSeriesItem);

            await halfMonthBusinessFlow.Execute();

            // act
            var response = await contentBlockClient
                              .GetContentBlockWithPriceSeriesOnly(indiaHM1ContentBlockId, Now)
                              .GetRawResponse();

            // assert
            response.MatchSnapshotWithSeriesItemId();
        }

        [Fact]
        public async Task Use_Month_Plus_One_Reference_Price_When_Fulfilment_Period_Is_1545D()
        {
            // Arrange
            var assessedDateTime = new DateTime(2025, 04, 13, 0, 0, 0, DateTimeKind.Utc);
            var seriesItem = new SeriesItem
            {
                AssessmentMethod = AssessmentMethod.PremiumDiscount.Value,
                ReferenceMarketName = "TTF",
                PremiumDiscount = 5,
                DataUsed = "Bid/offer"
            };

            var periodGeneratorServiceStub = GetService<PeriodGeneratorServiceStub>();
            periodGeneratorServiceStub.ReplaceAbsolutePeriods(
            [
                new AbsolutePeriod
                {
                    Code = "M2405",
                    PeriodCode = "M1",
                    Label = "May 2025",
                    FromDate = new DateOnly(2025, 05, 01),
                    UntilDate = new DateOnly(2025, 05, 31)
                }
            ]);

            var gasPriceSeriesItem = new GasPriceSeriesItem
            {
                Id = "267-M1-20250413",
                SpecificationName = "ICIS Heren® Natural Gas EoD Assessment - TTF Conv. USD/MMBtu",
                MarketCode = "TTF",
                AssessedDateTime = assessedDateTime,
                Status = "Published",
                PeriodLabel = "May '25",
                FulfilmentFromDate = new DateTime(2025, 05, 01, 0, 0, 0, DateTimeKind.Utc),
                FulfilmentUntilDate = new DateTime(2025, 05, 31, 0, 0, 0, DateTimeKind.Utc),
                Mid = 8.675M,
            };

            var gasPriceSeriesItemTestService = GetService<GasPriceSeriesItemTestService>();
            await gasPriceSeriesItemTestService.SaveGasPrice(gasPriceSeriesItem);

            // Act
            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, new List<string> { TestSeries.LNG_Reload_Spain_Month1 }, HttpClient)
               .SaveContentBlockDefinition()
               .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_Reload_Spain_Month1, assessedDateTime, seriesItem);

            await businessFlow.Execute();

            periodGeneratorServiceStub.ClearAbsolutePeriods();

            var response = await contentBlockClient
                              .GetContentBlockWithPriceSeriesOnly(ContentBlockId, assessedDateTime)
                              .GetRawResponse();

            // Assert
            response.MatchSnapshotWithSeriesItemId();
        }

        [Fact]
        public async Task Use_Month_Plus_One_Reference_Price_When_Fulfilment_Period_Is_2040D()
        {
            // Arrange
            var assessedDateTime = new DateTime(2025, 05, 18, 0, 0, 0, DateTimeKind.Utc);
            var seriesItem = new SeriesItem
            {
                AssessmentMethod = AssessmentMethod.PremiumDiscount.Value,
                ReferenceMarketName = "TTF",
                PremiumDiscount = 5,
                DataUsed = "Bid/offer"
            };

            var periodGeneratorServiceStub = GetService<PeriodGeneratorServiceStub>();
            periodGeneratorServiceStub.ReplaceAbsolutePeriods(
            [
                new AbsolutePeriod
                {
                    Code = "M2406",
                    PeriodCode = "M1",
                    Label = "June 2025",
                    FromDate = new DateOnly(2025, 06, 01),
                    UntilDate = new DateOnly(2025, 06, 30)
                }
            ]);

            var gasPriceSeriesItem = new GasPriceSeriesItem
            {
                Id = "267-M1-20250513",
                SpecificationName = "ICIS Heren® Natural Gas EoD Assessment - TTF Conv. USD/MMBtu",
                MarketCode = "TTF",
                AssessedDateTime = assessedDateTime,
                Status = "Published",
                PeriodLabel = "June '25",
                FulfilmentFromDate = new DateTime(2025, 06, 01, 0, 0, 0, DateTimeKind.Utc),
                FulfilmentUntilDate = new DateTime(2025, 06, 30, 0, 0, 0, DateTimeKind.Utc),
                Mid = 9.675M,
            };

            var gasPriceSeriesItemTestService = GetService<GasPriceSeriesItemTestService>();
            await gasPriceSeriesItemTestService.SaveGasPrice(gasPriceSeriesItem);

            // Act
            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, new List<string> { TestSeries.LNG_Reload_NWE_20_40D }, HttpClient)
               .SaveContentBlockDefinition()
               .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_Reload_NWE_20_40D, assessedDateTime, seriesItem);

            await businessFlow.Execute();

            periodGeneratorServiceStub.ClearAbsolutePeriods();

            var response = await contentBlockClient
                              .GetContentBlockWithPriceSeriesOnly(ContentBlockId, assessedDateTime)
                              .GetRawResponse();

            // Assert
            response.MatchSnapshotWithSeriesItemId();
        }

        [Theory]
        [MemberData(nameof(DeltaCases))]
        public async Task Calculate_Delta_Based_On_Correct_Series(
            string description,
            DateTime previousDate,
            object[] previousPrices,
            DateTime assessedDateTime,
            object[] assessedPrices)
        {
            var absolutePeriods = GetAbsolutePeriodsForDeltaTests(previousDate);

            var periodGeneratorServiceStub = GetService<PeriodGeneratorServiceStub>();
            periodGeneratorServiceStub.ReplaceAbsolutePeriods(absolutePeriods);

            // Arrange
            var halfMonthContentBlockId = "HalfMonthContentBlock";
            var chinaHalfMonthSeriesIds = new List<string>
            {
                TestSeries.LNG_China_HM1,
                TestSeries.LNG_China_HM2,
                TestSeries.LNG_China_HM3,
                TestSeries.LNG_China_HM4,
                TestSeries.LNG_China_HM5
            };

            var monthlyDerivedContentBlockId = "MonthlyDerivedContentBlock";
            var chinaMonthlyDerivedSeriesIds = new List<string>
            {
                TestSeries.LNG_China_MM1,
                TestSeries.LNG_China_MM2
            };

            // Create the Half Month Content Block Definition
            var contentBlockHalfMonthDefinitionBusinessFlow = new PriceEntryBusinessFlow(halfMonthContentBlockId, chinaHalfMonthSeriesIds, HttpClient)
               .SaveContentBlockDefinition();
            await contentBlockHalfMonthDefinitionBusinessFlow.Execute();

            var contentBlockMonthlyDerivedDefinitionBusinessFlow = new PriceEntryBusinessFlow(monthlyDerivedContentBlockId, chinaMonthlyDerivedSeriesIds, HttpClient)
               .SaveContentBlockDefinition();
            await contentBlockMonthlyDerivedDefinitionBusinessFlow.Execute();

            // Save the Half Month prices and publish
            var halfMonthBusinessFlow = new PriceEntryBusinessFlow(
                halfMonthContentBlockId,
                chinaHalfMonthSeriesIds,
                HttpClient)
               .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_China_HM1, previousDate, GetSeriesItemForDeltaTests((decimal)previousPrices[0]))
               .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_China_HM2, previousDate, GetSeriesItemForDeltaTests((decimal)previousPrices[1]))
               .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_China_HM3, previousDate, GetSeriesItemForDeltaTests((decimal)previousPrices[2]))
               .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_China_HM4, previousDate, GetSeriesItemForDeltaTests((decimal)previousPrices[3]))
               .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_China_HM5, previousDate, GetSeriesItemForDeltaTests((decimal)previousPrices[4]))
               .InitiatePublish(previousDate)
               .AcknowledgePublished(previousDate);

            await halfMonthBusinessFlow.Execute();

            // Publish the Monthly Derived Content Block
            await contentBlockMonthlyDerivedDefinitionBusinessFlow
               .InitiatePublish(previousDate)
               .AcknowledgePublished(previousDate)
               .Execute();

            // Act
            // Get the absolutePeriods for the assessed date
            absolutePeriods = GetAbsolutePeriodsForDeltaTests(assessedDateTime);
            periodGeneratorServiceStub.ReplaceAbsolutePeriods(absolutePeriods);

            // Save the Half Month prices
            if (assessedPrices.Any())
            {
                halfMonthBusinessFlow
                   .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_China_HM1, assessedDateTime, GetSeriesItemForDeltaTests((decimal)assessedPrices[0]))
                   .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_China_HM2, assessedDateTime, GetSeriesItemForDeltaTests((decimal)assessedPrices[1]))
                   .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_China_HM3, assessedDateTime, GetSeriesItemForDeltaTests((decimal)assessedPrices[2]))
                   .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_China_HM4, assessedDateTime, GetSeriesItemForDeltaTests((decimal)assessedPrices[3]))
                   .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_China_HM5, assessedDateTime, GetSeriesItemForDeltaTests((decimal)assessedPrices[4]));
                await halfMonthBusinessFlow.Execute();
            }

            var halfMonthResponse = await contentBlockClient.GetContentBlockWithPriceSeriesOnly(
                                            halfMonthContentBlockId,
                                            assessedDateTime)
                                       .GetRawResponse();

            var fullMonthResponse = await contentBlockClient.GetContentBlockWithPriceSeriesOnly(
                                            monthlyDerivedContentBlockId,
                                            assessedDateTime)
                                       .GetRawResponse();

            // Reset the absolutePeriods for other tests
            periodGeneratorServiceStub.ClearAbsolutePeriods();

            // Assert
            var halfMonthDescription = string.Concat("Half_Month_", description.Replace(" ", "_"));
            var fullMonthDescription = string.Concat("Full_Month_", description.Replace(" ", "_"));
            if (assessedPrices.Any() && previousPrices.Any())
            {
                halfMonthResponse.MatchSnapshotWithSeriesItemId(SnapshotNameExtension.Create(halfMonthDescription));
                fullMonthResponse.MatchSnapshotWithSeriesItemId(SnapshotNameExtension.Create(fullMonthDescription));
            }
            else
            {
                halfMonthResponse.MatchSnapshot(SnapshotNameExtension.Create(halfMonthDescription));
                fullMonthResponse.MatchSnapshot(SnapshotNameExtension.Create(fullMonthDescription));
            }
        }

        [Fact]
        public async Task Only_Calculate_Impacted_Full_Month_Derived_Price_Based_On_Half_Month_Average()
        {
            // arrange
            var halfMonthContentBlockId = "HalfMonthContentBlockId2";
            var fullMonthsContentBlockId = "FullMonthContentBlockId2";

            var fullMonthBusinessFlow = new PriceEntryBusinessFlow(
                    fullMonthsContentBlockId,
                    new List<string> { TestSeries.LNG_China_MM1, TestSeries.LNG_China_MM2 },
                    HttpClient)
               .SaveContentBlockDefinition();
            await fullMonthBusinessFlow.Execute();

            var halfMonth1SeriesItem = new SeriesItem { AssessmentMethod = AssessmentMethod.Assessed.Value, Price = 100, DataUsed = "Bid/offer" };
            var halfMonth2SeriesItem = new SeriesItem { AssessmentMethod = AssessmentMethod.Assessed.Value, Price = 150, DataUsed = "Bid/offer" };

            var halfMonthBusinessFlow = new PriceEntryBusinessFlow(
                    halfMonthContentBlockId,
                    new List<string> { TestSeries.LNG_China_HM1, TestSeries.LNG_China_HM2 },
                    HttpClient)
               .SaveContentBlockDefinition()
               .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_China_HM1, Now, halfMonth1SeriesItem)
               .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_China_HM2, Now, halfMonth2SeriesItem);
            await halfMonthBusinessFlow.Execute();

            // act
            var response = await contentBlockClient
                              .GetContentBlockWithPriceSeriesOnly(fullMonthsContentBlockId, Now)
                              .GetRawResponse();

            // assert
            response.MatchSnapshotWithoutSeriesItemId();
        }

        [Theory]
        [MemberData(nameof(LNGFOBReloadNWECases))]
        public async Task Save_Price_Value_When_Reference_Market_Is_LNG_FOB_Reload_NWE(
            DateTime assessedDateTime,
            string referenceSeries,
            string testCaseName,
            int price)
        {
            testClock.SetUtcNow(assessedDateTime);

            var priceSeriesIds = ContentBlockQueryHelper.CreatePriceSeriesIds([referenceSeries], [TestSeries.LNG_Belgium_DES_M1]);

            var businessFlow = new PriceEntryBusinessFlow(ContentBlockId, priceSeriesIds, HttpClient)
               .SaveContentBlockDefinition()
               .SaveSingleValueWithReferencePriceSeriesItem(
                            referenceSeries,
                            assessedDateTime,
                            SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice(price))
               .SaveSingleValueWithReferencePriceSeriesItem(
                           TestSeries.LNG_Belgium_DES_M1,
                           assessedDateTime,
                           SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithPremiumDiscount("LNG FOB Reload NWE"));

            await businessFlow.Execute();

            var response = await contentBlockClient
                              .GetContentBlockWithPriceSeriesOnly(ContentBlockId, assessedDateTime)
                              .GetRawResponse();

            response.MatchSnapshotWithoutSeriesItemId(SnapshotNameExtension.Create(testCaseName));
        }

        [Fact]
        public async Task Save_Price_Value_When_Reference_Market_Is_LNG_FOB_Reload_NWE_In_Different_Content_Block()
        {
            var assessedDateTime = new DateTime(2025, 05, 14, 0, 0, 0, DateTimeKind.Utc);

            var periodGeneratorServiceStub = GetService<PeriodGeneratorServiceStub>();
            periodGeneratorServiceStub.ReplaceAbsolutePeriods(new List<AbsolutePeriod>
            {
                new AbsolutePeriod
                     {
                        Code = "M2405",
                        PeriodCode = "M1",
                        Label = "May 2025",
                        FromDate = new DateOnly(2025, 05, 01),
                        UntilDate = new DateOnly(2025, 05, 31)
                     },
            });

            var assessedPricesId = ContentBlockQueryHelper.CreatePriceSeriesIds([TestSeries.LNG_Reload_Spain_20_40D]);
            var referencePricesId = ContentBlockQueryHelper.CreatePriceSeriesIds([TestSeries.LNG_Reload_NWE_20_40D]);

            var referencePriceBusinessFlow = new PriceEntryBusinessFlow("referencePricesContentBlockId", referencePricesId, HttpClient)
               .SaveContentBlockDefinition()
               .SaveSingleValueWithReferencePriceSeriesItem(
                            TestSeries.LNG_Reload_NWE_20_40D,
                            assessedDateTime,
                            SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice(11))
               .PublishWithAdvanceWorkflow(assessedDateTime);

            await referencePriceBusinessFlow.Execute();

            var spainFobBusinessFlow = new PriceEntryBusinessFlow(ContentBlockId, assessedPricesId, HttpClient)
                .SaveContentBlockDefinition()
                .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_Reload_Spain_20_40D, assessedDateTime, SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithPremiumDiscount("LNG FOB Reload NWE"))
                .PublishWithAdvanceWorkflow(assessedDateTime);

            await spainFobBusinessFlow.Execute();

            var response = await contentBlockClient
                              .GetContentBlockWithPriceSeriesOnly(ContentBlockId, assessedDateTime)
                              .GetRawResponse();

            periodGeneratorServiceStub.ClearAbsolutePeriods();

            response.MatchSnapshotWithoutSeriesItemId();
        }

        private List<AbsolutePeriod> GetAbsolutePeriodsForDeltaTests(DateTime referenceDate)
        {
            if (referenceDate.Day < 16)
            {
                return new List<AbsolutePeriod>
                {
                    new()
                    {
                        Code = "1H2410",
                        PeriodCode = "HM2",
                        Label = "1H October 2024",
                        FromDate = new DateOnly(2024, 10, 01),
                        UntilDate = new DateOnly(2024, 10, 15)
                    },
                    new()
                    {
                        Code = "2H2410",
                        PeriodCode = "HM3",
                        Label = "2H October 2024",
                        FromDate = new DateOnly(2024, 10, 16),
                        UntilDate = new DateOnly(2024, 10, 31)
                    },
                    new()
                    {
                        Code = "1H2411",
                        PeriodCode = "HM4",
                        Label = "1H November 2024",
                        FromDate = new DateOnly(2024, 11, 01),
                        UntilDate = new DateOnly(2024, 11, 15)
                    },
                    new()
                    {
                        Code = "2H2411",
                        PeriodCode = "HM5",
                        Label = "2H November 2024",
                        FromDate = new DateOnly(2024, 11, 16),
                        UntilDate = new DateOnly(2024, 11, 30)
                    },
                    new()
                    {
                        Code = "1H2412",
                        PeriodCode = "HM6",
                        Label = "1H December 2024",
                        FromDate = new DateOnly(2024, 12, 01),
                        UntilDate = new DateOnly(2024, 12, 15)
                    },
                    new()
                    {
                        Code = "MM2410",
                        PeriodCode = "MM1",
                        Label = "October 2024",
                        FromDate = new DateOnly(2024, 10, 01),
                        UntilDate = new DateOnly(2024, 10, 31)
                    },
                    new()
                    {
                        Code = "MM2411",
                        PeriodCode = "MM2",
                        Label = "November 2024",
                        FromDate = new DateOnly(2024, 11, 01),
                        UntilDate = new DateOnly(2024, 11, 30)
                    }
                };
            }
            else
            {
                return new List<AbsolutePeriod>
                {
                    new()
                    {
                        Code = "2H2410",
                        PeriodCode = "HM2",
                        Label = "2H October 2024",
                        FromDate = new DateOnly(2024, 10, 16),
                        UntilDate = new DateOnly(2024, 10, 31)
                    },
                    new()
                    {
                        Code = "1H2411",
                        PeriodCode = "HM3",
                        Label = "1H November 2024",
                        FromDate = new DateOnly(2024, 11, 1),
                        UntilDate = new DateOnly(2024, 11, 15)
                    },
                    new()
                    {
                        Code = "2H2411",
                        PeriodCode = "HM4",
                        Label = "2H November 2024",
                        FromDate = new DateOnly(2024, 11, 16),
                        UntilDate = new DateOnly(2024, 11, 30)
                    },
                    new()
                    {
                        Code = "1H2412",
                        PeriodCode = "HM5",
                        Label = "1H December 2024",
                        FromDate = new DateOnly(2024, 12, 1),
                        UntilDate = new DateOnly(2024, 12, 15)
                    },
                    new()
                    {
                        Code = "2H2412",
                        PeriodCode = "HM6",
                        Label = "2H December 2024",
                        FromDate = new DateOnly(2024, 12, 16),
                        UntilDate = new DateOnly(2024, 12, 31)
                    },
                    new()
                    {
                        Code = "MM2411",
                        PeriodCode = "MM1",
                        Label = "November 2024",
                        FromDate = new DateOnly(2024, 11, 1),
                        UntilDate = new DateOnly(2024, 11, 30)
                    },
                    new()
                    {
                        Code = "MM2412",
                        PeriodCode = "MM2",
                        Label = "December 2024",
                        FromDate = new DateOnly(2024, 12, 1),
                        UntilDate = new DateOnly(2024, 12, 31)
                    }
                };
            }
        }

        private SeriesItem GetSeriesItemForDeltaTests(decimal price)
        {
            return new SeriesItem
            {
                AssessmentMethod = AssessmentMethod.Assessed.Value,
                Price = price,
                DataUsed = "Bid/offer"
            };
        }
    }
}