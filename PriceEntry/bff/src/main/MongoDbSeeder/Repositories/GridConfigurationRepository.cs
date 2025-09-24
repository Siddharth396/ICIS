namespace MongoDbSeeder.Repositories
{
    using MongoDB.Bson;
    using MongoDB.Driver;

    using MongoDbSeeder.Data;

    public class GridConfigurationRepository : Repository<BsonDocument>
    {
        private const string CollectionName = "grid_configuration";

        public GridConfigurationRepository(IMongoDatabase database)
           : base(database, CollectionName)
        {
        }

        public void SeedCollection()
        {
            RemoveDocuments();
            Insert(GridConfigurationData.GridConfigurationFile);
        }
    }
}
