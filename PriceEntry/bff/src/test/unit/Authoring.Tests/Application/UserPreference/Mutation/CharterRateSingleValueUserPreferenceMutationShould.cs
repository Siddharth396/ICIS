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
    public class CharterRateSingleValueUserPreferenceMutationShould : UserPreferenceTestBase
    {
        private readonly List<UserPreferenceInput> userPreferenceInputs;

        public CharterRateSingleValueUserPreferenceMutationShould(AuthoringBffApplicationFactory factory)
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
                    TestSeries.CharterRates_Pacific_Prompt_Two_Stroke,
                }
            },
            new()
            {
                Id = PriceSeriesGridIdTwo,
                Title = "Second Priceseries Grid",
                PriceSeriesIds = new List<string>
                {
                    TestSeries.CharterRates_Pacific_Prompt_Steam
                }
            }
        };

        protected override SeriesItemTypeCode SeriesItemTypeCode => SeriesItemTypeCode.CharterRateSingleValueSeries;

        protected override UserPreferenceInput UpdateUserPreferenceInput(UserPreferenceInput userPreferenceInput)
        {
            userPreferenceInput.ColumnInput = new List<ColumnInput>
            {
                 new() { Field = "priceSeriesName", DisplayOrder = 0, Hidden = false },
                 new() { Field = "unitDisplay", DisplayOrder = 1, Hidden = false },
                 new() { Field = "price", DisplayOrder = 2, Hidden = false },
                 new() { Field = "dataUsed", DisplayOrder = 3, Hidden = false },
                 new() { Field = "priceDelta", DisplayOrder = 5, Hidden = false },
                 new() { Field = "status", DisplayOrder = 4, Hidden = false },
                 new() { Field = "lastAssessmentPrice", DisplayOrder = 6, Hidden = false },
                 new() { Field = "lastAssessmentDate", DisplayOrder = 7, Hidden = false },
            };

            return userPreferenceInput;
        }

        protected override List<UserPreferenceInput> GetUserPreferenceInputs()
        {
            return userPreferenceInputs;
        }

        private static List<UserPreferenceInput> BuildUserPreferenceInputs()
        {
            return new List<UserPreferenceInput>()
            {
                new()
                {
                    PriceSeriesGridId = PriceSeriesGridIdOne,
                    ContentBlockId = ContentBlockId,
                    PriceSeriesInput = new List<string>()
                    {
                        TestSeries.CharterRates_Pacific_Prompt_Two_Stroke
                    },
                    ColumnInput = new List<ColumnInput>()
                    {
                        new() { Field = "priceSeriesName", DisplayOrder = 0, Hidden = false },
                        new() { Field = "unitDisplay", DisplayOrder = 1, Hidden = true },
                        new() { Field = "price", DisplayOrder = 2, Hidden = false },
                        new() { Field = "dataUsed", DisplayOrder = 3, Hidden = false },
                        new() { Field = "priceDelta", DisplayOrder = 5, Hidden = false },
                        new() { Field = "status", DisplayOrder = 4, Hidden = true },
                        new() { Field = "lastAssessmentPrice", DisplayOrder = 6, Hidden = false },
                        new() { Field = "lastAssessmentDate", DisplayOrder = 7, Hidden = false },
                    }
                },
                new()
                {
                    PriceSeriesGridId = PriceSeriesGridIdTwo,
                    ContentBlockId = ContentBlockId,
                    PriceSeriesInput = new List<string>()
                    {
                        TestSeries.CharterRates_Pacific_Prompt_Steam
                    },
                    ColumnInput = new List<ColumnInput>()
                    {
                        new() { Field = "priceSeriesName", DisplayOrder = 0, Hidden = false },
                        new() { Field = "unitDisplay", DisplayOrder = 1, Hidden = true },
                        new() { Field = "price", DisplayOrder = 2, Hidden = false },
                        new() { Field = "dataUsed", DisplayOrder = 3, Hidden = false },
                        new() { Field = "status", DisplayOrder = 4, Hidden = false },
                        new() { Field = "priceDelta", DisplayOrder = 5, Hidden = false },
                        new() { Field = "lastAssessmentPrice", DisplayOrder = 7, Hidden = false },
                        new() { Field = "lastAssessmentDate", DisplayOrder = 6, Hidden = false },
                    }
                },
            };
        }
    }
}
