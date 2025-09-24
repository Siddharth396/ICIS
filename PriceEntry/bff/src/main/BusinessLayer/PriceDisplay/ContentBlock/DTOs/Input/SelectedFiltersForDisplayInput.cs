namespace BusinessLayer.PriceDisplay.ContentBlock.DTOs.Input
{
    using System.Collections.Generic;

    public class SelectedFiltersForDisplayInput
    {
        public bool IsInactiveIncluded { get; set; }

        public required List<string> SelectedCommodities { get; set; }

        public List<string>? SelectedRegions { get; set; }

        public List<string>? SelectedPriceCategories { get; set; }

        public List<string>? SelectedAssessedFrequencies { get; set; }

        public List<string>? SelectedTransactionTypes { get; set; }
    }
}
