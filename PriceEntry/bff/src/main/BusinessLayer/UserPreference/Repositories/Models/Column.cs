namespace BusinessLayer.UserPreference.Repositories.Models
{
    using MongoDB.Bson.Serialization.Attributes;

    [BsonIgnoreExtraElements]
    public class Column
    {
        [BsonElement("field")]
        public required string Field { get; set; }

        [BsonElement("display_order")]
        public required int DisplayOrder { get; set; }

        [BsonElement("hidden")]
        public required bool Hidden { get; set; }
    }
}
