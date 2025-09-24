namespace BusinessLayer.PriceDisplay.ContentBlock.DTOs.Input
{
    using System.Collections.Generic;

    public class ContentBlockForDisplayInput
    {
        public required string ContentBlockId { get; set; }

        public string? Title { get; set; }

        public List<ColumnForDisplayInput>? Columns { get; set; }

        public List<RowForDisplayInput>? Rows { get; set; }

        public SelectedFiltersForDisplayInput? SelectedFilters { get; set; }
    }
}
