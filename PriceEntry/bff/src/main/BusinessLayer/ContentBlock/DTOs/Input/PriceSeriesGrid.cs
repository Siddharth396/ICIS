namespace BusinessLayer.ContentBlock.DTOs.Input
{
    using System.Collections.Generic;

    public class PriceSeriesGrid
    {
        public string? Id { get; set; }

        public string? Title { get; set; }

        public List<string>? PriceSeriesIds { get; set; }
    }
}
