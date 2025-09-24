namespace BusinessLayer.PriceDisplay.PriceSeries.Repositories.Models
{
    using System;
    using System.Collections.Generic;

    using Infrastructure.MongoDB.Models;

    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    [BsonIgnoreExtraElements]
    public class PriceSeriesItem
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string? Id { get; set; }

        [BsonElement("series_id")]
        public string SeriesId { get; set; } = default!;

        [BsonElement("data_used")]
        public string? DataUsed { get; set; } = default!;

        [BsonElement("price_value")]
        public decimal? Price { get; set; } = default!;

        [BsonElement("price_low")]
        public decimal? PriceLow { get; set; } = default!;

        [BsonElement("price_high")]
        public decimal? PriceHigh { get; set; } = default!;

        [BsonElement("price_mid")]
        public decimal? PriceMid { get; set; } = default!;

        [BsonElement("series_item_type_code")]
        public string SeriesItemTypeCode { get; set; } = default!;

        [BsonElement("price_delta")]
        public decimal? PriceDelta { get; set; }

        [BsonElement("adjusted_price_delta")]
        public decimal? AdjustedPriceDelta { get; set; }

        [BsonElement("price_low_delta")]
        public decimal? PriceLowDelta { get; set; }

        [BsonElement("adjusted_price_low_delta")]
        public decimal? AdjustedPriceLowDelta { get; set; }

        [BsonElement("price_high_delta")]
        public decimal? PriceHighDelta { get; set; }

        [BsonElement("adjusted_price_high_delta")]
        public decimal? AdjustedPriceHighDelta { get; set; }

        [BsonElement("price_mid_delta")]
        public decimal? PriceMidDelta { get; set; }

        [BsonElement("adjusted_price_mid_delta")]
        public decimal? AdjustedPriceMidDelta { get; set; }

        [BsonElement("status")]
        public string? Status { get; set; } = default!;

        [BsonElement("period_label")]
        public string? PeriodLabel { get; set; } = default!;

        [BsonElement("price_delta_type_guid")]
        public string? PriceDeltaTypeGuid { get; set; } = default!;

        [BsonElement("assessed_datetime")]
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime AssessedDateTime { get; set; } = default!;

        [BsonElement("last_modified")]
        public required AuditInfo LastModified { get; set; }

        [BsonElement("previous_versions")]
        public List<PriceSeriesItemVersion>? PreviousVersions { get; set; } = default!;
    }
}
