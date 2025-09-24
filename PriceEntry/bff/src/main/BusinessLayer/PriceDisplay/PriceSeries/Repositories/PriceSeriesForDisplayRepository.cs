namespace BusinessLayer.PriceDisplay.PriceSeries.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using BusinessLayer.PriceDisplay.PriceSeries.Repositories.Models;

    using Infrastructure.MongoDB.Repositories;
    using Infrastructure.MongoDB.Transactions;

    using MongoDB.Bson;
    using MongoDB.Driver;

    public class PriceSeriesForDisplayRepository : BaseRepository<PriceSeriesAggregation>
    {
        public const string CollectionName = "price_series";

        public PriceSeriesForDisplayRepository(IMongoDatabase database, IMongoContext mongoContext)
            : base(database, mongoContext)
        {
        }

        public override string DbCollectionName => CollectionName;

        public async Task<List<PriceSeriesAggregation>> GetCurrentOrPendingPriceSeriesDetails(
            IEnumerable<string> seriesIds,
            DateTime assessedDateTime)
        {
            var priceSeriesItemPipelineDefinition = GetCurrentOrPendingPriceSeriesItemPipelineDefinitionForAuthoring(seriesIds, PriceSeriesItemPipelineBuilder.GetPriceSeries(assessedDateTime));
            return await Aggregate(priceSeriesItemPipelineDefinition).ToListAsync();
        }

        public async Task<List<PriceSeriesAggregation>> GetPriceSeriesItemDetail(
           IEnumerable<string> seriesIds)
        {
            var priceSeriesItemPipelineDefinition = GetPriceSeriesItemPipelineDefinitionForAuthoring(seriesIds, PriceSeriesItemPipelineBuilder.GetPriceSeries());
            return await Aggregate(priceSeriesItemPipelineDefinition).ToListAsync();
        }

        public async Task<List<PriceSeriesAggregation>> GetPublishedPriceSeriesItemDetail(
          IEnumerable<string> seriesIds,
          DateTime assessedDateTime,
          DateTime launchDate)
        {
            var priceSeriesItemPipelineDefinition = GetPriceSeriesItemPipelineDefinitionForSubscriber(seriesIds, PriceSeriesItemPipelineBuilder.GetPublishedPriceSeries(assessedDateTime), launchDate);
            return await Aggregate(priceSeriesItemPipelineDefinition).ToListAsync();
        }

        private static BsonDocument[] GetPriceSeriesItemPipelineDefinitionForAuthoring(IEnumerable<string> seriesIds, BsonDocument latestPriceSeries)
        {
            return
            [
                PriceSeriesItemPipelineBuilder.FilterPriceSeriesByIds(seriesIds),
                latestPriceSeries,
                PriceSeriesItemPipelineBuilder.Unwind("$price_series_item"),
                PriceSeriesItemPipelineBuilder.GetPriceDeltaType(latestPriceSeries),
                PriceSeriesItemPipelineBuilder.Unwind("$delta_type"),
            ];
        }

        private static BsonDocument[] GetCurrentOrPendingPriceSeriesItemPipelineDefinitionForAuthoring(IEnumerable<string> seriesIds, BsonDocument latestPriceSeries)
        {
            return
            [
                PriceSeriesItemPipelineBuilder.FilterPriceSeriesByIds(seriesIds),
                latestPriceSeries,
                PriceSeriesItemPipelineBuilder.Unwind("$price_series_item"),
                PriceSeriesItemPipelineBuilder.HandlePendingChanges()
            ];
        }

        private static BsonDocument[] GetPriceSeriesItemPipelineDefinitionForSubscriber(IEnumerable<string> seriesIds, BsonDocument publishedPriceSeries, DateTime launchDate)
        {
            return
            [
                PriceSeriesItemPipelineBuilder.FilterPriceSeriesByIdsAndLaunchDate(seriesIds, launchDate),
                publishedPriceSeries,
                PriceSeriesItemPipelineBuilder.Unwind("$price_series_item"),
                PriceSeriesItemPipelineBuilder.GetPriceDeltaType(publishedPriceSeries),
                PriceSeriesItemPipelineBuilder.Unwind("$delta_type")
            ];
        }
    }
}
