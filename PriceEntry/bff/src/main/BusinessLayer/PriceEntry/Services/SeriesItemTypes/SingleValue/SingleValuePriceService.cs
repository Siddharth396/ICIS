namespace BusinessLayer.PriceEntry.Services.SeriesItemTypes.SingleValue
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

    public class SingleValuePriceService : BasePriceItemService<PriceItemSingleValue>
    {
        private readonly IDeltaCalculator deltaCalculator;

        private readonly ILogger logger;

        public SingleValuePriceService(
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
            this.deltaCalculator = deltaCalculator;
            this.logger = logger.ForContext<SingleValuePriceService>();
        }

        protected override async Task<PriceItemSingleValue> BuildPriceItemForSaving(
            PriceItemInput priceItemInput,
            PriceItemSingleValue? current)
        {
            var localLogger = logger.ForContext("SeriesId", priceItemInput.SeriesId)
                   .ForContext("AssessedDateTime", priceItemInput.AssessedDateTime);

            localLogger.Information($"START : Building price item with priceItemInput {priceItemInput}");

            var newPriceItem = new PriceItemSingleValue
            {
                Price = priceItemInput.SeriesItem.Price,
                AdjustedPriceDelta = priceItemInput.SeriesItem.AdjustedPriceDelta
            };

            var priceDeltaResult = await CalculatePriceDeltaBasedOnPreviousPrice(
                                       priceItemInput,
                                       newPriceItem);

            newPriceItem.PriceDelta = priceDeltaResult?.PriceDelta;

            localLogger.Information($"END : New price item {newPriceItem}");

            return newPriceItem;
        }

        protected override PriceItemSingleValue DeltaCalculationForNextDate(
          PriceItemSingleValue priceSeriesItem, PriceItemSingleValue nextPriceSeriesItem)
        {
            nextPriceSeriesItem.PriceDelta = deltaCalculator.GetPriceDelta(nextPriceSeriesItem.Price, priceSeriesItem.Price).PriceDelta;
            return nextPriceSeriesItem;
        }

        protected override PriceDeltaType GetPriceDeltaType(PriceItemSingleValue item)
        {
            return item.AdjustedPriceDelta.HasValue ? PriceDeltaType.NonMarketAdjustment : PriceDeltaType.Regular;
        }

        protected override bool IsPriceSeriesItemValid(PriceItemSingleValue item)
        {
            var result = PriceSeriesValidator.ValidateSingleValueSeries(item, true);
            return result.IsValid;
        }

        private async Task<PriceDeltaResult> CalculatePriceDeltaBasedOnPreviousPrice(
            PriceItemInput priceItemInput,
            PriceItemSingleValue newPriceItem)
        {
            var previousPriceSeriesItem = await PriceSeriesItemsDomainService.GetPreviousPriceSeriesItem(priceItemInput.SeriesId, priceItemInput.AssessedDateTime) as PriceItemSingleValue;

            var priceDeltaResult = deltaCalculator.GetPriceDelta(newPriceItem?.Price, previousPriceSeriesItem?.Price);
            return priceDeltaResult;
        }
    }
}
