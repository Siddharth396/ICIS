namespace BusinessLayer.PriceEntry.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using BusinessLayer.PriceEntry.Repositories.Models;

    using Infrastructure.MongoDB.Repositories;
    using Infrastructure.MongoDB.Transactions;

    using MongoDB.Bson;
    using MongoDB.Driver;

    public class PriceEntryRepository : BaseRepository<PriceSeriesAggregation>
    {
        public const string CollectionName = "price_series";

        public PriceEntryRepository(IMongoDatabase database, IMongoContext mongoContext)
            : base(database, mongoContext)
        {
        }

        public override string DbCollectionName => CollectionName;

        public async Task<List<PriceSeriesAggregation>> GetPriceSeriesDetails(IEnumerable<string>? priceSeriesIds, DateTime assessedDateTime)
        {
            var priceSeriesPipelineDefinition = GetPriceSeriesPipelineDefinition(priceSeriesIds, assessedDateTime);
            return await Aggregate(priceSeriesPipelineDefinition).ToListAsync();
        }

        private BsonDocument[] GetPriceSeriesPipelineDefinition(IEnumerable<string>? priceSeriesIds, DateTime assessedDateTime)
        {
            return new[]
            {
                PriceEntryPipelineBuilder.FilterPriceSeries(priceSeriesIds, assessedDateTime),
                PriceEntryPipelineBuilder.FilterPriceSeriesItem(assessedDateTime),
                PriceEntryPipelineBuilder.Unwind("$price_series_item"),
                PriceEntryPipelineBuilder.HandlePendingChanges(),
                PriceEntryPipelineBuilder.Unwind("$price_series_link"),
                PriceEntryPipelineBuilder.IncludeLastAssessments(assessedDateTime),
            };
        }
    }
}
