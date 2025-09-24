namespace BusinessLayer.PriceDisplay.GridConfiguration.DTOs
{
    using System.Collections.Generic;

    using HotChocolate;

    [GraphQLName("PriceDisplayColumn")]
    public class Column
    {
        public string Field { get; set; } = default!;

        public string HeaderName { get; set; } = default!;

        public string CellDataType { get; set; } = default!;

        public int DisplayOrder { get; set; }

        public IList<string>? Values { get; set; } = [];

        public string? Pinned { get; set; }

        public string? Type { get; set; }

        public CustomConfig? CustomConfig { get; set; }

        public string? TooltipField { get; set; }

        public string? CellType { get; set; }

        public int ColumnOrder { get; set; }

        public bool Hideable { get; set; }

        public bool Hidden { get; set; }

        public int? MinWidth { get; set; }

        public int? MaxWidth { get; set; }

        public int? Width { get; set; }

        public IEnumerable<AlternateField> AlternateFields { get; init; } = [];

        public bool AutoSize { get; init; }
    }
}
