namespace BusinessLayer.UserPreference.DTOs.Output
{
    public class Column
    {
        public required string Field { get; set; }

        public required int DisplayOrder { get; set; }

        public required bool Hidden { get; set; }
    }
}
