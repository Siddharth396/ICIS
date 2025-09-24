namespace BusinessLayer.PriceSeriesSelection.DTOs.Output
{
    using System;

    public class FilterDetail
    {
        public Guid? Id { get; set; }

        public required string Name { get; set; }

        public bool IsDefault { get; set; } = default!;
    }
}
