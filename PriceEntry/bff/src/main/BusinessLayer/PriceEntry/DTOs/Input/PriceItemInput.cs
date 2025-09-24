namespace BusinessLayer.PriceEntry.DTOs.Input
{
    using System;

    public class PriceItemInput
    {
        public required string SeriesId { get; set; }

        public required string SeriesItemTypeCode { get; set; }

        public required DateTime AssessedDateTime { get; set; }

        public required SeriesItem SeriesItem { get; set; }

        public string? OperationType { get; set; }
    }
}
