namespace MongoDbSeeder.Data
{
    using System.Collections.Generic;

    using MongoDB.Bson;

    public static class GridConfigurationData
    {
        public static readonly IEnumerable<BsonDocument> GridConfigurationFile = Data.LoadDocuments("grid_configuration.json");
    }
}
