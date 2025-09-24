namespace BusinessLayer.DataPackage.Repositories.Models
{
    using MongoDB.Bson.Serialization.Attributes;

    public class WorkflowData
    {
        [BsonElement("id")]
        public required string Id { get; set; }

        [BsonElement("business_key")]
        public required string BusinessKey { get; set; }
    }
}
