namespace Authoring.Tests.Application.WorkflowIntegration.PriceEntry
{
    using Authoring.Tests.Functional.Tests.Common.Infrastructure;

    using global::BusinessLayer.PriceEntry.DTOs.Input;
    using global::BusinessLayer.PriceEntry.ValueObjects;

    using Test.Infrastructure.Authoring;
    using Test.Infrastructure.TestData;

    using Xunit;

    [Collection(WebApplicationCollection.Name)]
    public class SingleValuePriceEntryDataStatusShould : WorkflowIntegrationTestBase
    {
        public SingleValuePriceEntryDataStatusShould(AuthoringBffApplicationFactory factory)
            : base(factory)
        {
        }

        protected override string PriceSeriesId => TestSeries.Petchem_8603620;

        protected override SeriesItemTypeCode SeriesItemTypeCode => SeriesItemTypeCode.SingleValueSeries;

        protected override SeriesItem BuildSeriesItem()
        {
            return new SeriesItem { Price = 50 };
        }
    }
}