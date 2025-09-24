namespace BusinessLayer.UserPreference.DTOs.Output
{
    using System.Collections.Generic;

    public class PriceSeriesGrid
    {
        public required string Id { get; set; }

        public required List<Column> Columns { get; set; }

        public required List<string> PriceSeriesIds { get; set; }
    }
}
