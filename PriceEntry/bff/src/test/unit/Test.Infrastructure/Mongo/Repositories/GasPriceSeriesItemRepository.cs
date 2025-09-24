namespace Test.Infrastructure.Mongo.Repositories
{
    using System.Threading.Tasks;

    using BusinessLayer.PriceEntry.Repositories.Models;

    using MongoDB.Driver;

    public class GasPriceSeriesItemRepository : Repository<GasPriceSeriesItem>
    {
        private const string CollectionName = BusinessLayer.PriceEntry.Repositories.GasPriceSeriesItemRepository.CollectionName;

        public GasPriceSeriesItemRepository(IMongoDatabase database)
            : base(database, CollectionName)
        {
        }

        public async Task InsertGasPrice(GasPriceSeriesItem gasPriceSeriesItem)
        {
            await InsertOneAsync(gasPriceSeriesItem);
        }
    }
}
