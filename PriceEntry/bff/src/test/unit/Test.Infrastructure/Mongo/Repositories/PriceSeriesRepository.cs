namespace Test.Infrastructure.Mongo.Repositories
{
    using System;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    using BusinessLayer.PriceSeriesSelection.Repositories.Models;

    using MongoDB.Driver;
    using MongoDB.Driver.Linq;

    public class PriceSeriesRepository
    {
        private static readonly string CollectionName = global::BusinessLayer.PriceSeriesSelection.Repositories
           .PriceSeriesRepository.CollectionName;

        private readonly IMongoDatabase database;

        public PriceSeriesRepository(IMongoDatabase database)
        {
            this.database = database;
        }

        public Task<PriceSeries> GetPriceSeries(string seriesId)
        {
            return database.GetCollection<PriceSeries>(CollectionName).AsQueryable().SingleOrDefaultAsync(x => x.Id == seriesId);
        }

        public Task SetTerminationDate(string seriesId, DateTime? terminationDate)
        {
            var filter = Builders<PriceSeries>.Filter.Eq(x => x.Id, seriesId);
            var update = Builders<PriceSeries>.Update.Set(x => x.TerminationDate, terminationDate);
            return database.GetCollection<PriceSeries>(CollectionName).UpdateOneAsync(filter, update);
        }

        public Task PatchPriceSeries(string seriesId, Expression<Func<PriceSeries, object>> field, object? value)
        {
            var filter = Builders<PriceSeries>.Filter.Eq(x => x.Id, seriesId);
            var update = Builders<PriceSeries>.Update.Set(field!, value);
            return database.GetCollection<PriceSeries>(CollectionName).UpdateOneAsync(filter, update);
        }

        public Task PatchPriceSeries(string seriesId, string fieldName, object? value)
        {
            var filter = Builders<PriceSeries>.Filter.Eq(x => x.Id, seriesId);
            var update = Builders<PriceSeries>.Update.Set(fieldName, value);
            return database.GetCollection<PriceSeries>(CollectionName).UpdateOneAsync(filter, update);
        }
    }
}
