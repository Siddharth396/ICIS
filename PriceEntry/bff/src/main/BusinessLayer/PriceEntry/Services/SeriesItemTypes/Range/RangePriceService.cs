namespace BusinessLayer.PriceEntry.Services.SeriesItemTypes.Range
{
    using System.Threading.Tasks;

    using BusinessLayer.PriceEntry.DTOs.Input;
    using BusinessLayer.PriceEntry.Repositories;
    using BusinessLayer.PriceEntry.Repositories.Models;
    using BusinessLayer.PriceEntry.Services.Calculators;
    using BusinessLayer.PriceEntry.Validators;
    using BusinessLayer.PriceSeriesSelection.Repositories;

    using Infrastructure.Services.AuditInfoService;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Internal;

    using Serilog;

    using PriceDeltaType = BusinessLayer.PriceEntry.ValueObjects.PriceDeltaType;

    public class RangePriceService : BasePriceItemService<PriceItemRange>
    {
        private readonly IMidPriceCalculator midPriceCalculator;

        private readonly IDeltaCalculator deltaCalculator;

        private readonly ILogger logger;

        public RangePriceService(
            IMidPriceCalculator midPriceCalculator,
            IDeltaCalculator deltaCalculator,
            PriceSeriesRepository priceSeriesRepository,
            PriceDeltaTypeRepository priceDeltaTypeRepository,
            ISystemClock clock,
            IAuditInfoService auditInfoService,
            IAbsolutePeriodDomainService absolutePeriodDomainService,
            IPriceSeriesItemsDomainService priceSeriesItemsDomainService,
            ILogger logger,
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
            this.midPriceCalculator = midPriceCalculator;
            this.deltaCalculator = deltaCalculator;
            this.logger = logger.ForContext<RangePriceService>();
        }

        protected override async Task<PriceItemRange> BuildPriceItemForSaving(
            PriceItemInput priceItemInput,
            PriceItemRange? current)
        {
            var localLogger = logger.ForContext("SeriesId", priceItemInput.SeriesId)
                   .ForContext("AssessedDateTime", priceItemInput.AssessedDateTime);

            localLogger.Information($"START : Building price item with priceItemInput {priceItemInput}");

            var newPriceItem = new PriceItemRange
            {
                PriceLow = priceItemInput.SeriesItem.PriceLow,
                PriceMid = midPriceCalculator.CalculateMidPrice(
                    priceItemInput.SeriesItem.PriceLow,
                    priceItemInput.SeriesItem.PriceHigh),
                PriceHigh = priceItemInput.SeriesItem.PriceHigh,
                AdjustedPriceLowDelta = priceItemInput.SeriesItem.AdjustedPriceLowDelta,
                AdjustedPriceHighDelta = priceItemInput.SeriesItem.AdjustedPriceHighDelta,
                AdjustedPriceMidDelta = midPriceCalculator.CalculateMidPrice(
                    priceItemInput.SeriesItem.AdjustedPriceLowDelta,
                    priceItemInput.SeriesItem.AdjustedPriceHighDelta),
            };

            var (priceLowDeltaResult, priceMidDeltaResult, priceHighDeltaResult) = await CalculatePriceDeltaBasedOnPreviousPrice(priceItemInput, newPriceItem);

            newPriceItem.PriceLowDelta = priceLowDeltaResult?.PriceDelta;
            newPriceItem.PriceMidDelta = priceMidDeltaResult?.PriceDelta;
            newPriceItem.PriceHighDelta = priceHighDeltaResult?.PriceDelta;

            localLogger.Information($"END : New price item {newPriceItem}");

            return newPriceItem;
        }

        protected override PriceItemRange DeltaCalculationForNextDate(PriceItemRange priceSeriesItem, PriceItemRange nextPriceSeriesItem)
        {
            nextPriceSeriesItem.PriceLowDelta = deltaCalculator.GetPriceDelta(nextPriceSeriesItem.PriceLow, priceSeriesItem.PriceLow).PriceDelta;
            nextPriceSeriesItem.PriceMidDelta = deltaCalculator.GetPriceDelta(nextPriceSeriesItem.PriceMid, priceSeriesItem.PriceMid).PriceDelta;
            nextPriceSeriesItem.PriceHighDelta = deltaCalculator.GetPriceDelta(nextPriceSeriesItem.PriceHigh, priceSeriesItem.PriceHigh).PriceDelta;

            return nextPriceSeriesItem;
        }

        protected override PriceDeltaType GetPriceDeltaType(PriceItemRange item)
        {
            return item.AdjustedPriceLowDelta.HasValue || item.AdjustedPriceHighDelta.HasValue
                       ? PriceDeltaType.NonMarketAdjustment
                       : PriceDeltaType.Regular;
        }

        protected override bool IsPriceSeriesItemValid(PriceItemRange item)
        {
            var result = PriceSeriesValidator.ValidateRangeSeries(item, true);
            return result.IsValid;
        }

        private async Task<(PriceDeltaResult, PriceDeltaResult, PriceDeltaResult)> CalculatePriceDeltaBasedOnPreviousPrice(
            PriceItemInput priceItemInput,
            PriceItemRange currentPriceItem)
        {
            var previousPriceSeriesItem = await PriceSeriesItemsDomainService.GetPreviousPriceSeriesItem(priceItemInput.SeriesId, priceItemInput.AssessedDateTime) as PriceItemRange;

            var priceLowDeltaResult = deltaCalculator.GetPriceDelta(
                currentPriceItem?.PriceLow,
                previousPriceSeriesItem?.PriceLow);

            var priceMidDeltaResult = deltaCalculator.GetPriceDelta(
                currentPriceItem?.PriceMid,
                previousPriceSeriesItem?.PriceMid);

            var priceHighDeltaResult = deltaCalculator.GetPriceDelta(
                currentPriceItem?.PriceHigh,
                previousPriceSeriesItem?.PriceHigh);

            return (priceLowDeltaResult, priceMidDeltaResult, priceHighDeltaResult);
        }
    }
}
