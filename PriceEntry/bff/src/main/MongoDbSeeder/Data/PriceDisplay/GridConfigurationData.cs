namespace MongoDbSeeder.Data.PriceDisplay
{
    using System.Collections.Generic;

    using MongoDB.Bson;

    public static class GridConfigurationData
    {
        public static readonly IEnumerable<BsonDocument> GridConfigurationFile = Data.LoadDocuments("PriceDisplay/grid_configuration.json");
    }
}
