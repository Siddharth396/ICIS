namespace BusinessLayer.PriceEntry.DTOs.Output
{
    public record Sort
    {
        public required string Name { get; init; }

        public required string Order { get; set; }
    }
}
