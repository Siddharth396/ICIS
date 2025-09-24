namespace Authoring.Tests.Application.UserPreference.Mutation
{
    using System.Collections.Generic;

    using Authoring.Tests.Functional.Tests.Common.Infrastructure;

    using global::BusinessLayer.ContentBlock.DTOs.Input;
    using global::BusinessLayer.PriceEntry.ValueObjects;
    using global::BusinessLayer.UserPreference.DTOs.Input;

    using Test.Infrastructure.Authoring;
    using Test.Infrastructure.TestData;

    using Xunit;

    [Collection(WebApplicationCollection.Name)]
    public class RangeUserPreferenceMutationShould : UserPreferenceTestBase
    {
        private readonly List<UserPreferenceInput> userPreferenceInputs;

        public RangeUserPreferenceMutationShould(AuthoringBffApplicationFactory factory)
            : base(factory)
        {
            userPreferenceInputs = BuildUserPreferenceInputs();
        }

        protected override List<PriceSeriesGrid> PriceSeriesGrids => new()
        {
            new()
            {
                Id = PriceSeriesGridIdOne,
                Title = "First Priceseries Grid",
                PriceSeriesIds = new List<string>
                {
                    TestSeries.Melamine_China_Spot_FOB,
                    TestSeries.Melamine_Asia_SE_Spot,
                }
            },
            new()
            {
                Id = PriceSeriesGridIdTwo,
                Title = "Second Priceseries Grid",
                PriceSeriesIds = new List<string> { TestSeries.Melamine_China_Spot_Cif_2_6_Weeks }
            }
        };

        protected override SeriesItemTypeCode SeriesItemTypeCode => SeriesItemTypeCode.RangeSeries;

        protected override UserPreferenceInput UpdateUserPreferenceInput(UserPreferenceInput userPreferenceInput)
        {
            userPreferenceInput.PriceSeriesInput = new List<string>
            {
                TestSeries.Melamine_China_Spot_FOB,
                TestSeries.Melamine_Asia_SE_Spot
            };

            userPreferenceInput.ColumnInput = new List<ColumnInput>()
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
                new() { Field = "priceMidDelta", DisplayOrder = 10, Hidden = false },
                new() { Field = "adjustedPriceLowDelta", DisplayOrder = 11, Hidden = false },
                new() { Field = "adjustedPriceHighDelta", DisplayOrder = 12, Hidden = false },
                new() { Field = "adjustedPriceMidDelta", DisplayOrder = 13, Hidden = false },
                new() { Field = "period", DisplayOrder = 14, Hidden = true }
            };

            return userPreferenceInput;
        }

        protected override List<UserPreferenceInput> GetUserPreferenceInputs()
        {
            return userPreferenceInputs;
        }

        private static List<UserPreferenceInput> BuildUserPreferenceInputs()
        {
            return new List<UserPreferenceInput>
            {
                new()
                {
                    PriceSeriesGridId = PriceSeriesGridIdOne,
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
                        new() { Field = "priceMidDelta", DisplayOrder = 10, Hidden = false },
                        new() { Field = "adjustedPriceLowDelta", DisplayOrder = 11, Hidden = false },
                        new() { Field = "adjustedPriceHighDelta", DisplayOrder = 12, Hidden = false },
                        new() { Field = "adjustedPriceMidDelta", DisplayOrder = 13, Hidden = false },
                        new() { Field = "period", DisplayOrder = 14, Hidden = true }
                    }
                },
                new()
                {
                    PriceSeriesGridId = PriceSeriesGridIdTwo,
                    ContentBlockId = ContentBlockId,
                    PriceSeriesInput = new List<string>()
                    {
                        TestSeries.Melamine_China_Spot_Cif_2_6_Weeks
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
                        new() { Field = "priceMidDelta", DisplayOrder = 10, Hidden = false },
                        new() { Field = "adjustedPriceLowDelta", DisplayOrder = 11, Hidden = false },
                        new() { Field = "adjustedPriceHighDelta", DisplayOrder = 12, Hidden = false },
                        new() { Field = "adjustedPriceMidDelta", DisplayOrder = 13, Hidden = false },
                        new() { Field = "period", DisplayOrder = 14, Hidden = true }
                    }
                }
            };
        }
    }
}