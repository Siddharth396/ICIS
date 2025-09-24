namespace Authoring.Tests.Application.ContentBlock.Query.ContentBlockWithUserPreference
{
    using System.Collections.Generic;

    using Authoring.Tests.Functional.Tests.Common.Infrastructure;

    using global::BusinessLayer.PriceEntry.ValueObjects;
    using global::BusinessLayer.UserPreference.DTOs.Input;

    using Test.Infrastructure.Authoring;
    using Test.Infrastructure.TestData;

    using Xunit;

    [Collection(WebApplicationCollection.Name)]
    public class ContentBlockWithUserPreferenceSingleRefShould : ContentBlockWithUserPreferenceTestBase
    {
        public ContentBlockWithUserPreferenceSingleRefShould(AuthoringBffApplicationFactory factory)
            : base(factory)
        {
        }

        protected override List<string> PriceSeriesIds =>
        [
            TestSeries.LNG_China_HM1,
            TestSeries.LNG_China_HM2,
            TestSeries.LNG_Japan_HM1,
            TestSeries.LNG_Taiwan_HM1
        ];

        protected override SeriesItemTypeCode SeriesItemTypeCode => SeriesItemTypeCode.SingleValueWithReferenceSeries;

        protected override UserPreferenceInput BuildUserPreferenceInput()
        {
            return new UserPreferenceInput()
            {
                ContentBlockId = ContentBlockId,
                PriceSeriesInput =
                [
                    TestSeries.LNG_China_HM1,
                    TestSeries.LNG_Japan_HM1,
                    TestSeries.LNG_China_HM2,
                    TestSeries.LNG_Taiwan_HM1
                ],
                ColumnInput = new List<ColumnInput>()
                {
                    new() { Field = "priceSeriesName", DisplayOrder = 0, Hidden = false },
                    new() { Field = "unitDisplay", DisplayOrder = 1, Hidden = true },
                    new() { Field = "price", DisplayOrder = 3, Hidden = false },
                    new() { Field = "status", DisplayOrder = 2, Hidden = true },
                    new() { Field = "priceDelta", DisplayOrder = 4, Hidden = false },
                    new() { Field = "lastAssessmentPrice", DisplayOrder = 6, Hidden = false },
                    new() { Field = "lastAssessmentDate", DisplayOrder = 5, Hidden = false },
                    new() { Field = "period", DisplayOrder = 7, Hidden = true },
                    new() { Field = "assessmentMethod", DisplayOrder = 8, Hidden = false },
                    new() { Field = "referencePrice", DisplayOrder = 9, Hidden = false },
                    new() { Field = "premiumDiscount", DisplayOrder = 10, Hidden = false },
                    new() { Field = "dataUsed", DisplayOrder = 11, Hidden = false }
                },
                PriceSeriesGridId = $"{ContentBlockId}_Grid_1"
            };
        }

        protected override UserPreferenceInput BuildUserPreferenceInputWithLessColumn()
        {
            return new UserPreferenceInput()
            {
                ContentBlockId = ContentBlockId,
                PriceSeriesInput =
                [
                    TestSeries.LNG_China_HM1,
                    TestSeries.LNG_Japan_HM1,
                    TestSeries.LNG_China_HM2,
                    TestSeries.LNG_Taiwan_HM1
                ],
                ColumnInput =
                [
                    new() { Field = "priceSeriesName", DisplayOrder = 0, Hidden = false },
                    new() { Field = "unitDisplay", DisplayOrder = 1, Hidden = true },
                    new() { Field = "price", DisplayOrder = 3, Hidden = false },
                    new() { Field = "status", DisplayOrder = 2, Hidden = true },

                    new() { Field = "priceDelta", DisplayOrder = 4, Hidden = false },
                    new() { Field = "lastAssessmentPrice", DisplayOrder = 5, Hidden = false },
                    new() { Field = "period", DisplayOrder = 8, Hidden = true },
                    new() { Field = "assessmentMethod", DisplayOrder = 7, Hidden = false },

                    new() { Field = "referencePrice", DisplayOrder = 6, Hidden = false },
                    new() { Field = "premiumDiscount", DisplayOrder = 10, Hidden = false },
                    new() { Field = "dataUsed", DisplayOrder = 9, Hidden = false }
                ],
                PriceSeriesGridId = $"{ContentBlockId}_Grid_1"
            };
        }

        protected override List<string> GetPriceSeriesIds() =>
        [
            TestSeries.LNG_China_HM1,
            TestSeries.LNG_China_HM2,
            TestSeries.LNG_Japan_HM1,
            TestSeries.LNG_Taiwan_HM1,

            TestSeries.LNG_China_HM4,
            TestSeries.LNG_China_HM5
        ];
    }
}