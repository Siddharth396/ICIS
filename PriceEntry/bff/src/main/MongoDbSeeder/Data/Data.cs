namespace MongoDbSeeder.Data
{
    using System.Collections.Generic;
    using System.Linq;

    using MongoDB.Bson;
    using MongoDB.Bson.Serialization;

    public static class Data
    {
        public static IEnumerable<BsonDocument> LoadDocuments(string fileName)
        {
            var json = JsonFileLoader.LoadFile(fileName);
            return BsonSerializer.Deserialize<BsonArray>(json)
              .Select(bsonValue => BsonDocument.Parse(bsonValue.ToJson()))
              .ToList();
        }
    }
}
