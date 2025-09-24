namespace BusinessLayer.Commentary.Repositories.Models
{
    using System;

    using HotChocolate;

    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    [BsonIgnoreExtraElements]
    public class Commentary
    {
        public const string DefaultVersion = "0.0";

        [BsonElement]
        [BsonRepresentation(BsonType.String)]
        public required string Id { get; set; }

        [BsonElement("content_block_id")]
        public required string ContentBlockId { get; set; }

        [BsonElement("commentary_id")]
        public required string CommentaryId { get; set; }

        [BsonElement("version")]
        public required string Version { get; set; }

        [BsonElement("assessed_datetime")]
        public DateTime AssessedDateTime { get; set; }

        [BsonElement("pending_changes")]
        [GraphQLIgnore]
        public Commentary? PendingChanges { get; set; }

        [GraphQLIgnore]
        public Commentary PendingChangesOrOriginal()
        {
            return PendingChanges ?? this;
        }
    }
}
