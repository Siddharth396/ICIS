namespace Authoring.Application.ContentBlock.DataLoaders
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Authoring.Application.ContentBlock.DataLoaders.Keys;

    using BusinessLayer.ImpactedPrices.Services;

    using GreenDonut;

    using Serilog;

    public class PriceSeriesHasImpactedPricesBatchDataLoader : BatchDataLoader<HasImpactedPricesBatchKey, bool>
    {
        private readonly ILogger logger;

        private readonly IImpactedPricesService impactedPricesService;

        public PriceSeriesHasImpactedPricesBatchDataLoader(
            IImpactedPricesService impactedPricesService,
            IBatchScheduler batchScheduler,
            ILogger logger,
            DataLoaderOptions? options = null)
            : base(batchScheduler, options)
        {
            this.impactedPricesService = impactedPricesService;
            this.logger = logger.ForContext<PriceSeriesHasImpactedPricesBatchDataLoader>();
        }

        protected override async Task<IReadOnlyDictionary<HasImpactedPricesBatchKey, bool>> LoadBatchAsync(
            IReadOnlyList<HasImpactedPricesBatchKey> keys,
            CancellationToken cancellationToken)
        {
            logger.Debug($"Loading batch of price series hasImpactedPrices for {keys.Count} rows", keys.Count);

            var priceSeriesIds = keys.Select(x => x.PriceSeriesId).Distinct().ToList();
            var assessedDateTime = keys.First().AssessedDateTime;

            var hasImpactedPricesDict = await impactedPricesService.HasImpactedPrices(priceSeriesIds, assessedDateTime);

            var result = keys.ToDictionary(key => key, key => hasImpactedPricesDict[key.PriceSeriesId]);

            logger.Debug($"Finished loading batch of price series hasImpactedPrices for {keys.Count} rows. Returning {result.Count} results");

            return result;
        }
    }
}
