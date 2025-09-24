namespace BusinessLayer.PriceDisplay.PriceSeriesSelection.DTOs.Output
{
    using System.Collections.Generic;

    using HotChocolate;

    [GraphQLName("PriceSeriesSelectionItemForDisplay")]
    public class PriceSeriesSelectionItem
    {
        public required IEnumerable<PriceSeriesDetail> PriceSeriesDetails { get; set; }

        public IEnumerable<DropdownFilterItem> Regions { get; set; } = default!;

        public IEnumerable<DropdownFilterItem> PriceCategories { get; set; } = default!;

        public IEnumerable<DropdownFilterItem> TransactionTypes { get; set; } = default!;

        public IEnumerable<DropdownFilterItem> AssessedFrequencies { get; set; } = default!;
    }
}
