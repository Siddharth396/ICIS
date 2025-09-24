namespace MongoDbSeeder.Repositories.PriceDisplay
{
    using MongoDB.Bson;
    using MongoDB.Driver;

    using MongoDbSeeder.Data.PriceDisplay;

    using MongoDbSeeder.Repositories;

    public class GridConfigurationRepository : Repository<BsonDocument>
    {
        public const string CollectionName = "price_display.grid_configuration";

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
