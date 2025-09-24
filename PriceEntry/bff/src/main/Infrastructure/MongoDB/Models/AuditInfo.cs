namespace Infrastructure.MongoDB.Models
{
    using System;

    using global::MongoDB.Bson.Serialization.Attributes;

    [BsonIgnoreExtraElements]
    public class AuditInfo
    {
        [BsonElement("timestamp")]
        public required DateTime Timestamp { get; set; }

        [BsonElement("user")]
        public required string User { get; set; }
    }
}
