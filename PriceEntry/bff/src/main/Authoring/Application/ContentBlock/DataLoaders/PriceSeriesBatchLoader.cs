namespace Authoring.Application.ContentBlock.DataLoaders
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;

    using Authoring.Application.ContentBlock.DataLoaders.Keys;

    using BusinessLayer.PriceEntry.Repositories.Models;
    using BusinessLayer.PriceEntry.Services;

    using GreenDonut;

    using Serilog;

    public class PriceSeriesBatchLoader : BatchDataLoader<PriceSeriesBatchKey, List<PriceSeriesAggregation>>
    {
        private readonly IAuthoringService priceEntryService;

        private readonly ILogger logger;

        public PriceSeriesBatchLoader(
            IAuthoringService priceEntryService,
            ILogger logger,
            IBatchScheduler batchScheduler,
            DataLoaderOptions? options = null)
            : base(batchScheduler, options)
        {
            this.priceEntryService = priceEntryService;
            this.logger = logger.ForContext<PriceSeriesBatchLoader>();
        }

        protected override async Task<IReadOnlyDictionary<PriceSeriesBatchKey, List<PriceSeriesAggregation>>> LoadBatchAsync(
            IReadOnlyList<PriceSeriesBatchKey> keys,
            CancellationToken cancellationToken)
        {
            logger.Debug($"Loading batch of price series for {keys.Count} rows", keys.Count);

            var priceSeriesIdsByDate = keys
                .GroupBy(k => k.AssessedDateTime)
                .Select(g => (g.Key, g.SelectMany(k => k.PriceSeriesIds).ToList()))
                .ToList();

            var result = new Dictionary<PriceSeriesBatchKey, List<PriceSeriesAggregation>>();

            foreach (var (assessedDateTime, priceSeriesIds) in priceSeriesIdsByDate)
            {
                var priceSeries = await priceEntryService.GetPriceSeriesFromAggregation(
                    priceSeriesIds,
                    assessedDateTime);

                logger.Debug($"Loaded {priceSeries.Count} price series for applies from date {assessedDateTime} and price series ids '{JsonSerializer.Serialize(priceSeriesIds)}'");

                foreach (var key in keys.Where(k => k.AssessedDateTime == assessedDateTime))
                {
                    if (!result.TryGetValue(key, out _))
                    {
                        result[key] = [];
                    }

                    var matchingPriceSeries = priceSeries.Where(ps => key.PriceSeriesIds.Contains(ps.Id)).ToList();

                    if (matchingPriceSeries.Count > 0)
                    {
                        result[key].AddRange(matchingPriceSeries);
                    }
                }
            }

            logger.Debug($"Finished loading batch of price series for {keys.Count} rows. Returning {result.Count} results");

            return result;
        }
    }
}
