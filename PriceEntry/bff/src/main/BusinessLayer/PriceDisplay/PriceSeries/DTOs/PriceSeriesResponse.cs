namespace BusinessLayer.PriceDisplay.PriceSeries.DTOs
{
    using BusinessLayer.PriceDisplay.PriceSeries.Repositories.Models;

    public class PriceSeriesResponse
    {
        public string? Id { get; set; }

        public string? UnitDisplay { get; set; } = default!;

        public string? PriceSeriesName { get; set; } = default!;

        public string? Period { get; set; } = default!;

        public string? SeriesItemTypeCode { get; set; } = default!;

        public string? SeriesId { get; set; } = default!;

        public string? ItemFrequencyName { get; set; } = default!;

        public string? AssessedDateTime { get; set; }

        public string? DataUsed { get; set; } = default!;

        public decimal? Price { get; set; } = default!;

        public decimal? PriceLow { get; set; } = default!;

        public decimal? PriceHigh { get; set; } = default!;

        public decimal? PriceMid { get; set; } = default!;

        public decimal? PriceDelta { get; set; } = default!;

        public string? AssessmentMethod { get; set; } = default!;

        public decimal? PriceLowDelta { get; set; } = default!;

        public decimal? PriceMidDelta { get; set; } = default!;

        public decimal? PriceHighDelta { get; set; } = default!;

        public string? Status { get; set; } = default!;

        public string? LastModifiedDate { get; set; } = default!;

        public PriceSeriesItemVersion? PreviousVersion { get; set; }

        public string PriceDeltaType { get; set; } = default!;
    }
}
