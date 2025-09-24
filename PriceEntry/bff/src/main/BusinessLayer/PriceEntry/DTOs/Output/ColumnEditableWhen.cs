namespace BusinessLayer.PriceEntry.DTOs.Output
{
    public record ColumnEditableWhen
    {
        public required string Field { get; init; }

        public required string Value { get; init; }
    }
}
