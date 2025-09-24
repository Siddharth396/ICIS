namespace MongoDbSeeder
{
    using MongoDB.Driver;

    using MongoDbSeeder.Repositories;

    public static class DatabaseSeeder
    {
        public static bool ShouldSeederRun(IMongoDatabase database)
        {
            return new ConfigRepository(database).ShouldSeederRun();
        }

        public static void SeedAll(IMongoDatabase database)
        {
            new PriceSeriesRepository(database).SeedCollection();
        }
    }
}
