namespace Authoring.Application.ContentBlock.DataLoaders
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Authoring.Application.ContentBlock.DataLoaders.Keys;

    using BusinessLayer.PriceEntry.Services;

    using GreenDonut;

    using Serilog;

    public class PriceSeriesEditabilityBatchLoader : BatchDataLoader<PriceSeriesEditabilityBatchKey, bool>
    {
        private readonly IPriceSeriesItemEditabilityService priceSeriesItemEditabilityService;

        private readonly ILogger logger;

        public PriceSeriesEditabilityBatchLoader(
            IPriceSeriesItemEditabilityService priceSeriesItemEditabilityService,
            ILogger logger,
            IBatchScheduler batchScheduler,
            DataLoaderOptions? options = null)
            : base(batchScheduler, options)
        {
            this.priceSeriesItemEditabilityService = priceSeriesItemEditabilityService;
            this.logger = logger.ForContext<PriceSeriesEditabilityBatchLoader>();
        }

        protected override async Task<IReadOnlyDictionary<PriceSeriesEditabilityBatchKey, bool>> LoadBatchAsync(
            IReadOnlyList<PriceSeriesEditabilityBatchKey> keys,
            CancellationToken cancellationToken)
        {
            logger.Debug($"Loading batch of price series editability for {keys.Count} rows", keys.Count);

            var result = new Dictionary<PriceSeriesEditabilityBatchKey, bool>();

            foreach (var key in keys)
            {
                var isPriceSeriesItemEditable = await priceSeriesItemEditabilityService.IsPriceSeriesItemEditable(
                                                    key.DataPackageKey,
                                                    key.IsReviewMode,
                                                    key.PriceSeriesItemStatus);

                result.Add(key, isPriceSeriesItemEditable);
            }

            logger.Debug($"Finished loading batch of price series editability for {keys.Count} rows. Returning {result.Count} results");

            return result;
        }
    }
}
