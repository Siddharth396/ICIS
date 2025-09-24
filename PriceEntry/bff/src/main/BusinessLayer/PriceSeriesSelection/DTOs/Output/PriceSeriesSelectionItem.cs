namespace BusinessLayer.PriceSeriesSelection.DTOs.Output
{
    using System.Collections.Generic;

    public class PriceSeriesSelectionItem
    {
        public required List<PriceSeriesDetail> PriceSeriesDetails { get; set; }

        public required string SeriesItemTypeCode { get; set; }
    }
}
