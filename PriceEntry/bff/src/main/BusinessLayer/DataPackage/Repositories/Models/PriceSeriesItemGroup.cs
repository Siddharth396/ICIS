namespace BusinessLayer.DataPackage.Repositories.Models
{
    using System.Collections.Generic;

    using MongoDB.Bson.Serialization.Attributes;

    [BsonIgnoreExtraElements]
    public class PriceSeriesItemGroup
    {
        [BsonElement("price_series_item_ids")]
        public required List<string> PriceSeriesItemIds { get; set; } = new();

        [BsonElement("series_item_type_code")]
        public required string SeriesItemTypeCode { get; set; }
    }
}
