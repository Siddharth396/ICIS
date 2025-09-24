namespace BusinessLayer.PriceSeriesSelection.DTOs.Output
{
    public class PriceSeriesDetail
    {
        public required string Id { get; set; }

        public required string Name { get; set; }

        public string? ScheduleId { get; set; }
    }
}