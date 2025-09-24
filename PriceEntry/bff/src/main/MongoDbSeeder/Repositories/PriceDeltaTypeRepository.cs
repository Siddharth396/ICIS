namespace MongoDbSeeder.Repositories
{
    using System.Collections.Generic;

    using MongoDB.Bson;
    using MongoDB.Driver;

    using MongoDbSeeder.Data;

    public class PriceDeltaTypeRepository : Repository<BsonDocument>
    {
        private const string CollectionName = "price_delta_types";

        private static readonly IEnumerable<BsonDocument> PriceDeltaTypes = Data.LoadDocuments("price_delta_type.json");

        public PriceDeltaTypeRepository(IMongoDatabase database)
            : base(database, CollectionName)
        {
        }

        public void SeedCollection()
        {
            Insert(PriceDeltaTypes);
        }
    }
}