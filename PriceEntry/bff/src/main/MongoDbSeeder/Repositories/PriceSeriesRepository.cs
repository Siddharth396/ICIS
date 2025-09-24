namespace MongoDbSeeder.Repositories
{
    using MongoDB.Bson;
    using MongoDB.Driver;

    using MongoDbSeeder.Data;

    public class PriceSeriesRepository : Repository<BsonDocument>
    {
        private const string CollectionName = "price_series";

        public PriceSeriesRepository(IMongoDatabase database)
            : base(database, CollectionName)
        {
        }

        public void SeedCollection()
        {
            RemoveDocuments();
            Insert(PriceSeriesData.LngJsonFile);
            Insert(PriceSeriesData.StyreneJsonFile);
            Insert(PriceSeriesData.MelamineJsonFile);
            Insert(PriceSeriesData.CharterRateJsonFile);
            Insert(PriceSeriesData.CausticSodaJsonFile);
        }
    }
}
