namespace Test.Infrastructure.Services
{
    using System.Threading.Tasks;

    using BusinessLayer.PriceEntry.Repositories.Models;

    using MongoDB.Driver;

    using Test.Infrastructure.Mongo.Repositories;

    public class GasPriceSeriesItemTestService
    {
        private readonly IMongoDatabase mongoDatabase;

        public GasPriceSeriesItemTestService(IMongoDatabase mongoDatabase)
        {
            this.mongoDatabase = mongoDatabase;
        }

        public async Task SaveGasPrice(GasPriceSeriesItem gasPriceSeriesItem)
        {
            await new GasPriceSeriesItemRepository(mongoDatabase).InsertGasPrice(gasPriceSeriesItem);
        }
    }
}
