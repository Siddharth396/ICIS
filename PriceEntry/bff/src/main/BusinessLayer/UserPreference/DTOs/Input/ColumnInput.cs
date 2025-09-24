namespace BusinessLayer.UserPreference.DTOs.Input
{
    public class ColumnInput
    {
        public required string Field { get; set; } = default!;

        public required int DisplayOrder { get; set; } = default!;

        public required bool Hidden { get; set; } = default!;
    }
}
