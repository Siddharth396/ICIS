namespace BusinessLayer.PriceEntry.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using BusinessLayer.PriceEntry.Repositories.Models;
    using BusinessLayer.PriceEntry.ValueObjects;

    using Infrastructure.MongoDB.Repositories;
    using Infrastructure.MongoDB.Transactions;

    using MongoDB.Driver;
    using MongoDB.Driver.Linq;

    public class ReferenceMarketRepository : BaseRepository<ReferenceMarket>
    {
        public const string CollectionName = "reference_markets";

        public ReferenceMarketRepository(IMongoDatabase database, IMongoContext mongoContext)
            : base(database, mongoContext)
        {
        }

        public override string DbCollectionName => CollectionName;

        public async Task<ReferenceMarket> GetReferenceMarket(string referenceMarketName)
        {
            return await Query().Where(x => x.Name == referenceMarketName).SingleAsync();
        }

        public async Task<List<ReferenceMarket>> GetReferenceMarketsByType(ReferenceMarketType referenceMarketType)
        {
            return await Query().Where(x => x.Type == referenceMarketType.Value).ToListAsync();
        }
    }
}
