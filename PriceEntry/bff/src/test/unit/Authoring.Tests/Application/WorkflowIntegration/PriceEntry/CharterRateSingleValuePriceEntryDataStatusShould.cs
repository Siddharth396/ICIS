namespace Authoring.Tests.Application.WorkflowIntegration.PriceEntry
{
    using Authoring.Tests.Functional.Tests.Common.Infrastructure;

    using global::BusinessLayer.PriceEntry.DTOs.Input;
    using global::BusinessLayer.PriceEntry.ValueObjects;

    using Test.Infrastructure.Authoring;
    using Test.Infrastructure.Authoring.BusinessFlows.PriceEntryBusinessFlow.PriceSeriesItemBuilders;
    using Test.Infrastructure.TestData;

    using Xunit;

    [Collection(WebApplicationCollection.Name)]
    public class CharterRateSingleValuePriceEntryDataStatusShould : WorkflowIntegrationTestBase
    {
        public CharterRateSingleValuePriceEntryDataStatusShould(AuthoringBffApplicationFactory factory)
            : base(factory)
        {
        }

        protected override string PriceSeriesId => TestSeries.CharterRates_Pacific_Prompt_Two_Stroke;

        protected override SeriesItemTypeCode SeriesItemTypeCode => SeriesItemTypeCode.CharterRateSingleValueSeries;

        protected override SeriesItem BuildSeriesItem()
        {
            return CharterRateSingleValuePriceSeriesItemBuilder.BuildDefaultValidSeriesItem();
        }
    }
}
