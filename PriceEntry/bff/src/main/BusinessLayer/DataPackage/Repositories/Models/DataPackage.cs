namespace BusinessLayer.DataPackage.Repositories.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Infrastructure.MongoDB.Models;

    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    [BsonIgnoreExtraElements]
    public class DataPackage
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public required string Id { get; set; }

        [BsonElement("content_block")]
        public required ContentBlock ContentBlock { get; set; }

        [BsonElement("assessed_datetime")]
        public required DateTime AssessedDateTime { get; set; }

        [BsonElement("price_series_item_groups")]
        public required List<PriceSeriesItemGroup> PriceSeriesItemGroups { get; set; } = new();

        [BsonElement("status")]
        public required string Status { get; set; }

        [BsonElement("workflow_data")]
        public WorkflowData? WorkflowData { get; set; }

        [BsonElement("commentary")]
        public Commentary? Commentary { get; set; }

        [BsonElement("last_modified")]
        public required AuditInfo LastModified { get; set; }

        [BsonElement("created")]
        public required AuditInfo Created { get; set; }

        [BsonElement("pending_changes")]
        public DataPackage? PendingChanges { get; set; }

        [BsonElement("sequence_id")]
        public required string SequenceId { get; set; }

        public DataPackage PendingChangesOrOriginal()
        {
            return PendingChanges ?? this;
        }

        public List<string> GetPriceSeriesItemIds()
        {
            return PriceSeriesItemGroups.SelectMany(psig => psig.PriceSeriesItemIds).ToList();
        }
    }
}