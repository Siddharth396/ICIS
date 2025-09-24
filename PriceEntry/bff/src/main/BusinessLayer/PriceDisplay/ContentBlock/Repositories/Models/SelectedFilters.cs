namespace BusinessLayer.PriceDisplay.ContentBlock.Repositories.Models
{
    using System.Collections.Generic;

    using MongoDB.Bson.Serialization.Attributes;

    [BsonIgnoreExtraElements]
    public class SelectedFilters
    {
        [BsonElement("is_inactive_included")]
        public bool IsInactiveIncluded { get; set; }

        [BsonElement("selected_commodities")]
        public required List<string> SelectedCommodities { get; set; }

        [BsonElement("selected_regions")]
        public List<string>? SelectedRegions { get; set; }

        [BsonElement("selected_price_categories")]
        public List<string>? SelectedPriceCategories { get; set; }

        [BsonElement("selected_item_frequencies")]
        public List<string>? SelectedAssessedFrequencies { get; set; }

        [BsonElement("selected_price_settlement_types")]
        public List<string>? SelectedTransactionTypes { get; set; }
    }
}
