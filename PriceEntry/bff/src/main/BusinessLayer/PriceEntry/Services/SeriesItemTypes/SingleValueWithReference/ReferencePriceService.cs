namespace BusinessLayer.PriceEntry.Services.SeriesItemTypes.SingleValueWithReference
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using BusinessLayer.Helpers;
    using BusinessLayer.PriceEntry.DTOs.Input;
    using BusinessLayer.PriceEntry.Handler;
    using BusinessLayer.PriceEntry.Repositories;
    using BusinessLayer.PriceEntry.Repositories.Models;
    using BusinessLayer.PriceEntry.Services.Calculators.Periods;
    using BusinessLayer.PriceEntry.ValueObjects;
    using BusinessLayer.PriceSeriesSelection.Services;

    using Serilog;
    using Serilog.Context;

    using PriceSeries = BusinessLayer.PriceSeriesSelection.Repositories.Models.PriceSeries;

    public class ReferencePriceService : IReferencePriceService
    {
        private readonly IPriceSeriesService priceSeriesService;

        private readonly ReferenceMarketRepository referenceMarketRepository;

        private readonly GasPriceSeriesItemRepository gasPriceSeriesItemRepository;

        private readonly IAuthoringService priceEntryService;

        private readonly IPeriodCalculator periodCalculator;

        private readonly IPriceSeriesItemsDomainService priceSeriesItemsDomainService;

        private readonly ILogger logger;

        public ReferencePriceService(
            IPriceSeriesService priceSeriesService,
            ReferenceMarketRepository referenceMarketRepository,
            GasPriceSeriesItemRepository gasPriceSeriesItemRepository,
            IAuthoringService priceEntryService,
            ILogger logger,
            IPeriodCalculator periodCalculator,
            IPriceSeriesItemsDomainService priceSeriesItemsDomainService)
        {
            this.priceSeriesService = priceSeriesService;
            this.referenceMarketRepository = referenceMarketRepository;
            this.gasPriceSeriesItemRepository = gasPriceSeriesItemRepository;
            this.priceEntryService = priceEntryService;
            this.periodCalculator = periodCalculator;
            this.priceSeriesItemsDomainService = priceSeriesItemsDomainService;
            this.logger = logger.ForContext<ReferencePriceService>();
        }

        public bool UseMonthPlusOnePeriod(
           DateTime? fulfilmentFromDateTime,
           DateTime? fulfilmentUntilDateTime,
           string? relativeFulfilmentPeriod)
        {
            if (fulfilmentFromDateTime is null &&
                fulfilmentUntilDateTime is null &&
                (relativeFulfilmentPeriod == RelativeFulfilmentPeriodCode.FifteenFortyFiveDays ||
                    relativeFulfilmentPeriod == RelativeFulfilmentPeriodCode.TwentyFortyDays))
            {
                return true;
            }

            return false;
        }

        public async Task<ReferencePrice> GetReferencePrice(
            string referenceMarketName,
            DateTime assessedDateTime,
            DateTime? fulfilmentFromDateTime,
            DateTime? fulfilmentUntilDateTime,
            string seriesId)
        {
            var referenceMarket = await GetReferenceMarket(referenceMarketName);

            var referencePrice = new ReferencePrice
            {
                Market = referenceMarketName
            };

            var (fulfilmentFrom, fulfilmentUntil) = (fulfilmentFromDateTime, fulfilmentUntilDateTime);

            var priceSeries = await priceSeriesService.GetPriceSeriesById(seriesId);

            if (UseMonthPlusOnePeriod(fulfilmentFromDateTime, fulfilmentUntilDateTime, priceSeries.RelativeFulfilmentPeriod?.Code))
            {
                (fulfilmentFrom, fulfilmentUntil) = await GetFulfilmentDatesForM1Period(assessedDateTime);
            }

            if (ReferenceMarketType.LNG.Matches(referenceMarket.Type))
            {
                var referencePriceSeries = await GetReferencePriceSeries(referenceMarket.PriceSeriesIds!, fulfilmentFrom, fulfilmentUntil, assessedDateTime);

                if (referencePriceSeries != null)
                {
                    referencePrice.SeriesId = referencePriceSeries.Id;
                    referencePrice.SeriesName = GetPriceSeriesName(referencePriceSeries);

                    var lngPrice = await priceSeriesItemsDomainService.GetPriceSeriesItem(
                                       referencePriceSeries.Id,
                                       assessedDateTime,
                                       fulfilmentFrom,
                                       fulfilmentUntil);

                    if (lngPrice != null)
                    {
                        referencePrice.Price = lngPrice.GetPriceValue();
                        referencePrice.Datetime = lngPrice.AssessedDateTime;
                        referencePrice.PeriodLabel = lngPrice.PeriodLabel;
                    }
                }
            }
            else
            {
                var gasPrice = await gasPriceSeriesItemRepository.GetGasPriceSeriesItem(
                                referenceMarketName,
                                assessedDateTime,
                                fulfilmentFrom,
                                fulfilmentUntil);

                if (gasPrice != null)
                {
                    referencePrice.Datetime = gasPrice.AssessedDateTime;
                    referencePrice.PeriodLabel = gasPrice.PeriodLabel;
                    referencePrice.Price = gasPrice.Mid;
                    referencePrice.SeriesName = gasPrice.SpecificationName;
                }
            }

            return referencePrice;
        }

        public async Task<List<string>> UpdateGasReferencePrice(GasPricePayload gasPricePayload)
        {
            using (LogContext.PushProperty("Flow", "UpdateGasReferencePrice"))
            {
                var priceSeriesItemsIdsWithCalculatedPrice = new List<string>();

                var priceSeriesItems = await priceSeriesItemsDomainService.GetPriceSeriesItems(
                                           gasPricePayload.GetAssessedDateTime(),
                                           gasPricePayload.GetFulfilmentFrom(),
                                           gasPricePayload.GetFulfilmentUntil());

                foreach (var priceSeriesItem in priceSeriesItems)
                {
                    if (priceSeriesItem is PriceItemSingleValueWithReference priceItemSingleValueWithRef
                        && AssessmentMethod.PremiumDiscount.Matches(priceItemSingleValueWithRef.AssessmentMethod)
                        && priceItemSingleValueWithRef.ReferencePrice?.Market == gasPricePayload.MarketCode)
                    {
                        logger.Information($"Updating reference price for seriesId {priceItemSingleValueWithRef.SeriesId} and assessed date {priceItemSingleValueWithRef.AssessedDateTime}.");

                        var priceItemInput = GetPriceItemInput(priceItemSingleValueWithRef);

                        var result = await priceEntryService.SavePriceEntryData(priceItemInput);

                        logger.Information($"Updated reference price for seriesItemId {result.Id}.");

                        priceSeriesItemsIdsWithCalculatedPrice.Add(result.Id);
                    }
                }

                return priceSeriesItemsIdsWithCalculatedPrice;
            }
        }

        public async Task UpdateLngReferencePrice(PriceSeriesItemSavedEvent priceSeriesItemSavedEvent)
        {
            using (LogContext.PushProperty("Flow", "UpdateLngReferencePrice"))
            {
                var referenceMarkets = await referenceMarketRepository.GetReferenceMarketsByType(ReferenceMarketType.LNG);

                var referenceSeriesIds = referenceMarkets.SelectMany(x => x.PriceSeriesIds!).ToList();

                if (referenceSeriesIds.Any(x => x == priceSeriesItemSavedEvent.SeriesId))
                {
                    var priceSeriesItems = await priceSeriesItemsDomainService.GetPriceSeriesItems(priceSeriesItemSavedEvent.AssessedDateTime);

                    foreach (var priceSeriesItem in priceSeriesItems)
                    {
                        if (priceSeriesItem is PriceItemSingleValueWithReference priceItemSingleValueWithRef)
                        {
                            if (AssessmentMethod.PremiumDiscount.Matches(priceItemSingleValueWithRef.AssessmentMethod) &&
                                priceItemSingleValueWithRef.ReferencePrice?.SeriesId == priceSeriesItemSavedEvent.SeriesId)
                            {
                                logger.Information($"Updating reference price for seriesId {priceItemSingleValueWithRef.SeriesId} and assessed date {priceItemSingleValueWithRef.AssessedDateTime}.");

                                var priceItemInput = GetPriceItemInput(priceItemSingleValueWithRef);

                                var result = await priceEntryService.SavePriceEntryData(priceItemInput);

                                logger.Information($"Updated reference price for seriesItemId {result.Id}");
                            }
                        }
                    }
                }
            }
        }

        public async Task<(DateTime? FulfilmentFrom, DateTime? FulfilmentUntil)> GetFulfilmentDatesForM1Period(DateTime assessedDateTime)
        {
            var absolutePeriods = await periodCalculator.CalculatePeriods(
                           new PeriodCalculatorInput
                           {
                               ReferenceDate = DateOnly.FromDateTime(assessedDateTime),
                               PeriodCodes = new List<string> { RelativeFulfilmentPeriodCode.MonthPlusOne }
                           });

            var absolutePeriod = absolutePeriods.Single();

            var fulfilmentFrom = UtcDateTime.GetUtcDateTime(absolutePeriod.FromDate);

            var fulfilmentUntil = UtcDateTime.GetUtcDateTime(absolutePeriod.UntilDate);

            return (fulfilmentFrom, fulfilmentUntil);
        }

        public async Task<List<string>> GetPriceSeriesIdsForReferencePriceSeriesItems(List<string> seriesIds, DateTime assessedDateTime)
        {
            return await priceSeriesItemsDomainService.GetPriceSeriesIdsForReferencePriceSeriesItems(seriesIds, assessedDateTime);
        }

        public async Task<IDictionary<string, bool>> HasReferencePriceSeriesItems(List<string> seriesIds, DateTime assessedDateTime)
        {
            var priceSeriesIdsForReferencePriceSeriesItems =
                await priceSeriesItemsDomainService.GetPriceSeriesIdsForReferencePriceSeriesItems(seriesIds, assessedDateTime);

            return seriesIds.ToDictionary(seriesId => seriesId, seriesId => priceSeriesIdsForReferencePriceSeriesItems.Contains(seriesId));
        }

        private static PriceItemInput GetPriceItemInput(PriceItemSingleValueWithReference priceItemSingleValueWithReference)
        {
            return new PriceItemInput
            {
                AssessedDateTime = priceItemSingleValueWithReference.AssessedDateTime,
                SeriesId = priceItemSingleValueWithReference.SeriesId,
                SeriesItemTypeCode = SeriesItemTypeCode.SingleValueWithReference,
                SeriesItem = new SeriesItem
                {
                    AssessmentMethod = priceItemSingleValueWithReference.AssessmentMethod,
                    DataUsed = priceItemSingleValueWithReference.DataUsed,
                    ReferenceMarketName = priceItemSingleValueWithReference.ReferencePrice!.Market,
                    Price = priceItemSingleValueWithReference.Price,
                    PremiumDiscount = priceItemSingleValueWithReference.PremiumDiscount
                }
            };
        }

        private async Task<ReferenceMarket> GetReferenceMarket(string referenceMarketName)
        {
            return await referenceMarketRepository.GetReferenceMarket(referenceMarketName);
        }

        private string? GetPriceSeriesName(PriceSeries priceSeries)
        {
            return priceSeries.SeriesShortName?.English ?? priceSeries.SeriesName.English;
        }

        private async Task<PriceSeries?> GetReferencePriceSeries(
            List<string> referenceSeriesIds,
            DateTime? fulfilmentFrom,
            DateTime? fulfilmentUntil,
            DateTime assessedDateTime)
        {
            var activeReferencePriceSeries = await priceSeriesService.GetActivePriceSeriesByIds(referenceSeriesIds, assessedDateTime);

            var fulfilmentPeriodCodes = activeReferencePriceSeries.Select(x => x.RelativeFulfilmentPeriod?.Code).Distinct().ToList();

            if (fulfilmentPeriodCodes.All(code => code == RelativeFulfilmentPeriodCode.TwentyFortyDays))
            {
                return activeReferencePriceSeries.First();
            }

            var absolutePeriods = await periodCalculator.CalculatePeriods(
                               new PeriodCalculatorInput
                               {
                                   ReferenceDate = DateOnly.FromDateTime(assessedDateTime),
                                   PeriodCodes = fulfilmentPeriodCodes!
                               });

            var fulfilmentFromDate = fulfilmentFrom.HasValue ? DateOnly.FromDateTime(fulfilmentFrom.Value) : default;

            var fulfilmentUntilDate = fulfilmentUntil.HasValue ? DateOnly.FromDateTime(fulfilmentUntil.Value) : default;

            var absolutePeriod = absolutePeriods.SingleOrDefault(x => x.FromDate == fulfilmentFromDate && x.UntilDate == fulfilmentUntilDate);

            var referencePriceSeries = activeReferencePriceSeries.SingleOrDefault(x => x.RelativeFulfilmentPeriod?.Code == absolutePeriod?.PeriodCode);

            return referencePriceSeries;
        }
    }
}
