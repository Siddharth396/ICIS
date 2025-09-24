namespace MongoDbSeeder.Models
{
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    public class Config
    {
        [BsonElement("run_seeder")]
        [BsonRepresentation(BsonType.Boolean)]
        public bool RunSeeder { get; set; }
    }
}
