namespace MongoDbSeeder.Repositories
{
    using System.Collections.Generic;

    using MongoDB.Bson;
    using MongoDB.Driver;

    using MongoDbSeeder.Data;

    public class GasPriceSeriesItemRepository : Repository<BsonDocument>
    {
        private const string CollectionName = "gas_price_series_items";

        private static readonly IEnumerable<BsonDocument> GasPriceSeriesItems = Data.LoadDocuments("gas_price_series_items.json");

        public GasPriceSeriesItemRepository(IMongoDatabase database)
           : base(database, CollectionName)
        {
        }

        public void SeedCollection()
        {
            Insert(GasPriceSeriesItems);
        }
    }
}
