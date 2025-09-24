namespace Authoring.Tests.Application.ContentBlock.Query.ContentBlockWithPriceSeries
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Authoring.Tests.Functional.Tests.Common.Infrastructure;

    using global::Infrastructure.Services.PeriodGenerator;

    using Snapshooter;

    using Test.Infrastructure.Authoring;
    using Test.Infrastructure.Authoring.BusinessFlows.PriceEntryBusinessFlow;
    using Test.Infrastructure.Authoring.BusinessFlows.PriceEntryBusinessFlow.PriceSeriesItemBuilders;
    using Test.Infrastructure.Authoring.GraphQLClients.PriceEntry;
    using Test.Infrastructure.Extensions;
    using Test.Infrastructure.Stubs;
    using Test.Infrastructure.TestData;

    using Xunit;

    [Collection(WebApplicationCollection.Name)]
    public class ContentBlockQueryWithPriceSeriesHasImpactedPricesShould : WebApplicationTestBase
    {
        protected static readonly DateTime Today = new(2024, 09, 01, 0, 0, 0, DateTimeKind.Utc);

        private const string ContentBlockId = "contentBlockId";

        public ContentBlockQueryWithPriceSeriesHasImpactedPricesShould(AuthoringBffApplicationFactory factory)
            : base(factory)
        {
            ContentBlockClient = new ContentBlockGraphQLClient(GraphQLClient);
            var testClock = GetService<TestClock>();
            testClock.SetUtcNow(Today);
        }

        protected ContentBlockGraphQLClient ContentBlockClient { get; }

        public static
            TheoryData<string, string, DateTime, Func<PriceEntryBusinessFlow, string, DateTime, PriceEntryBusinessFlow>>
            HasImpactedPricesTestData()
        {
            return new
                TheoryData<string, string, DateTime,
                    Func<PriceEntryBusinessFlow, string, DateTime, PriceEntryBusinessFlow>>
                {
                    {
                        "WITH_IMPACTED_PRICES_BEFORE_ROLLOVER", TestSeries.LNG_India_HM1, Today,
                        (businessFlow, seriesId, assessedDateTime) =>
                            businessFlow.SaveSingleValueWithReferencePriceSeriesItem(
                                seriesId,
                                assessedDateTime,
                                SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice())
                    },
                    {
                        "WITHOUT_IMPACTED_PRICES_DUE_TO_ROLLOVER", TestSeries.LNG_India_HM1, new DateTime(2024, 09, 26),
                        (businessFlow, seriesId, assessedDateTime) =>
                            businessFlow.SaveSingleValueWithReferencePriceSeriesItem(
                                seriesId,
                                assessedDateTime,
                                SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice())
                    },
                    {
                        "WITHOUT_IMPACTED_PRICES", TestSeries.Petchem_8602999, new DateTime(2024, 09, 01),
                        (businessFlow, seriesId, assessedDateTime) => businessFlow.SaveRangePriceSeriesItem(
                            seriesId,
                            assessedDateTime,
                            RangePriceSeriesItemBuilder.BuildDefaultValidSeriesItem())
                    }
                };
        }

        [Theory(Skip = "Skipping since data used works with advanced workflow, can be un-skipped after LNG moves to advanced workflow")]
        [MemberData(nameof(HasImpactedPricesTestData))]
        public async Task Return_Has_Impacted_Prices_Flag_When_In_Advanced_Correction_Workflow(
            string scenario,
            string seriesId,
            DateTime assessedDateTime,
            Func<PriceEntryBusinessFlow, string, DateTime, PriceEntryBusinessFlow> scenarioBusinessFlow)
        {
            var absolutePeriods = GetAbsolutePeriodsForHasImpactedPricesTests(assessedDateTime);

            var periodGeneratorServiceStub = GetService<PeriodGeneratorServiceStub>();
            periodGeneratorServiceStub.ReplaceAbsolutePeriods(absolutePeriods);

            var priceEntryBusinessFlow =
                new PriceEntryBusinessFlow(ContentBlockId, new List<string> { seriesId }, HttpClient)
                   .SaveContentBlockDefinition();

            await scenarioBusinessFlow.Invoke(priceEntryBusinessFlow, seriesId, assessedDateTime).Execute();

            var publishAndInitiateCorrectionBusinessFlow = priceEntryBusinessFlow
               .PublishWithAdvanceWorkflow(assessedDateTime)
               .InitiateCorrection(assessedDateTime);

            await publishAndInitiateCorrectionBusinessFlow.Execute();

            var result = await ContentBlockClient
                            .GetContentBlockWithPriceSeriesOnly(ContentBlockId, assessedDateTime)
                            .GetRawResponse();

            result.MatchSnapshotWithSeriesItemId(SnapshotNameExtension.Create(scenario));

            periodGeneratorServiceStub.ClearAbsolutePeriods();
        }

        [Fact(Skip = "Skipping since data used works with advanced workflow, can be un-skipped after LNG moves to advanced workflow")]
        public async Task Return_Has_Impacted_Prices_Flag_When_In_Advanced_Correction_Workflow_And_ContentBlock_With_Multiple_Grids()
        {
            var absolutePeriods = GetAbsolutePeriodsForHasImpactedPricesTests(Today);

            var periodGeneratorServiceStub = GetService<PeriodGeneratorServiceStub>();
            periodGeneratorServiceStub.ReplaceAbsolutePeriods(absolutePeriods);

            var priceSeriesIds = ContentBlockQueryHelper.CreatePriceSeriesIds(
                [TestSeries.LNG_China_HM1, TestSeries.LNG_China_HM2, TestSeries.LNG_China_HM3,
                  TestSeries.LNG_China_HM4, TestSeries.LNG_China_HM5],
                [TestSeries.LNG_China_MM1, TestSeries.LNG_China_MM2]);

            var priceEntryBusinessFlow =
            new PriceEntryBusinessFlow(ContentBlockId, priceSeriesIds, HttpClient)
                   .SaveContentBlockDefinition()
                   .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_China_HM1, Today, SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice())
                   .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_China_HM2, Today, SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice())
                   .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_China_HM3, Today, SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice())
                   .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_China_HM4, Today, SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice())
                   .SaveSingleValueWithReferencePriceSeriesItem(TestSeries.LNG_China_HM5, Today, SingleValueWithReferencePriceSeriesItemBuilder.BuildValidSeriesItemWithAssessedPrice())
                   .PublishWithAdvanceWorkflow(Today)
                  .InitiateCorrection(Today);

            await priceEntryBusinessFlow.Execute();

            var result = await ContentBlockClient
                            .GetContentBlockWithPriceSeriesOnly(ContentBlockId, Today)
                            .GetRawResponse();

            result.MatchSnapshotWithSeriesItemId();

            periodGeneratorServiceStub.ClearAbsolutePeriods();
        }

        private List<AbsolutePeriod> GetAbsolutePeriodsForHasImpactedPricesTests(DateTime referenceDate)
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
