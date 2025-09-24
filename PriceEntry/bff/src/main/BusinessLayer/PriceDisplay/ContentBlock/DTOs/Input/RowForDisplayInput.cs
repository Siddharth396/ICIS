namespace BusinessLayer.PriceDisplay.ContentBlock.DTOs.Input
{
    public class RowForDisplayInput
    {
        public required string PriceSeriesId { get; set; }

        public int DisplayOrder { get; set; } = default!;

        public required string SeriesItemTypeCode { get; set; }
    }
}
