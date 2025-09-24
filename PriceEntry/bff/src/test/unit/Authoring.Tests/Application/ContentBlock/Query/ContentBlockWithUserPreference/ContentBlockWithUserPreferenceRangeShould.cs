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
    public class ContentBlockWithUserPreferenceRangeShould : ContentBlockWithUserPreferenceTestBase
    {
        public ContentBlockWithUserPreferenceRangeShould(AuthoringBffApplicationFactory factory)
            : base(factory)
        {
        }

        protected override List<string> PriceSeriesIds => new()
        {
            TestSeries.Melamine_China_Spot_FOB,
            TestSeries.Melamine_Asia_SE_Spot,
        };

        protected override SeriesItemTypeCode SeriesItemTypeCode => SeriesItemTypeCode.RangeSeries;

        protected override UserPreferenceInput BuildUserPreferenceInput()
        {
            return new UserPreferenceInput()
            {
                ContentBlockId = ContentBlockId,
                PriceSeriesInput = new List<string>()
                {
                    TestSeries.Melamine_Asia_SE_Spot,
                    TestSeries.Melamine_China_Spot_FOB,
                },
                ColumnInput = new List<ColumnInput>()
                {
                    new() { Field = "priceSeriesName", DisplayOrder = 0, Hidden = false },
                    new() { Field = "unitDisplay", DisplayOrder = 1, Hidden = true },
                    new() { Field = "priceLow", DisplayOrder = 2, Hidden = false },
                    new() { Field = "status", DisplayOrder = 3, Hidden = true },
                    new() { Field = "priceHigh", DisplayOrder = 4, Hidden = false },
                    new() { Field = "lastAssessmentPrice", DisplayOrder = 6, Hidden = true },
                    new() { Field = "lastAssessmentDate", DisplayOrder = 5, Hidden = true },
                    new() { Field = "priceLowDelta", DisplayOrder = 7, Hidden = false },
                    new() { Field = "priceHighDelta", DisplayOrder = 9, Hidden = false },
                    new() { Field = "priceMid", DisplayOrder = 8, Hidden = false },
                    new() { Field = "priceMidDelta", DisplayOrder = 10, Hidden = false }
                },
                PriceSeriesGridId = $"{ContentBlockId}_Grid_1"
            };
        }

        protected override UserPreferenceInput BuildUserPreferenceInputWithLessColumn()
        {
            return new UserPreferenceInput()
            {
                ContentBlockId = ContentBlockId,
                PriceSeriesInput = new List<string>()
                {
                    TestSeries.Melamine_Asia_SE_Spot,
                    TestSeries.Melamine_China_Spot_FOB,
                },
                ColumnInput = new List<ColumnInput>()
                {
                    new() { Field = "priceSeriesName", DisplayOrder = 0, Hidden = false },
                    new() { Field = "unitDisplay", DisplayOrder = 1, Hidden = true },
                    new() { Field = "priceLow", DisplayOrder = 2, Hidden = false },
                    new() { Field = "status", DisplayOrder = 3, Hidden = true },
                    new() { Field = "priceHigh", DisplayOrder = 4, Hidden = false },
                    new() { Field = "lastAssessmentPrice", DisplayOrder = 5, Hidden = true },
                    new() { Field = "priceLowDelta", DisplayOrder = 6, Hidden = false },
                    new() { Field = "priceHighDelta", DisplayOrder = 7, Hidden = false },
                    new() { Field = "priceMid", DisplayOrder = 8, Hidden = false },
                    new() { Field = "priceMidDelta", DisplayOrder = 9, Hidden = false }
                },
                PriceSeriesGridId = $"{ContentBlockId}_Grid_1"
            };
        }

        protected override List<string> GetPriceSeriesIds() => new()
        {
            TestSeries.Melamine_China_Spot_FOB,
            TestSeries.Melamine_Asia_SE_Spot,
            TestSeries.Melamine_China_Spot_Cif_2_6_Weeks,
        };
    }
}