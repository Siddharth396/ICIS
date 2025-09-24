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
    public class ContentBlockWithUserPreferenceSingleValueShould : ContentBlockWithUserPreferenceTestBase
    {
        public ContentBlockWithUserPreferenceSingleValueShould(AuthoringBffApplicationFactory factory)
            : base(factory)
        {
        }

        protected override List<string> PriceSeriesIds => [TestSeries.Petchem_8603620];

        protected override SeriesItemTypeCode SeriesItemTypeCode => SeriesItemTypeCode.SingleValueSeries;

        protected override UserPreferenceInput BuildUserPreferenceInput()
        {
            return new UserPreferenceInput()
            {
                ContentBlockId = ContentBlockId,
                PriceSeriesInput = [TestSeries.Petchem_8603620],
                ColumnInput = new List<ColumnInput>()
                {
                    new() { Field = "priceSeriesName", DisplayOrder = 0, Hidden = false },
                    new() { Field = "unitDisplay", DisplayOrder = 1, Hidden = true },
                    new() { Field = "price", DisplayOrder = 3, Hidden = false },
                    new() { Field = "status", DisplayOrder = 2, Hidden = true },
                    new() { Field = "priceDelta", DisplayOrder = 4, Hidden = false },
                    new() { Field = "lastAssessmentPrice", DisplayOrder = 6, Hidden = false },
                    new() { Field = "lastAssessmentDate", DisplayOrder = 5, Hidden = false },
                },
                PriceSeriesGridId = $"{ContentBlockId}_Grid_1"
            };
        }

        protected override UserPreferenceInput BuildUserPreferenceInputWithLessColumn()
        {
            return new UserPreferenceInput()
            {
                ContentBlockId = ContentBlockId,
                PriceSeriesInput = [TestSeries.Petchem_8603620],
                ColumnInput = new List<ColumnInput>()
                {
                    new() { Field = "priceSeriesName", DisplayOrder = 0, Hidden = false },
                    new() { Field = "unitDisplay", DisplayOrder = 1, Hidden = true },
                    new() { Field = "price", DisplayOrder = 3, Hidden = false },
                    new() { Field = "status", DisplayOrder = 2, Hidden = true },
                    new() { Field = "priceDelta", DisplayOrder = 4, Hidden = false },
                    new() { Field = "lastAssessmentPrice", DisplayOrder = 5, Hidden = false }
                },
                PriceSeriesGridId = $"{ContentBlockId}_Grid_1"
            };
        }

        protected override List<string> GetPriceSeriesIds() => [TestSeries.Petchem_8603620];
    }
}