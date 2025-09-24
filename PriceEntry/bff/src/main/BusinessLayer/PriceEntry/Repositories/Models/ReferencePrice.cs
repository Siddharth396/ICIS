namespace BusinessLayer.PriceEntry.Repositories.Models
{
    using System;

    using MongoDB.Bson.Serialization.Attributes;

    [BsonIgnoreExtraElements]
    public class ReferencePrice
    {
        [BsonElement("market")]
        public required string Market { get; set; }

        [BsonElement("price")]
        public decimal? Price { get; set; }

        [BsonElement("datetime")]
        public DateTime? Datetime { get; set; }

        [BsonElement("series_name")]
        public string? SeriesName { get; set; }

        [BsonElement("series_id")]
        public string? SeriesId { get; set; }

        [BsonElement("period_label")]
        public string? PeriodLabel { get; set; }
    }
}
