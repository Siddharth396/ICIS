namespace Authoring.Tests.Application.WorkflowIntegration.PriceEntry
{
    using Authoring.Tests.Functional.Tests.Common.Infrastructure;

    using global::BusinessLayer.PriceEntry.DTOs.Input;
    using global::BusinessLayer.PriceEntry.ValueObjects;

    using Test.Infrastructure.Authoring;
    using Test.Infrastructure.TestData;

    using Xunit;

    [Collection(WebApplicationCollection.Name)]
    public class SingleRefPriceEntryDataStatusShould : WorkflowIntegrationTestBase
    {
        public SingleRefPriceEntryDataStatusShould(AuthoringBffApplicationFactory factory)
            : base(factory)
        {
        }

        protected override string PriceSeriesId => TestSeries.LNG_China_HM1;

        protected override SeriesItemTypeCode SeriesItemTypeCode => SeriesItemTypeCode.SingleValueWithReferenceSeries;

        protected override SeriesItem BuildSeriesItem()
        {
            return new SeriesItem
            {
                AssessmentMethod = AssessmentMethod.PremiumDiscount.Value,
                ReferenceMarketName = "TTF",
                PremiumDiscount = 5,
                DataUsed = "Bid/offer"
            };
        }
    }
}