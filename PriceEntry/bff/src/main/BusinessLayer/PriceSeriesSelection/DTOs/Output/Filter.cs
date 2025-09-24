namespace BusinessLayer.PriceSeriesSelection.DTOs.Output
{
    using System.Collections.Generic;

    public class Filter
    {
        public required string Name { get; set; }

        public required List<FilterDetail> FilterDetails { get; set; }
    }
}
