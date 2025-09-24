namespace BusinessLayer.PriceEntry.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json;
    using System.Threading.Tasks;

    using BusinessLayer.PriceEntry.Repositories;
    using BusinessLayer.PriceEntry.Repositories.Models;
    using BusinessLayer.PriceEntry.Services.Calculators.Periods;
    using BusinessLayer.PriceEntry.ValueObjects;
    using BusinessLayer.PriceSeries.Services;
    using BusinessLayer.PriceSeriesSelection.Repositories.Models;

    using Infrastructure.Attributes.BusinessAnnotations;

    using Serilog;
    using Serilog.Context;

    using static BusinessLayer.Helpers.PeriodLabelTypeHelper;

    [DomainService]
    public class AbsolutePeriodDomainService : IAbsolutePeriodDomainService
    {
        private readonly IPriceSeriesItemRepository priceSeriesItemRepository;

        private readonly IPriceSeriesDomainService priceSeriesDomainService;

        private readonly ILogger logger;

        private readonly IPeriodCalculator periodCalculator;

        public AbsolutePeriodDomainService(
            IPriceSeriesItemRepository priceSeriesItemRepository,
            IPriceSeriesDomainService priceSeriesDomainService,
            ILogger logger,
            IPeriodCalculator periodCalculator)
        {
            this.priceSeriesDomainService = priceSeriesDomainService;
            this.logger = logger.ForContext<AbsolutePeriodDomainService>();
            this.periodCalculator = periodCalculator;
            this.priceSeriesItemRepository = priceSeriesItemRepository;
        }

        public async Task<PeriodCalculatorOutputItem?> GetAbsolutePeriod(PriceSeries priceSeries, DateTime assessedDateTime)
        {
            var seriesId = priceSeries.Id;

            using (LogContext.PushProperty("SeriesId", seriesId))
            using (LogContext.PushProperty("AssessedDateTime", assessedDateTime))
            {
                var periodLabelType = priceSeries.PeriodLabelTypeCode;

                var lastPublishedAppliesFromDateTime = PeriodLabelTypeCode.ReferenceTime.Matches(periodLabelType) ? await GetLastPublishedAppliesFromDateTime(seriesId, assessedDateTime) :
                                                                                                                    null;

                var absolutePeriodCalculationInput = GetAbsolutePeriodCalculationInput(
                                                            logger,
                                                            priceSeries,
                                                            assessedDateTime,
                                                            lastPublishedAppliesFromDateTime);

                if (absolutePeriodCalculationInput is null)
                {
                    return null;
                }

                var (referenceDate, periodCode) = absolutePeriodCalculationInput.Value;

                return await GetAbsolutePeriod(referenceDate, periodCode);
            }
        }

        public async Task<List<PeriodCalculatorOutputItem>> GetAbsolutePeriods(List<PriceSeriesAggregation> priceSeriesAggregations, DateTime assessedDateTime)
        {
            using (LogContext.PushProperty("SeriesId", priceSeriesAggregations.Select(x => x.Id)))
            using (LogContext.PushProperty("AssessedDateTime", assessedDateTime))
            {
                Dictionary<DateOnly, List<string>> absolutePeriodInputs = [];

                foreach (var priceSeriesAggregation in priceSeriesAggregations)
                {
                    var absolutePeriodCalculationInput = GetAbsolutePeriodCalculationInput(
                                                           logger,
                                                           priceSeriesAggregation,
                                                           assessedDateTime);

                    if (absolutePeriodCalculationInput is not null)
                    {
                        var (referenceDate, periodCode) = absolutePeriodCalculationInput.Value;

                        if (!absolutePeriodInputs.TryGetValue(referenceDate, out var periodCodes))
                        {
                            periodCodes = [];
                            absolutePeriodInputs[referenceDate] = periodCodes;
                        }

                        periodCodes.Add(periodCode);
                    }
                }

                var absolutePeriodTasks = absolutePeriodInputs.Select(absolutePeriodInput => GetAbsolutePeriods(absolutePeriodInput.Key, absolutePeriodInput.Value));

                var absolutePeriods = await Task.WhenAll(absolutePeriodTasks);
                return absolutePeriods.SelectMany(x => x).ToList();
            }
        }

        public PeriodCalculatorOutputItem? FilterAbsolutePeriod(
            List<PeriodCalculatorOutputItem> absolutePeriods,
            PriceSeriesAggregation priceSeriesAggregation,
            DateTime assessedDateTime)
        {
            var absolutePeriodCalculationInput = GetAbsolutePeriodCalculationInput(
                                                    logger,
                                                    priceSeriesAggregation,
                                                    assessedDateTime);

            return absolutePeriods.FirstOrDefault(x => x.ReferenceDate == absolutePeriodCalculationInput?.ReferenceDate &&
                                                       x.PeriodCode == absolutePeriodCalculationInput?.PeriodCode);
        }

        public async Task<PeriodCalculatorOutputItem?> GetAbsolutePeriod(string seriesId, DateTime assessedDateTime)
        {
            var priceSeries = await priceSeriesDomainService.GetPriceSeriesById(seriesId);

            return await GetAbsolutePeriod(priceSeries, assessedDateTime);
        }

        private async Task<List<PeriodCalculatorOutputItem>> GetAbsolutePeriods(DateOnly referenceDate, List<string> periodCodes)
        {
            logger.Debug("Fetching absolute period data for period code {PeriodCodes}", periodCodes);

            var absolutePeriods = await periodCalculator.CalculatePeriods(
                    new PeriodCalculatorInput
                    {
                        ReferenceDate = referenceDate,
                        PeriodCodes = periodCodes
                    });

            logger.Debug("Loaded {Count} calculated periods for reference date {ReferenceDate} and period codes '{PeriodCodes}': '{AbsolutePeriods}'", absolutePeriods.Count, referenceDate, periodCodes, JsonSerializer.Serialize(absolutePeriods));

            return absolutePeriods.Where(
                                    p => periodCodes.Contains(p.PeriodCode) && p.ReferenceDate == referenceDate)
                                 .ToList();
        }

        private async Task<PeriodCalculatorOutputItem?> GetAbsolutePeriod(DateOnly referenceDate, string periodCode)
        {
            logger.Debug("Fetching absolute period data for period code {PeriodCode}", periodCode);

            var absolutePeriods = await periodCalculator.CalculatePeriods(
                    new PeriodCalculatorInput
                    {
                        ReferenceDate = referenceDate,
                        PeriodCodes = [periodCode]
                    });

            var absolutePeriod = absolutePeriods.FirstOrDefault(
                                    p => p.PeriodCode == periodCode && p.ReferenceDate == referenceDate);

            if (absolutePeriod == null)
            {
                logger.Debug("Unable to retrieve absolute period for period code: {PeriodCode}", periodCode);
                return null;
            }

            logger.Debug("Received absolute period: {Label}, {FromDate}, {UntilDate} for period code {PeriodCode}", absolutePeriod.Label, absolutePeriod.FromDate, absolutePeriod.UntilDate, absolutePeriod.PeriodCode);

            return absolutePeriod;
        }

        private async Task<DateTime?> GetLastPublishedAppliesFromDateTime(string seriesId, DateTime assessedDatetime)
        {
            var previousPriceSeriesItem = await priceSeriesItemRepository.GetPreviousPriceSeriesItem(
                                            [seriesId],
                                            assessedDatetime);

            return previousPriceSeriesItem?.AppliesFromDateTime;
        }
    }
}
