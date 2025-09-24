namespace Authoring.Tests.Application.WorkflowIntegration.PriceEntry
{
    using Authoring.Tests.Functional.Tests.Common.Infrastructure;

    using global::BusinessLayer.PriceEntry.DTOs.Input;
    using global::BusinessLayer.PriceEntry.ValueObjects;

    using Test.Infrastructure.Authoring;
    using Test.Infrastructure.TestData;

    using Xunit;

    [Collection(WebApplicationCollection.Name)]
    public class RangePriceEntryDataStatusShould : WorkflowIntegrationTestBase
    {
        public RangePriceEntryDataStatusShould(AuthoringBffApplicationFactory factory)
            : base(factory)
        {
        }

        protected override string PriceSeriesId => TestSeries.Melamine_Asia_SE_Spot;

        protected override SeriesItemTypeCode SeriesItemTypeCode => SeriesItemTypeCode.RangeSeries;

        protected override SeriesItem BuildSeriesItem()
        {
            return new SeriesItem { PriceLow = 50, PriceHigh = 100 };
        }
    }
}