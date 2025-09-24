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
    public class SingleRefUserPreferenceMutationShould : UserPreferenceTestBase
    {
        private readonly List<UserPreferenceInput> userPreferenceInputs;

        public SingleRefUserPreferenceMutationShould(AuthoringBffApplicationFactory factory)
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
                    TestSeries.LNG_China_HM1,
                    TestSeries.LNG_China_HM2,
                    TestSeries.LNG_China_HM3,
                }
            },
            new()
            {
                Id = PriceSeriesGridIdTwo,
                Title = "Second Priceseries Grid",
                PriceSeriesIds = new List<string> { TestSeries.LNG_China_MM1 }
            }
        };

        protected override SeriesItemTypeCode SeriesItemTypeCode => SeriesItemTypeCode.SingleValueWithReferenceSeries;

        protected override UserPreferenceInput UpdateUserPreferenceInput(UserPreferenceInput userPreferenceInput)
        {
            userPreferenceInput.PriceSeriesInput = new List<string>
            {
                TestSeries.LNG_China_HM3,
                TestSeries.LNG_China_HM1,
                TestSeries.LNG_China_HM2,
            };
            userPreferenceInput.ColumnInput = new List<ColumnInput>()
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
                new() { Field = "premiumDiscount", DisplayOrder = 11, Hidden = false },
                new() { Field = "dataUsed", DisplayOrder = 10, Hidden = false }
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
                        TestSeries.LNG_China_HM1,
                        TestSeries.LNG_China_HM2,
                        TestSeries.LNG_China_HM3,
                     },
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
                     }
                 },
                 new()
                 {
                     PriceSeriesGridId = PriceSeriesGridIdTwo,
                     ContentBlockId = ContentBlockId,
                     PriceSeriesInput = new List<string>()
                     {
                        TestSeries.LNG_China_MM1,
                     },
                     ColumnInput = new List<ColumnInput>()
                     {
                        new() { Field = "priceSeriesName", DisplayOrder = 0, Hidden = false },
                        new() { Field = "period", DisplayOrder = 1, Hidden = true },
                        new() { Field = "unitDisplay", DisplayOrder = 2, Hidden = true },
                        new() { Field = "assessmentMethod", DisplayOrder = 3, Hidden = false },
                        new() { Field = "referencePrice", DisplayOrder = 4, Hidden = false },
                        new() { Field = "price", DisplayOrder = 5, Hidden = false },
                        new() { Field = "premiumDiscount", DisplayOrder = 6, Hidden = false },
                        new() { Field = "dataUsed", DisplayOrder = 7, Hidden = false },
                        new() { Field = "status", DisplayOrder = 8, Hidden = true },
                        new() { Field = "priceDelta", DisplayOrder = 9, Hidden = false },
                        new() { Field = "lastAssessmentPrice", DisplayOrder = 10, Hidden = false },
                        new() { Field = "lastAssessmentDate", DisplayOrder = 11, Hidden = false }
                     }
                 },
            };
        }
    }
}