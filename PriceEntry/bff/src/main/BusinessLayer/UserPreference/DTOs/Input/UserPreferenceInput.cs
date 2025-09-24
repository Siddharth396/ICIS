namespace BusinessLayer.UserPreference.DTOs.Input
{
    using System.Collections.Generic;

    public class UserPreferenceInput
    {
        public required string ContentBlockId { get; set; }

        public required List<ColumnInput> ColumnInput { get; set; }

        public required List<string> PriceSeriesInput { get; set; }

        public required string PriceSeriesGridId { get; set; }
    }
}
