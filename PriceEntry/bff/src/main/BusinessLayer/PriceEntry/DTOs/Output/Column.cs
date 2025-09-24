namespace BusinessLayer.PriceEntry.DTOs.Output
{
    using System.Collections.Generic;

    public record Column
    {
        public required string Field { get; init; }

        public required string HeaderName { get; init; }

        public required string CellDataType { get; init; }

        public required int DisplayOrder { get; init; }

        public required bool Editable { get; init; }

        public IList<string>? Values { get; init; }

        public string? Pinned { get; init; }

        public ColumnEditableWhen? EditableWhen { get; init; }

        public string? Type { get; init; }

        public CustomConfig? CustomConfig { get; init; }

        public string? TooltipField { get; init; }

        public string? CellType { get; init; }

        public required int ColumnOrder { get; init; }

        public required bool Hideable { get; init; }

        public required bool Hidden { get; init; }

        public bool? ForCorrectionWorkflow { get; init; }
    }
}
