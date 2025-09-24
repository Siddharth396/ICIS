namespace BusinessLayer.PriceEntry.Repositories.Models
{
    using System;
    using System.Collections.Generic;

    using HotChocolate;

    using Infrastructure.MongoDB.Models;

    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    [BsonKnownTypes(
        typeof(PriceItemSingleValueWithReference),
        typeof(PriceItemRange),
        typeof(PriceItemSingleValue),
        typeof(CharterRatePriceItemSingleValue),
        typeof(ShippingCostPriceItemSingleValue))]
    [BsonIgnoreExtraElements]
    public abstract class BasePriceItem
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string Id { get; set; } = default!;

        [BsonElement("series_id")]
        public string SeriesId { get; set; } = default!;

        [BsonElement("assessed_datetime")]
        public DateTime AssessedDateTime { get; set; } = default!;

        [BsonElement("status")]
        public string? Status { get; set; } = default!;

        [BsonElement("period_label")]
        public string? PeriodLabel { get; set; } = default!;

        [BsonElement("fulfilment_from_datetime")]
        public DateTime? FulfilmentFromDate { get; set; }

        [BsonElement("fulfilment_until_datetime")]
        public DateTime? FulfilmentUntilDate { get; set; }

        [BsonElement("last_modified")]
        public AuditInfo LastModified { get; set; } = default!;

        [BsonElement("created")]
        public AuditInfo Created { get; set; } = default!;

        [BsonElement("release_datetime")]
        public DateTime? ReleaseDate { get; set; }

        [BsonElement("price_delta_type_guid")]
        public string? PriceDeltaTypeGuid { get; set; } = default!;

        [BsonElement("pending_changes")]
        public BasePriceItem? PendingChanges { get; set; }

        [BsonElement("previous_versions")]
        public List<BasePriceItem>? PreviousVersions { get; set; }

        [BsonElement("series_item_id")]
        public string SeriesItemId { get; set; } = default!;

        [BsonElement("dlh_record_source")]
        public string? DlhRecordSource { get; set; }

        [BsonElement("applies_from_datetime")]
        public DateTime? AppliesFromDateTime { get; set; } = default!;

        [BsonElement("applies_until_datetime")]
        public DateTime? AppliesUntilDateTime { get; set; } = default!;

        [GraphQLIgnore]
        public BasePriceItem PendingChangesOrOriginal()
        {
            return PendingChanges ?? this;
        }

        [GraphQLIgnore]
        public void CancelNonMarketAdjustment(AuditInfo auditInfo, string priceDeltaTypeGuid)
        {
            LastModified = auditInfo;
            PriceDeltaTypeGuid = priceDeltaTypeGuid;
            CancelNonMarketAdjustment();
        }

        [GraphQLIgnore]
        public abstract decimal? GetPriceValue();

        [GraphQLIgnore]
        protected abstract void CancelNonMarketAdjustment();
    }
}