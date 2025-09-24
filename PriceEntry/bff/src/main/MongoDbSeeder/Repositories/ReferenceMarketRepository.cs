namespace MongoDbSeeder.Repositories
{
    using System.Collections.Generic;

    using MongoDB.Bson;
    using MongoDB.Driver;

    using MongoDbSeeder.Data;

    public class ReferenceMarketRepository : Repository<BsonDocument>
    {
        private const string CollectionName = "reference_markets";

        private static readonly IEnumerable<BsonDocument> ReferenceMarkets = Data.LoadDocuments("reference_markets.json");

        public ReferenceMarketRepository(IMongoDatabase database)
           : base(database, CollectionName)
        {
        }

        public void SeedCollection()
        {
            Insert(ReferenceMarkets);
        }
    }
}
