namespace Authoring.Tests.Application.ImpactedPrices.Query
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Authoring.Tests.Functional.Tests.Common.Infrastructure;

    using global::Infrastructure.Services.PeriodGenerator;

    using Snapshooter;
    using Snapshooter.Xunit;

    using Test.Infrastructure.Authoring;
    using Test.Infrastructure.Authoring.BusinessFlows.PriceEntryBusinessFlow;
    using Test.Infrastructure.Authoring.BusinessFlows.PriceEntryBusinessFlow.PriceSeriesItemBuilders;
    using Test.Infrastructure.Authoring.GraphQLClients.PriceEntry;
    using Test.Infrastructure.Extensions;
    using Test.Infrastructure.Stubs;
    using Test.Infrastructure.TestData;

    using Xunit;

    [Collection(WebApplicationCollection.Name)]
    public class ImpactedPricesQueriesShould : WebApplicationTestBase
    {
        private static readonly DateTime Now = TestData.Now;

        private readonly ImpactedPricesGraphQLClient impactedPricesClient;

        public ImpactedPricesQueriesShould(AuthoringBffApplicationFactory factory)
            : base(factory)
        {
            var testClock = GetService<TestClock>();
            testClock.SetUtcNow(Now);

            impactedPricesClient = new ImpactedPricesGraphQLClient(GraphQLClient);
        }

        public static TheoryData<string, DateTime> AssessedDateTimeMemberData()
        {
            return new TheoryData<string, DateTime>()
            {
                { "BEFORE_ROLLOVER", new DateTime(2024, 09, 01) },
                { "AFTER_ROLLOVER", new DateTime(2024, 09, 26) }
            };
        }

        [Fact]
        public async Task Return_Impacted_Prices_For_Series_Item_Styrene()
        {
            // Arrange
            var assessedDateTime = new DateTime(2025, 01, 01, 0, 0, 0);

            var saveContentBlockBusinessFlow =
                new PriceEntryBusinessFlow("styreneContentBlockId", [TestSeries.Styrene_Dagu_Spot_China_N], HttpClient)
                   .SaveRangePriceSeriesItem(TestSeries.Styrene_Dagu_Spot_China_N, assessedDateTime, RangePriceSeriesItemBuilder.BuildDefaultValidSeriesItem());

            await saveContentBlockBusinessFlow.Execute();

            // Act
            var getImpactedPricesResponse = await impactedPricesClient.GetImpactedPrices(TestSeries.Styrene_Dagu_Spot_China_N, assessedDateTime).GetRawResponse();

            getImpactedPricesResponse.MatchSnapshot();
        }

        [Theory(Skip = "Skipping since data used works with advanced workflow, can be un-skipped after LNG moves to advanced workflow")]
        [MemberData(nameof(AssessedDateTimeMemberData))]
        public async Task Return_Impacted_Prices_For_Series_Item_LNG(string scenario, DateTime assessedDateTime)
        {
            // Arrange
            const string HalfMonthInputsContentBlockId = "a7436dfb-ce01-459e-92a1-a82b18017625";
            const string FullMonthDerivedContentBlockId = "ae99fbf9-81dc-46c2-aa97-4e159de7db79";
            const string EaxHalfMonthFortnightDerivedContentBlockId = "b7280eec-4aa4-4959-8b2f-0d64b9ce39cd";
            const string EaxDerivedContentBlockId = "3b0f5b49-f40b-442c-913f-a0d30d3f05c0";

            const string ReferenceContentBlockId = "b1b1b1b1-b1b1-b1b1-b1b1-b1b1b1b1b1b1";

            var absolutePeriods = GetAbsolutePeriodsForImpactedPricesTests(assessedDateTime);

            var periodGeneratorServiceStub = GetService<PeriodGeneratorServiceStub>();
            periodGeneratorServiceStub.ReplaceAbsolutePeriods(absolutePeriods);

            await PublishHalfMonthInputs(HalfMonthInputsContentBlockId, assessedDateTime);

            await PublishDerivedContentBlock(FullMonthDerivedContentBlockId, assessedDateTime, TestSeries.LngFullMonthDerivedSeriesIds);
            await PublishDerivedContentBlock(EaxHalfMonthFortnightDerivedContentBlockId, assessedDateTime, TestSeries.LngEaxHalfMonthDerivedFortnightSeriesIds);
            await PublishDerivedContentBlock(EaxDerivedContentBlockId, assessedDateTime, TestSeries.LngEaxFullMonthDerivedSeriesIds);

            await PublishReferenceContentBlock(ReferenceContentBlockId, assessedDateTime, [TestSeries.LNG_India_HM3]);

            // Assert
            var getImpactedPricesResponse = await impactedPricesClient.GetImpactedPrices(TestSeries.LNG_China_HM3, assessedDateTime).GetRawResponse();

            getImpactedPricesResponse.MatchSnapshot(SnapshotNameExtension.Create(scenario));

            periodGeneratorServiceStub.ClearAbsolutePeriods();
        }

        [Fact]
        public async Task Throw_Error_For_Missing_Price_Series()
        {
            var assessedDateTime = new DateTime(2025, 01, 01, 0, 0, 0);

            var getImpactedPricesResponse = await impactedPricesClient.GetImpactedPrices("8745c52c-965f-4bab-b04b-37a4c548eecc", assessedDateTime).GetRawResponse();

            getImpactedPricesResponse.MatchSnapshot();
        }

        private async Task PublishHalfMonthInputs(string inputsContentBlockId, DateTime assessedDateTime)
        {
            var publishHalfMonthInputsBusinessFlow =
                new PriceEntryBusinessFlow(
                        inputsContentBlockId,
                        TestSeries.LngHalfMonthInputSeriesIds,
                        HttpClient)
                   .SaveContentBlockDefinition()
                   .SaveSingleValueWithReferencePriceSeriesItem(
                        TestSeries.LNG_China_HM1,
                        assessedDateTime,
                        SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice(10))
                   .SaveSingleValueWithReferencePriceSeriesItem(
                        TestSeries.LNG_China_HM2,
                        assessedDateTime,
                        SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice(20))
                   .SaveSingleValueWithReferencePriceSeriesItem(
                        TestSeries.LNG_China_HM3,
                        assessedDateTime,
                        SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice(30))
                   .SaveSingleValueWithReferencePriceSeriesItem(
                        TestSeries.LNG_China_HM4,
                        assessedDateTime,
                        SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice(40))
                   .SaveSingleValueWithReferencePriceSeriesItem(
                        TestSeries.LNG_China_HM5,
                        assessedDateTime,
                        SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice(50))
                   .SaveSingleValueWithReferencePriceSeriesItem(
                        TestSeries.LNG_Japan_HM1,
                        assessedDateTime,
                        SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice(10))
                   .SaveSingleValueWithReferencePriceSeriesItem(
                        TestSeries.LNG_Japan_HM2,
                        assessedDateTime,
                        SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice(20))
                   .SaveSingleValueWithReferencePriceSeriesItem(
                        TestSeries.LNG_Japan_HM3,
                        assessedDateTime,
                        SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice(30))
                   .SaveSingleValueWithReferencePriceSeriesItem(
                        TestSeries.LNG_Japan_HM4,
                        assessedDateTime,
                        SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice(40))
                   .SaveSingleValueWithReferencePriceSeriesItem(
                        TestSeries.LNG_Japan_HM5,
                        assessedDateTime,
                        SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice(50))
                   .SaveSingleValueWithReferencePriceSeriesItem(
                        TestSeries.LNG_Taiwan_HM1,
                        assessedDateTime,
                        SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice(10))
                   .SaveSingleValueWithReferencePriceSeriesItem(
                        TestSeries.LNG_Taiwan_HM2,
                        assessedDateTime,
                        SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice(20))
                   .SaveSingleValueWithReferencePriceSeriesItem(
                        TestSeries.LNG_Taiwan_HM3,
                        assessedDateTime,
                        SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice(30))
                   .SaveSingleValueWithReferencePriceSeriesItem(
                        TestSeries.LNG_Taiwan_HM4,
                        assessedDateTime,
                        SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice(40))
                   .SaveSingleValueWithReferencePriceSeriesItem(
                        TestSeries.LNG_Taiwan_HM5,
                        assessedDateTime,
                        SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice(50))
                   .SaveSingleValueWithReferencePriceSeriesItem(
                        TestSeries.LNG_South_Korea_HM1,
                        assessedDateTime,
                        SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice(10))
                   .SaveSingleValueWithReferencePriceSeriesItem(
                        TestSeries.LNG_South_Korea_HM2,
                        assessedDateTime,
                        SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice(20))
                   .SaveSingleValueWithReferencePriceSeriesItem(
                        TestSeries.LNG_South_Korea_HM3,
                        assessedDateTime,
                        SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice(30))
                   .SaveSingleValueWithReferencePriceSeriesItem(
                        TestSeries.LNG_South_Korea_HM4,
                        assessedDateTime,
                        SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice(40))
                   .SaveSingleValueWithReferencePriceSeriesItem(
                        TestSeries.LNG_South_Korea_HM5,
                        assessedDateTime,
                        SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice(50))
                   .InitiateSendForReview(assessedDateTime)
                   .AcknowledgeSendForReview(assessedDateTime)
                   .InitiateStartReview(assessedDateTime)
                   .AcknowledgeStartReview(assessedDateTime)
                   .InitiateApproval(assessedDateTime)
                   .AcknowledgeApproval(assessedDateTime)
                   .AcknowledgePublished(assessedDateTime);

            await publishHalfMonthInputsBusinessFlow.Execute();
        }

        private async Task PublishDerivedContentBlock(string derivedContentBlockId, DateTime assessedDateTime, List<string> derivedPriceSeriesIds)
        {
            var setupDerivedContentBlockBusinessFlow = new PriceEntryBusinessFlow(
                    derivedContentBlockId,
                    derivedPriceSeriesIds,
                    HttpClient)
               .SaveContentBlockDefinition()
               .InitiateSendForReview(assessedDateTime)
               .AcknowledgeSendForReview(assessedDateTime)
               .InitiateStartReview(assessedDateTime)
               .AcknowledgeStartReview(assessedDateTime)
               .InitiateApproval(assessedDateTime)
               .AcknowledgeApproval(assessedDateTime)
               .AcknowledgePublished(assessedDateTime);

            await setupDerivedContentBlockBusinessFlow.Execute();
        }

        private async Task PublishReferenceContentBlock(string referenceContentBlock, DateTime assessedDateTime, List<string> referencePriceSeriesIds)
        {
            var setupReferenceContentBlockBusinessFlow = new PriceEntryBusinessFlow(
                    referenceContentBlock,
                    referencePriceSeriesIds,
                    HttpClient)
               .SaveContentBlockDefinition()
               .SaveSingleValueWithReferencePriceSeriesItem(
                    TestSeries.LNG_India_HM3,
                    assessedDateTime,
                    SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithPremiumDiscount("LNG EAX Index"))
               .InitiateSendForReview(assessedDateTime)
               .AcknowledgeSendForReview(assessedDateTime)
               .InitiateStartReview(assessedDateTime)
               .AcknowledgeStartReview(assessedDateTime)
               .InitiateApproval(assessedDateTime)
               .AcknowledgeApproval(assessedDateTime)
               .AcknowledgePublished(assessedDateTime);

            await setupReferenceContentBlockBusinessFlow.Execute();
        }

        private List<AbsolutePeriod> GetAbsolutePeriodsForImpactedPricesTests(DateTime referenceDate)
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
    }
}
