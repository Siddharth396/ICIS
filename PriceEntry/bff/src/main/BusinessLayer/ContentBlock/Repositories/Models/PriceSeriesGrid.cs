namespace BusinessLayer.ContentBlock.Repositories.Models
{
    using System.Collections.Generic;

    using MongoDB.Bson.Serialization.Attributes;

    [BsonIgnoreExtraElements]
    public class PriceSeriesGrid
    {
        [BsonElement("price_series_grid_id")]
        public required string PriceSeriesGridId { get; set; }

        [BsonElement("title")]
        public string? Title { get; set; }

        [BsonElement("price_series_ids")]
        public List<string>? PriceSeriesIds { get; set; }

        [BsonElement("series_item_type_code")]
        public string? SeriesItemTypeCode { get; set; } = default!;
    }
}
