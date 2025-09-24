namespace BusinessLayer.PriceDisplay.ContentBlock.Repositories.Models
{
    using System.Collections.Generic;

    using Infrastructure.MongoDB.Models;

    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    [BsonIgnoreExtraElements]
    public class ContentBlockForDisplay
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public required string Id { get; set; }

        [BsonElement("content_block_id")]
        public required string ContentBlockId { get; set; }

        [BsonElement("version")]
        [BsonRepresentation(BsonType.Int32)]
        public required int Version { get; set; }

        [BsonElement("title")]
        public string? Title { get; set; }

        [BsonElement("last_modified")]
        public required AuditInfo LastModified { get; set; }

        [BsonElement("columns")]
        public List<ColumnForDisplay>? Columns { get; set; }

        [BsonElement("rows")]
        public List<RowForDisplay>? Rows { get; set; }

        [BsonElement("selected_filters")]
        public SelectedFilters? SelectedFilters { get; set; }
    }
}
