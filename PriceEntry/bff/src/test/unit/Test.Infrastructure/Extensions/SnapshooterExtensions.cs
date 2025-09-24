namespace Test.Infrastructure.Extensions
{
    using System;

    using Newtonsoft.Json.Linq;

    using Snapshooter;
    using Snapshooter.Xunit;

    using Xunit;

    public static class SnapshooterExtensions
    {
        public static void MatchSnapshot(this object currentResult, string snapshotName, string ignoreFieldsPath)
        {
            currentResult.MatchSnapshot(snapshotName, matchOptions => matchOptions.IgnoreFields(ignoreFieldsPath));
        }

        public static void MatchSnapshotWithSeriesItemId(this object currentResult, SnapshotNameExtension? snapshotNameExtension = null)
        {
            currentResult.MatchSnapshot(
                snapshotNameExtension: snapshotNameExtension,
                options => options
               .IsTypeField<Guid>("data.contentBlock.priceSeries[*].seriesItemId")
               .Assert(option => Assert.DoesNotContain(Guid.Empty, option.Fields<Guid>("data.contentBlock.priceSeries[*].seriesItemId")))
               .IgnoreAllFields("seriesItemId"));
        }

        public static void MatchSnapshotForDisplayWithInputsWithoutContentBlockIdOrLastModified(this object currentResult, SnapshotNameExtension? snapshotNameExtension = null)
        {
            currentResult.MatchSnapshot(
                snapshotNameExtension: snapshotNameExtension,
                options => options
               .IsTypeField<Guid>("data.contentBlockFromInputParametersForDisplay.contentBlockId")
               .Assert(option => Assert.NotEqual(Guid.Empty, option.Field<Guid>("data.contentBlockFromInputParametersForDisplay.contentBlockId")))
               .IgnoreAllFields("contentBlockId")
               .IgnoreAllFields("lastModifiedDate"));
        }

        public static void MatchSnapshotWithoutSeriesItemId(this object currentResult, SnapshotNameExtension? snapshotNameExtension = null)
        {
            currentResult.MatchSnapshot(
                snapshotNameExtension: snapshotNameExtension,
                options => options
                   .IgnoreAllFields("seriesItemId"));
        }

        public static void MatchSnapshotForPriceEntryUpdate(this object currentResult)
        {
            currentResult.MatchSnapshot(options => options
               .IsTypeFields<Guid>("data.updatePriceEntryGridData.id")
               .Assert(option => Assert.NotEqual(Guid.Empty, option.Field<Guid>("data.updatePriceEntryGridData.id")))
               .IgnoreAllFields("id"));
        }

        public static void MatchSnapshotWithLastModifiedDate(this object currentResult)
        {
            currentResult.MatchSnapshot(options => options.IgnoreAllFields("lastModifiedDate"));
        }

        public static void MatchSnapshotForAdjustedDeltaRangePrice(this object currentResult, string deltaTypeNonMktAdj, decimal? lowDeltaAfterAdjustment, decimal? highDeltaAfterAdjustment)
        {
            currentResult.MatchSnapshot(
                options => options
                   .Assert(
                        op => Assert.NotNull(
                            op.Field<string>(
                                "data.contentBlockForDisplay.priceSeriesItemForDisplay[0].priceDeltaType")))
                   .Assert(
                        op => Assert.Equal(
                            deltaTypeNonMktAdj,
                            op.Field<string?>(
                                "data.contentBlockForDisplay.priceSeriesItemForDisplay[0].priceDeltaType")))
                   .Assert(
                        op => Assert.Equal(
                            lowDeltaAfterAdjustment,
                            op.Field<decimal?>(
                                "data.contentBlockForDisplay.priceSeriesItemForDisplay[0].priceLowDelta")))
                   .Assert(
                        op => Assert.Equal(
                            highDeltaAfterAdjustment,
                            op.Field<decimal?>(
                                "data.contentBlockForDisplay.priceSeriesItemForDisplay[0].priceHighDelta")))
                   .IgnoreAllFields("lastModifiedDate"));
        }

        public static void MatchSnapshotForAdjustedDeltaSinglePrice(this object currentResult, string deltaTypeNonMktAdj, decimal? deltaAfterAdjustment)
        {
            currentResult.MatchSnapshot(
                options => options
                   .Assert(
                        op => Assert.NotNull(
                            op.Field<string>(
                                "data.contentBlockForDisplay.priceSeriesItemForDisplay[0].priceDeltaType")))
                   .Assert(
                        op => Assert.Equal(
                            deltaTypeNonMktAdj,
                            op.Field<string>(
                                "data.contentBlockForDisplay.priceSeriesItemForDisplay[0].priceDeltaType")))
                   .Assert(
                        op => Assert.Equal(
                            deltaAfterAdjustment,
                            op.Field<decimal>("data.contentBlockForDisplay.priceSeriesItemForDisplay[0].priceDelta")))
                   .IgnoreAllFields("lastModifiedDate"));
        }

        public static void MatchSnapshotForCanvasApiRequests(this object currentResult, SnapshotNameExtension? snapshotNameExtension = null)
        {
            currentResult.MatchSnapshot(
                snapshotNameExtension: snapshotNameExtension,
                options => options
                   .IsTypeField<Guid>("[*].ContentPackage.ContentPackageId")
                   .Assert(
                        option => Assert.DoesNotContain(
                            Guid.Empty,
                            option.Fields<Guid>("[*].ContentPackage.ContentPackageId"))));
        }

        public static void MatchSnapshotForWorkflowRequests(this object currentResult, SnapshotNameExtension? snapshotNameExtension = null)
        {
            currentResult.MatchSnapshot(
                snapshotNameExtension: snapshotNameExtension,
                options => options
                   .IsTypeField<Guid>("[*].Params.BusinessKey")
                   .Assert(
                        option => Assert.DoesNotContain(
                            Guid.Empty,
                            option.Fields<Guid>("[*].Params.BusinessKey"))));
        }

        public static void MatchSnapshotWithUserPreferenceId(this object currentResult, SnapshotNameExtension? snapshotNameExtension = null)
        {
            currentResult.MatchSnapshot(
                snapshotNameExtension: snapshotNameExtension,
                options => options.IgnoreAllFields("id"));
        }

        public static void MatchSnapshotIgnoringId(this object currentResult, SnapshotNameExtension? snapshotNameExtension = null)
        {
            currentResult.MatchSnapshot(
                snapshotNameExtension: snapshotNameExtension,
                options => options.IgnoreAllFields("Id"));
        }
    }
}