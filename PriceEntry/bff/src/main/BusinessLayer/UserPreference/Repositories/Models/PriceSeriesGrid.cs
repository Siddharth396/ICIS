namespace BusinessLayer.UserPreference.Repositories.Models
{
    using System.Collections.Generic;

    using MongoDB.Bson.Serialization.Attributes;

    [BsonIgnoreExtraElements]
    public class PriceSeriesGrid
    {
        [BsonElement("price_series_grid_id")]
        public required string PriceSeriesGridId { get; set; }

        [BsonElement("columns")]
        public required List<Column> Columns { get; set; }

        [BsonElement("price_series_ids")]
        public required List<string> PriceSeriesIds { get; set; }
    }
}
