namespace BusinessLayer.ImpactedPrices.DTOs.Output
{
    using System.Collections.Generic;

    public class ImpactedPrices
    {
        public required string SeriesName { get; set; }

        public required List<string> ImpactedDerivedPriceSeriesIds { get; set; } = [];

        public required List<string> ImpactedReferencePriceSeriesIds { get; set; } = [];

        public required List<string> ImpactedCalculatedPriceSeriesIds { get; set; } = [];
    }
}
