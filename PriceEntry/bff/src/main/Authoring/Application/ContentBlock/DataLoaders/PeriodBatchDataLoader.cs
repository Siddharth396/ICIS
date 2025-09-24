namespace Authoring.Application.ContentBlock.DataLoaders
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;

    using Authoring.Application.ContentBlock.DataLoaders.Keys;

    using BusinessLayer.PriceEntry.Services.Calculators.Periods;

    using GreenDonut;

    using Serilog;

    public class PeriodBatchDataLoader : BatchDataLoader<PeriodBatchKey, PeriodCalculatorOutputItem?>
    {
        private readonly ILogger logger;

        private readonly IPeriodCalculator periodCalculator;

        public PeriodBatchDataLoader(
            IPeriodCalculator periodCalculator,
            IBatchScheduler batchScheduler,
            ILogger logger,
            DataLoaderOptions? options = null)
            : base(batchScheduler, options)
        {
            this.periodCalculator = periodCalculator;
            this.logger = logger.ForContext<PeriodBatchDataLoader>();
        }

        protected override async Task<IReadOnlyDictionary<PeriodBatchKey, PeriodCalculatorOutputItem?>> LoadBatchAsync(
            IReadOnlyList<PeriodBatchKey> keys,
            CancellationToken cancellationToken)
        {
            logger.Debug($"Loading batch of periods for {keys.Count} rows", keys.Count);

            var periodsByReferenceDate = keys
               .GroupBy(k => k.ReferenceDate)
               .ToDictionary(g => g.Key, g => g.Select(x => x.PeriodCode).ToList());

            var result = new Dictionary<PeriodBatchKey, PeriodCalculatorOutputItem?>();

            foreach (var (referenceDate, periodCodes) in periodsByReferenceDate)
            {
                var calculatedPeriods = await periodCalculator.CalculatePeriods(
                                            new PeriodCalculatorInput
                                            {
                                                ReferenceDate = referenceDate,
                                                PeriodCodes = periodCodes
                                            });

                if (calculatedPeriods.Count == 0)
                {
                    logger.Debug($"No calculated periods found for reference date {referenceDate} and period codes '{JsonSerializer.Serialize(periodCodes)}'");
                    continue;
                }

                logger.Debug($"Loaded {calculatedPeriods.Count} calculated periods for reference date {referenceDate} and period codes '{JsonSerializer.Serialize(periodCodes)}': '{JsonSerializer.Serialize(calculatedPeriods)}'");

                foreach (var key in keys.Where(k => k.ReferenceDate == referenceDate))
                {
                    var calculatedPeriod = calculatedPeriods.FirstOrDefault(p => p.PeriodCode == key.PeriodCode);

                    if (calculatedPeriod != null)
                    {
                        result[key] = calculatedPeriod;
                    }
                }
            }

            logger.Debug($"Finished loading batch of periods for {keys.Count} rows. Returning {result.Count} results");

            return result;
        }
    }
}
