namespace BusinessLayer.PriceEntry.Services.Calculators.DerivedPrice
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading.Tasks;

    using BusinessLayer.PriceEntry.Repositories.Models;
    using BusinessLayer.PriceSeriesSelection.Repositories.Models;

    using Infrastructure.Services.Workflow;

    public class PeriodAvgDerivedPriceCalculator : IDerivedPriceCalculator
    {
        private readonly IHalfMonthPriceSeriesItemService halfMonthPriceSeriesItemService;

        private readonly IPriceSeriesItemsDomainService priceSeriesItemsDomainService;

        public PeriodAvgDerivedPriceCalculator(
            IHalfMonthPriceSeriesItemService halfMonthPriceSeriesItemService,
            IPriceSeriesItemsDomainService priceSeriesItemsDomainService)
        {
            this.halfMonthPriceSeriesItemService = halfMonthPriceSeriesItemService;
            this.priceSeriesItemsDomainService = priceSeriesItemsDomainService;
        }

        public async Task<decimal?> CalculatePrice(PriceSeries priceSeries, DateTime assessedDateTime, OperationType operationType)
        {
            var priceSeriesItems = await priceSeriesItemsDomainService.GetPriceSeriesItems(
                priceSeries.DerivationInputs!.Select(x => x.SeriesId).ToList(), assessedDateTime);

            priceSeriesItems = IfCorrectionGetPendingChanges(operationType, priceSeriesItems);

            var (halfMonthPeriodOne, halfMonthPeriodTwo) = await halfMonthPriceSeriesItemService.GetHalfMonthPriceSeriesItems(priceSeriesItems, priceSeries, assessedDateTime);

            var prices = new List<decimal?>();

            if (halfMonthPeriodOne != null && halfMonthPeriodTwo != null)
            {
                var periodOnePrice = halfMonthPeriodOne.GetPriceValue();
                if (periodOnePrice != null)
                {
                    prices.Add(periodOnePrice);
                }

                var periodTwoPrice = halfMonthPeriodTwo.GetPriceValue();
                if (periodTwoPrice != null)
                {
                    prices.Add(periodTwoPrice);
                }
            }

            return prices.Count != 2 ? null : prices.Average();
        }

        [ExcludeFromCodeCoverage(Justification = "Excluding from coverage until LNG is switched to the advanced workflow")]
        private static List<BasePriceItem> IfCorrectionGetPendingChanges(OperationType operationType, List<BasePriceItem> priceSeriesItems)
        {
            return OperationType.Correction.Equals(operationType) ? priceSeriesItems.Select(item => item.PendingChangesOrOriginal()).ToList() : priceSeriesItems;
        }
    }
}
