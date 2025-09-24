namespace BusinessLayer.DataPackage.Repositories.Models
{
    using MongoDB.Bson.Serialization.Attributes;

    public class Commentary
    {
        [BsonElement("commentary_id")]
        public required string CommentaryId { get; set; }

        [BsonElement("version")]
        public required string Version { get; set; }
    }
}
