namespace BusinessLayer.ContentBlock.DTOs.Input
{
    using System.Collections.Generic;

    public class ContentBlockInput
    {
        public required string ContentBlockId { get; set; }

        public string? Title { get; set; }

        public List<PriceSeriesGrid>? PriceSeriesGrids { get; set; }
    }
}