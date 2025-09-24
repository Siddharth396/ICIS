namespace BusinessLayer.PriceEntry.Repositories.Models
{
    using MongoDB.Bson.Serialization.Attributes;

    [BsonIgnoreExtraElements]
    public class PriceSeriesLink
    {
        [BsonElement("object_series_id")]
        public required string ObjectSeriesId { get; set; }

        [BsonElement("series_link_reason_code")]
        public required string ReasonCode { get; set; }
    }
}
