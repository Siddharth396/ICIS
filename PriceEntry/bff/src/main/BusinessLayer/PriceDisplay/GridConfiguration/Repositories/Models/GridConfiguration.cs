namespace BusinessLayer.PriceDisplay.GridConfiguration.Repositories.Models
{
    using System.Collections.Generic;

    using MongoDB.Bson.Serialization.Attributes;

    [BsonIgnoreExtraElements]
    public class GridConfiguration
    {
        [BsonElement("series_item_type_codes")]
        public IEnumerable<string> SeriesItemTypeCodes { get; set; } = [];

        [BsonElement("sort")]
        public Sort? Sort { get; set; }

        [BsonElement("columns")]
        public IList<Column> Columns { get; set; } = [];
    }
}
