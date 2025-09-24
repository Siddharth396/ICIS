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

    public class RegionalAvgDerivedPriceCalculator : IDerivedPriceCalculator
    {
        private readonly IPriceSeriesItemsDomainService priceSeriesItemsDomainService;

        public RegionalAvgDerivedPriceCalculator(IPriceSeriesItemsDomainService priceSeriesItemsDomainService)
        {
            this.priceSeriesItemsDomainService = priceSeriesItemsDomainService;
        }

        public async Task<decimal?> CalculatePrice(PriceSeries priceSeries, DateTime assessedDateTime, OperationType operationType)
        {
            var priceSeriesItems = await priceSeriesItemsDomainService.GetPriceSeriesItems(
                                       priceSeries.DerivationInputs!.Select(x => x.SeriesId).ToList(),
                                       assessedDateTime);

            priceSeriesItems = IfCorrectionGetPendingChanges(operationType, priceSeriesItems);

            var prices = priceSeriesItems
               .Select(x => x.GetPriceValue())
               .Where(x => x > 0)
               .ToList();

            return (prices.Count != priceSeries.DerivationInputs!.Count) ? null : prices.Average();
        }

        [ExcludeFromCodeCoverage(Justification = "Excluding from coverage until LNG is switched to the advanced workflow")]
        private static List<BasePriceItem> IfCorrectionGetPendingChanges(OperationType operationType, List<BasePriceItem> priceSeriesItems)
        {
            return OperationType.Correction.Equals(operationType) ? priceSeriesItems.Select(item => item.PendingChangesOrOriginal()).ToList() : priceSeriesItems;
        }
    }
}
