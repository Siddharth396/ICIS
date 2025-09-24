namespace Authoring.Application.ContentBlock.DataLoaders
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;

    using BusinessLayer.PriceEntry.DTOs.Output;
    using BusinessLayer.PriceEntry.Services;
    using BusinessLayer.PriceEntry.ValueObjects;

    using GreenDonut;

    using HotChocolate.Fetching;

    using Serilog;

    public class GridConfigurationBatchLoader : BatchDataLoader<SeriesItemTypeCode, GridConfiguration>
    {
        private readonly IAuthoringService priceEntryService;

        private readonly ILogger logger;

        public GridConfigurationBatchLoader(
            IAuthoringService priceEntryService,
            ILogger logger,
            BatchScheduler batchScheduler,
            DataLoaderOptions? options = null)
            : base(batchScheduler, options)
        {
            this.priceEntryService = priceEntryService;
            this.logger = logger.ForContext<GridConfigurationBatchLoader>();
        }

        protected override async Task<IReadOnlyDictionary<SeriesItemTypeCode, GridConfiguration>> LoadBatchAsync(
            IReadOnlyList<SeriesItemTypeCode> seriesItemTypeCodes,
            CancellationToken cancellationToken)
        {
            logger.Debug($"Loading batch of grid configurations for {seriesItemTypeCodes.Count} rows", seriesItemTypeCodes.Count);

            var result = new Dictionary<SeriesItemTypeCode, GridConfiguration>();

            var gridConfigurations = await priceEntryService.GetGridConfigurations(seriesItemTypeCodes);

            logger.Debug($"Loaded {gridConfigurations.Count} grid configurations for series item type codes '{JsonSerializer.Serialize(seriesItemTypeCodes)}')");

            foreach (var seriesItemTypeCode in seriesItemTypeCodes)
            {
                var gridConfiguration = gridConfigurations.SingleOrDefault(x => x.SeriesItemTypeCode == seriesItemTypeCode.Value);

                var configuration = gridConfiguration
                                    ?? new GridConfiguration
                                    {
                                        SeriesItemTypeCode = seriesItemTypeCode.Value,
                                        Columns = [],
                                        Sort = new Sort { Name = string.Empty, Order = string.Empty }
                                    };
                result.Add(seriesItemTypeCode, configuration);
            }

            logger.Debug($"Finished loading batch of periods for {seriesItemTypeCodes.Count} rows. Returning {result.Count} results");

            return result;
        }
    }
}
