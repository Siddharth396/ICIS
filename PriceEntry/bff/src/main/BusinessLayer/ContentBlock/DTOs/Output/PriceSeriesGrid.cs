namespace BusinessLayer.ContentBlock.DTOs.Output
{
    using System.Collections.Generic;

    public class PriceSeriesGrid
    {
        public required string Id { get; set; }

        public string? Title { get; set; }

        public List<string>? PriceSeriesIds { get; set; }

        public string? SeriesItemTypeCode { get; set; }
    }
}
