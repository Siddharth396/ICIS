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
    public class ContentBlockWithUserPreferenceCharterRateSingleValueShould : ContentBlockWithUserPreferenceTestBase
    {
        public ContentBlockWithUserPreferenceCharterRateSingleValueShould(AuthoringBffApplicationFactory factory)
            : base(factory)
        {
        }

        protected override List<string> PriceSeriesIds => new()
        {
            TestSeries.CharterRates_Pacific_Prompt_Two_Stroke
        };

        protected override SeriesItemTypeCode SeriesItemTypeCode => SeriesItemTypeCode.CharterRateSingleValueSeries;

        protected override UserPreferenceInput BuildUserPreferenceInput()
        {
            return new UserPreferenceInput()
            {
                ContentBlockId = ContentBlockId,
                PriceSeriesInput = new List<string>()
                {
                    TestSeries.CharterRates_Pacific_Prompt_Two_Stroke
                },
                ColumnInput = new List<ColumnInput>()
                {
                    new() { Field = "priceSeriesName", DisplayOrder = 0, Hidden = false },
                    new() { Field = "price", DisplayOrder = 3, Hidden = false },
                    new() { Field = "dataUsed", DisplayOrder = 1, Hidden = false },
                    new() { Field = "status", DisplayOrder = 2, Hidden = true },
                    new() { Field = "priceDelta", DisplayOrder = 4, Hidden = false },
                    new() { Field = "lastAssessmentPrice", DisplayOrder = 6, Hidden = false },
                    new() { Field = "lastAssessmentDate", DisplayOrder = 5, Hidden = false },
                    new() { Field = "unitDisplay", DisplayOrder = 7, Hidden = false }
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
                    TestSeries.CharterRates_Pacific_Prompt_Two_Stroke
                },
                ColumnInput = new List<ColumnInput>()
                {
                    new() { Field = "priceSeriesName", DisplayOrder = 0, Hidden = false },
                    new() { Field = "price", DisplayOrder = 3, Hidden = false },
                    new() { Field = "dataUsed", DisplayOrder = 1, Hidden = false },
                    new() { Field = "status", DisplayOrder = 2, Hidden = true },
                    new() { Field = "priceDelta", DisplayOrder = 4, Hidden = false },
                    new() { Field = "lastAssessmentPrice", DisplayOrder = 5, Hidden = false },
                    new() { Field = "unitDisplay", DisplayOrder = 6, Hidden = false }
                },
                PriceSeriesGridId = $"{ContentBlockId}_Grid_1"
            };
        }

        protected override List<string> GetPriceSeriesIds() => new()
        {
            TestSeries.CharterRates_Pacific_Prompt_Two_Stroke
        };
    }
}
