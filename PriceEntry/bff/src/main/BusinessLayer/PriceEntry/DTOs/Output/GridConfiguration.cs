namespace BusinessLayer.PriceEntry.DTOs.Output
{
    using System.Collections.Generic;

    public record GridConfiguration
    {
        public required IEnumerable<Column> Columns { get; init; }

        public required Sort Sort { get; init; }

        public required string SeriesItemTypeCode { get; init; }
    }
}
