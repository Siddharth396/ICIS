namespace BusinessLayer.PriceDisplay.ContentBlock.Repositories.Models
{
    using MongoDB.Bson.Serialization.Attributes;

    [BsonIgnoreExtraElements]
    public class RowForDisplay
    {
        [BsonElement("price_series_id")]
        public required string PriceSeriesId { get; set; }

        [BsonElement("display_order")]
        public int DisplayOrder { get; set; }

        [BsonElement("series_item_type_code")]
        public required string SeriesItemTypeCode { get; set; }
    }
}
