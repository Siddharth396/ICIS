namespace BusinessLayer.PriceEntry.Services.SeriesItemTypes.SingleValueWithReference
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading.Tasks;

    using BusinessLayer.Helpers;
    using BusinessLayer.PriceEntry.DTOs.Input;
    using BusinessLayer.PriceEntry.Repositories;
    using BusinessLayer.PriceEntry.Repositories.Models;
    using BusinessLayer.PriceEntry.Services.Calculators;
    using BusinessLayer.PriceEntry.Services.Calculators.Periods;
    using BusinessLayer.PriceEntry.Validators;
    using BusinessLayer.PriceEntry.ValueObjects;
    using BusinessLayer.PriceSeriesSelection.Repositories;
    using BusinessLayer.PriceSeriesSelection.Services;

    using Infrastructure.Services.AuditInfoService;
    using Infrastructure.Services.Workflow;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Internal;

    using Serilog;

    using PriceDeltaType = BusinessLayer.PriceEntry.ValueObjects.PriceDeltaType;
    using PriceSeriesOutput = BusinessLayer.PriceEntry.DTOs.Output.PriceSeries;
    using ReferencePriceOutput = BusinessLayer.PriceEntry.DTOs.Output.ReferencePrice;

    public class SingleValueWithReferencePriceService :
            BasePriceItemService<PriceItemSingleValueWithReference>
    {
        private readonly IDeltaCalculator deltaCalculator;

        private readonly IOutrightPriceCalculator outrightPriceCalculator;

        private readonly IReferencePriceService referencePriceService;

        private readonly ReferenceMarketRepository referenceMarketRepository;

        private readonly ILogger logger;

        private readonly IPriceSeriesService priceSeriesService;

        public SingleValueWithReferencePriceService(
            PriceSeriesRepository priceSeriesRepository,
            PriceDeltaTypeRepository priceDeltaTypeRepository,
            IDeltaCalculator deltaCalculator,
            IOutrightPriceCalculator outrightPriceCalculator,
            ISystemClock clock,
            IPeriodCalculator periodCalculator,
            IAuditInfoService auditInfoService,
            IReferencePriceService referencePriceService,
            IAbsolutePeriodDomainService absolutePeriodDomainService,
            ReferenceMarketRepository referenceMarketRepository,
            ILogger logger,
            IPriceSeriesService priceSeriesService,
            IPriceSeriesItemsDomainService priceSeriesItemsDomainService,
            IConfiguration configuration)
            : base(
                  priceSeriesRepository,
                  priceDeltaTypeRepository,
                  clock,
                  auditInfoService,
                  absolutePeriodDomainService,
                  priceSeriesItemsDomainService,
                  logger,
                  configuration)
        {
            this.priceSeriesService = priceSeriesService;
            this.deltaCalculator = deltaCalculator;
            this.outrightPriceCalculator = outrightPriceCalculator;
            this.referencePriceService = referencePriceService;
            this.logger = logger.ForContext<SingleValueWithReferencePriceService>();
            this.referenceMarketRepository = referenceMarketRepository;
        }

        // Public for now to be able to test it in the unit tests
        public static bool HasAssessmentMethodChanged(
            PriceItemSingleValueWithReference newPriceItem,
            PriceItemSingleValueWithReference oldPriceItem)
        {
            return !string.IsNullOrWhiteSpace(oldPriceItem.AssessmentMethod)
                   && string.Compare(
                       oldPriceItem.AssessmentMethod,
                       newPriceItem.AssessmentMethod,
                       StringComparison.Ordinal)
                   != 0;
        }

        // Public for now to be able to test it in the unit tests
        public static void ResetFieldsOnAssessmentMethodChanged(PriceItemSingleValueWithReference priceItem)
        {
            priceItem.Price = null;
            priceItem.ReferencePrice = null;
            priceItem.PremiumDiscount = null;
        }

        public override async Task<List<PriceSeriesOutput>> ExtendPriceSeries(
            List<PriceSeriesOutput> priceSeries,
            DateTime assessedDateTime,
            List<PeriodCalculatorOutputItem> absolutePeriods)
        {
            var priceSeriesWithReferencePriceNotSet = priceSeries.Where(x => AssessmentMethod.PremiumDiscount.Matches(x.AssessmentMethod)
                                                                    && x.ReferencePrice?.Market is null).ToList();

            if (priceSeriesWithReferencePriceNotSet.Any())
            {
                logger.Debug($"The price series without reference price has been found, and the reference price needs to be fetched.");

                foreach (var series in priceSeriesWithReferencePriceNotSet)
                {
                    var absolutePeriod = absolutePeriods.SingleOrDefault(x => x.PeriodCode == series.RelativeFulfilmentPeriod?.Code);

                    if (series.LastAssessmentReferenceMarket is not null)
                    {
                        logger.Debug($"Fetching the reference price for series id {series.Id}");

                        var referencePrice = await referencePriceService.GetReferencePrice(
                                                 series.LastAssessmentReferenceMarket,
                                                 assessedDateTime,
                                                 UtcDateTime.GetUtcDateTime(absolutePeriod?.FromDate),
                                                 UtcDateTime.GetUtcDateTime(absolutePeriod?.UntilDate),
                                                 series.Id);

                        series.ReferencePrice = new ReferencePriceOutput
                        {
                            Market = referencePrice.Market,
                            Datetime = referencePrice.Datetime,
                            PeriodLabel = referencePrice.PeriodLabel,
                            SeriesName = referencePrice.SeriesName,
                            Price = referencePrice.Price
                        };
                    }
                }
            }

            return priceSeries;
        }

        public override async Task<bool> AreAllReferencePriceSeriesPublishedOrInSameContentBlock(List<string> priceSeriesIds, DateTime assessedDateTime, List<string> otherGridsPriceSeriesIds)
        {
            var priceSeriesItems = await PriceSeriesItemsDomainService.GetPriceSeriesItems(priceSeriesIds, assessedDateTime);

            var referenceMarkets = await referenceMarketRepository.GetReferenceMarketsByType(ReferenceMarketType.LNG);

            foreach (var priceSeriesItem in priceSeriesItems)
            {
                var priceItemSingleValueWithReference = (PriceItemSingleValueWithReference)priceSeriesItem;

                if (AssessmentMethod.PremiumDiscount.Matches(priceItemSingleValueWithReference.AssessmentMethod))
                {
                    var referenceMarket = referenceMarkets.SingleOrDefault(x => x.Name == priceItemSingleValueWithReference.ReferencePrice!.Market);

                    if (referenceMarket != null)
                    {
                        var (fulfilmentFrom, fulfilmentUntil) = (priceItemSingleValueWithReference.FulfilmentFromDate, priceItemSingleValueWithReference.FulfilmentUntilDate);

                        if (fulfilmentFrom is null && fulfilmentUntil is null)
                        {
                            var priceSeries = await priceSeriesService.GetPriceSeriesById(priceItemSingleValueWithReference.SeriesId);

                            if (referencePriceService.UseMonthPlusOnePeriod(fulfilmentFrom, fulfilmentUntil, priceSeries.RelativeFulfilmentPeriod?.Code))
                            {
                                (fulfilmentFrom, fulfilmentUntil) = await referencePriceService.GetFulfilmentDatesForM1Period(assessedDateTime);
                            }
                        }

                        var referencePriceSeriesItem = await PriceSeriesItemsDomainService.GetPriceSeriesItem(
                                                           referenceMarket.PriceSeriesIds!,
                                                           assessedDateTime,
                                                           fulfilmentFrom,
                                                           fulfilmentUntil);

                        var isReferencePriceInSameContentBlock = otherGridsPriceSeriesIds.Contains(referencePriceSeriesItem?.SeriesId ?? string.Empty);

                        if (isReferencePriceInSameContentBlock)
                        {
                            continue;
                        }

                        if (!WorkflowStatus.IsPublishedOrCorrectionPublishedStatusMatch(referencePriceSeriesItem?.Status))
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        protected override async Task<PriceItemSingleValueWithReference> BuildPriceItemForSaving(
            PriceItemInput priceItemInput,
            PriceItemSingleValueWithReference? current)
        {
            var localLogger = logger.ForContext("SeriesId", priceItemInput.SeriesId)
                   .ForContext("AssessedDateTime", priceItemInput.AssessedDateTime);

            localLogger.Information($"START : Building price item with priceItemInput {priceItemInput}");

            var newPriceItem = new PriceItemSingleValueWithReference
            {
                Price = priceItemInput.SeriesItem.Price,
                DataUsed = priceItemInput.SeriesItem.DataUsed,
                AssessmentMethod = priceItemInput.SeriesItem.AssessmentMethod,
                PremiumDiscount = priceItemInput.SeriesItem.PremiumDiscount,
                AdjustedPriceDelta = priceItemInput.SeriesItem.AdjustedPriceDelta
            };

            if (AssessmentMethod.PremiumDiscount.Matches(newPriceItem.AssessmentMethod) && !string.IsNullOrWhiteSpace(priceItemInput.SeriesItem.ReferenceMarketName))
            {
                newPriceItem.ReferencePrice = await GetReferencePrice(priceItemInput, current);
                newPriceItem.Price = outrightPriceCalculator.GetOutrightPrice(
                    newPriceItem.ReferencePrice?.Price,
                    newPriceItem.PremiumDiscount);
            }

            if (current != null)
            {
                var assessmentMethodChanged = HasAssessmentMethodChanged(newPriceItem, current);

                if (assessmentMethodChanged)
                {
                    localLogger.Debug($"Assessment method changed.Current assessment method is {current.AssessmentMethod}.New assessment method is {newPriceItem.AssessmentMethod}");

                    ResetFieldsOnAssessmentMethodChanged(newPriceItem);
                }
            }

            var priceDeltaResult = await CalculatePriceDeltaBasedOnPreviousPrice(
                                       priceItemInput,
                                       newPriceItem);

            newPriceItem.PriceDelta = priceDeltaResult?.PriceDelta;

            localLogger.Information($"END : New price item {newPriceItem}");

            return newPriceItem;
        }

        [ExcludeFromCodeCoverage(Justification = "Price item single value with reference is only for LNG and not covered under advance correction workflow")]
        protected override PriceItemSingleValueWithReference DeltaCalculationForNextDate(
         PriceItemSingleValueWithReference priceSeriesItem, PriceItemSingleValueWithReference nextPriceSeriesItem)
        {
            nextPriceSeriesItem.PriceDelta = deltaCalculator.GetPriceDelta(nextPriceSeriesItem.Price, priceSeriesItem.Price).PriceDelta;
            return nextPriceSeriesItem;
        }

        protected override PriceDeltaType GetPriceDeltaType(PriceItemSingleValueWithReference item)
        {
            return item.AdjustedPriceDelta.HasValue ? PriceDeltaType.NonMarketAdjustment : PriceDeltaType.Regular;
        }

        protected override bool IsPriceSeriesItemValid(PriceItemSingleValueWithReference item)
        {
            var result = PriceSeriesValidator.ValidateSingleValueWithReferenceSeries(item, true);
            return result.IsValid;
        }

        private async Task<PriceDeltaResult> CalculatePriceDeltaBasedOnPreviousPrice(
            PriceItemInput priceItemInput,
            PriceItemSingleValueWithReference newPriceItem)
        {
            var previousPriceSeriesItem = await PriceSeriesItemsDomainService.GetPreviousPriceSeriesItem(priceItemInput.SeriesId, priceItemInput.AssessedDateTime) as PriceItemSingleValueWithReference;

            var priceDeltaResult = deltaCalculator.GetPriceDelta(newPriceItem?.Price, previousPriceSeriesItem?.Price);
            return priceDeltaResult;
        }

        private async Task<ReferencePrice> GetReferencePrice(PriceItemInput priceItemInput, PriceItemSingleValueWithReference? current)
        {
            var (fulfilmentFrom, fulfilmentUntil) = await PriceSeriesItemsDomainService.GetFulfilmentDates(priceItemInput.SeriesId, priceItemInput.AssessedDateTime, current);

            return await referencePriceService.GetReferencePrice(
                priceItemInput.SeriesItem.ReferenceMarketName!,
                priceItemInput.AssessedDateTime,
                fulfilmentFrom,
                fulfilmentUntil,
                priceItemInput.SeriesId);
        }
    }
}
