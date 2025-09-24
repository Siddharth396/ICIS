namespace BusinessLayer.PriceDisplay.ContentBlock.DTOs.Input
{
    public class ColumnForDisplayInput
    {
        public required string Field { get; set; }

        public required int DisplayOrder { get; set; }

        public required bool Hidden { get; set; }
    }
}
