namespace BusinessLayer.DataPackage.Repositories.Models
{
    using MongoDB.Bson.Serialization.Attributes;

    public class ContentBlock
    {
        [BsonElement("id")]
        public required string Id { get; set; }

        [BsonElement("version")]
        public required int Version { get; set; }
    }
}
