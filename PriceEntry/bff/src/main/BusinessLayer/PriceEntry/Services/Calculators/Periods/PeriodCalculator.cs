namespace BusinessLayer.PriceEntry.Services.Calculators.Periods
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using BusinessLayer.PriceEntry.ValueObjects;

    using Infrastructure.Services.PeriodGenerator;
    using Microsoft.Extensions.Caching.Memory;

    using Serilog;

    public class PeriodCalculator : IPeriodCalculator
    {
        private readonly IPeriodGeneratorService periodGeneratorService;

        private readonly PeriodsCache periodsCache;

        private readonly ILogger logger = Log.ForContext<PeriodCalculator>();

        public PeriodCalculator(IMemoryCache memoryCache, IPeriodGeneratorService periodGeneratorService)
        {
            periodsCache = new PeriodsCache(memoryCache);
            this.periodGeneratorService = periodGeneratorService;
        }

        public async Task<List<PeriodCalculatorOutputItem>> CalculatePeriods(PeriodCalculatorInput input)
        {
            logger.Debug("Calculating periods for {@input}", input);

            var result = new List<PeriodCalculatorOutputItem>();

            var toBeRequestedCodes = new List<string>();

            foreach (var periodCodes in input.PeriodCodes.Where(periodCode => RelativeFulfilmentPeriodCode.IsAllowed(periodCode) ||
                                                                              ReferencePeriodCode.IsAllowed(periodCode)))
            {
                var cachedItem = periodsCache.Get(input.ReferenceDate, periodCodes);

                if (cachedItem != null)
                {
                    result.Add(cachedItem);
                }
                else
                {
                    toBeRequestedCodes.Add(periodCodes);
                }
            }

            if (toBeRequestedCodes.Count == 0)
            {
                logger.Debug("All periods were found in cache.");
                return result;
            }

            var items = await MakeRequestToPeriodGeneratorService(input.ReferenceDate, toBeRequestedCodes);

            foreach (var item in items)
            {
                result.Add(item);
                periodsCache.Set(
                    item.ReferenceDate,
                    item.PeriodCode,
                    item);
            }

            logger.Debug("Periods calculated for {@input}.", input);
            return result;
        }

        private async Task<List<PeriodCalculatorOutputItem>> MakeRequestToPeriodGeneratorService(
            DateOnly referenceDate,
            List<string> periodCodes)
        {
            var result = await periodGeneratorService.GeneratePeriods(referenceDate, periodCodes);

            return result.AbsolutePeriods.Select(
                    x => new PeriodCalculatorOutputItem
                    {
                        ReferenceDate = referenceDate,
                        Code = x.Code,
                        PeriodCode = x.PeriodCode,
                        Label = x.Label,
                        FromDate = x.FromDate,
                        UntilDate = x.UntilDate
                    })
               .ToList();
        }

        private class PeriodsCache
        {
            private readonly IMemoryCache cache;

            public PeriodsCache(IMemoryCache cache)
            {
                this.cache = cache;
            }

            public PeriodCalculatorOutputItem? Get(DateOnly referenceDate, string periodCode)
            {
                return cache.Get<PeriodCalculatorOutputItem>(CacheKey.From(referenceDate, periodCode));
            }

            public void Set(DateOnly referenceDate, string periodCode, PeriodCalculatorOutputItem item)
            {
                cache.Set(
                    CacheKey.From(referenceDate, periodCode),
                    item,
                    new MemoryCacheEntryOptions
                    {
                        SlidingExpiration = TimeSpan.FromMinutes(10),
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
                    });
            }

            private record CacheKey
            {
                public required DateOnly ReferenceDate { get; init; }

                public required string PeriodCode { get; init; }

                public static CacheKey From(DateOnly referenceDate, string periodCode)
                {
                    return new CacheKey { ReferenceDate = referenceDate, PeriodCode = periodCode };
                }
            }
        }
    }
}
