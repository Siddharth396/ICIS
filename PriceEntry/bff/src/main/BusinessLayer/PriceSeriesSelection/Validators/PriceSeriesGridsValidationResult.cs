namespace BusinessLayer.PriceSeriesSelection.Validators
{
    using System.Collections.Generic;

    using BusinessLayer.ContentBlock.Repositories.Models;
    using BusinessLayer.PriceEntry.ValueObjects;

    public class PriceSeriesGridsValidationResult
    {
        public bool IsValid => ErrorCodes.Count == 0;

        public SeriesItemTypeCode? SeriesItemTypeCode { get; set; }

        public List<string> ErrorCodes { get; } = new();

        public List<PriceSeriesGrid> PriceSeriesGrids { get; set; } = new();
    }
}
