namespace BusinessLayer.UserPreference.DTOs.Output
{
    using System.Collections.Generic;

    public class UserPreference
    {
        public required string ContentBlockId { get; set; }

        public required string UserId { get; set; }

        public List<PriceSeriesGrid> PriceSeriesGrids { get; set; } = [];
    }
}
