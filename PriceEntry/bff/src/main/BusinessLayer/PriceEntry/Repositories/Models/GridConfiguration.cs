namespace BusinessLayer.PriceEntry.Repositories.Models
{
    using System.Collections.Generic;

    using MongoDB.Bson.Serialization.Attributes;

    [BsonIgnoreExtraElements]
    public class GridConfiguration
    {
        [BsonElement("series_item_type_code")]
        public string SeriesItemTypeCode { get; set; } = default!;

        [BsonElement("sort")]
        public Sort Sort { get; set; } = default!;

        [BsonElement("columns")]
        public IList<Column> Columns { get; set; } = default!;
    }
}
