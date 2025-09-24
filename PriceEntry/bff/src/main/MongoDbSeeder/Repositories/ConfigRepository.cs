namespace MongoDbSeeder.Repositories
{
    using MongoDB.Driver;
    using MongoDB.Driver.Linq;

    using MongoDbSeeder.Models;

    public class ConfigRepository
    {
        private const string CollectionName = "config";
        private readonly IMongoCollection<Config> collection;

        public ConfigRepository(IMongoDatabase database)
        {
            collection = database.GetCollection<Config>(CollectionName);

            if (!database.ListCollectionNames().ToList().Contains(CollectionName))
            {
                CreateConfigCollection();
            }
        }

        public void CreateConfigCollection()
        {
            var config = new Config { RunSeeder = true };
            collection.InsertOne(config);
        }

        public bool ShouldSeederRun()
        {
            return collection.AsQueryable().Select(x => x.RunSeeder).FirstOrDefault();
        }
    }
}
