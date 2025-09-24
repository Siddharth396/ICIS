namespace BusinessLayer.PriceDisplay.ContentBlock.DTOs.Output
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using BusinessLayer.PriceDisplay.ContentBlock.Repositories.Models;

    using HotChocolate;

    public class ContentBlockDefinitionForDisplay
    {
        public required string ContentBlockId { get; set; }

        public required int Version { get; set; }

        public string? Title { get; set; }

        public List<ColumnForDisplay>? Columns { get; set; }

        public List<RowForDisplay>? Rows { get; set; }

        public SelectedFilters? SelectedFilters { get; set; }

        public DateTime? AssessedDateTime { get; set; }

        [GraphQLIgnore]
        public bool IsValidForPriceSeries()
        {
           return Rows is not null &&
                  Rows.Count > 0 &&
                  Rows.Any(x => !string.IsNullOrWhiteSpace(x.PriceSeriesId));
        }
    }
}
