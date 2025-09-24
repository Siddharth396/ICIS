namespace BusinessLayer.PriceEntry.DTOs.Output
{
    using System;

    public class ReferencePrice
    {
        public required string Market { get; set; }

        public decimal? Price { get; set; }

        public DateTime? Datetime { get; set; }

        public string? SeriesName { get; set; }

        public string? PeriodLabel { get; set; }
    }
}
