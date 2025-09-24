namespace BusinessLayer.PriceEntry.Services.SeriesItemTypes.CharterRateSingleValue
{
    using System.Diagnostics.CodeAnalysis;
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

    public class CharterRateSingleValuePriceService : BasePriceItemService<CharterRatePriceItemSingleValue>
    {
        private readonly IDeltaCalculator deltaCalculator;

        private readonly ILogger logger;

        public CharterRateSingleValuePriceService(
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
            this.logger = logger.ForContext<CharterRateSingleValuePriceService>();
        }

        protected override async Task<CharterRatePriceItemSingleValue> BuildPriceItemForSaving(
            PriceItemInput priceItemInput,
            CharterRatePriceItemSingleValue? current)
        {
            var assessedDateTime = priceItemInput.AssessedDateTime;

            var localLogger = logger.ForContext("SeriesId", priceItemInput.SeriesId)
               .ForContext("AssessedDateTime", assessedDateTime);

            localLogger.Information($"START : Building price item with priceItemInput {priceItemInput}");

            var newPriceItem = new CharterRatePriceItemSingleValue
            {
                Price = priceItemInput.SeriesItem.Price,
                DataUsed = priceItemInput.SeriesItem.DataUsed,
                AdjustedPriceDelta = priceItemInput.SeriesItem.AdjustedPriceDelta
            };

            var priceDeltaResult = await CalculatePriceDeltaBasedOnPreviousPrice(
                                       priceItemInput,
                                       newPriceItem);

            newPriceItem.PriceDelta = priceDeltaResult?.PriceDelta;

            localLogger.Information($"END : New price item {newPriceItem}");

            return newPriceItem;
        }

        [ExcludeFromCodeCoverage(Justification = "Charter rate price item single value is only for LNG and not covered under advance correction workflow")]
        protected override CharterRatePriceItemSingleValue DeltaCalculationForNextDate(
         CharterRatePriceItemSingleValue priceSeriesItem, CharterRatePriceItemSingleValue nextPriceSeriesItem)
        {
            nextPriceSeriesItem.PriceDelta = deltaCalculator.GetPriceDelta(nextPriceSeriesItem.Price, priceSeriesItem.Price).PriceDelta;
            return nextPriceSeriesItem;
        }

        protected override PriceDeltaType GetPriceDeltaType(CharterRatePriceItemSingleValue item)
        {
            return item.AdjustedPriceDelta.HasValue ? PriceDeltaType.NonMarketAdjustment : PriceDeltaType.Regular;
        }

        protected override bool IsPriceSeriesItemValid(CharterRatePriceItemSingleValue item)
        {
            var result = PriceSeriesValidator.ValidateCharterRateSingleValueSeries(item, true);
            return result.IsValid;
        }

        private async Task<PriceDeltaResult> CalculatePriceDeltaBasedOnPreviousPrice(
            PriceItemInput priceItemInput,
            CharterRatePriceItemSingleValue newPriceItem)
        {
            var previousPriceSeriesItem = await PriceSeriesItemsDomainService.GetPreviousPriceSeriesItem(priceItemInput.SeriesId, priceItemInput.AssessedDateTime) as CharterRatePriceItemSingleValue;

            var priceDeltaResult = deltaCalculator.GetPriceDelta(newPriceItem?.Price, previousPriceSeriesItem?.Price);
            return priceDeltaResult;
        }
    }
}
