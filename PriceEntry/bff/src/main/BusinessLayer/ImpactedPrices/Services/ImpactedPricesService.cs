namespace BusinessLayer.ImpactedPrices.Services
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading.Tasks;

    using BusinessLayer.ImpactedPrices.DTOs.Output;
    using BusinessLayer.PriceEntry.Services;
    using BusinessLayer.PriceEntry.Services.SeriesItemTypes.SingleValueWithReference;
    using BusinessLayer.PriceSeriesSelection.Repositories.Models;
    using BusinessLayer.PriceSeriesSelection.Services;

    [ExcludeFromCodeCoverage(Justification = "Excluding from coverage since works mostly with LNG in correction and LNG hasn't moved to Advanced workflow yet")]
    public class ImpactedPricesService : IImpactedPricesService
    {
        private readonly IPriceSeriesService priceSeriesService;

        private readonly IReferencePriceService referencePriceService;

        private readonly IDerivedPriceService derivedPriceService;

        public ImpactedPricesService(
            IPriceSeriesService priceSeriesService,
            IReferencePriceService referencePriceService,
            IDerivedPriceService derivedPriceService)
        {
            this.priceSeriesService = priceSeriesService;
            this.referencePriceService = referencePriceService;
            this.derivedPriceService = derivedPriceService;
        }

        public async Task<GetImpactedPricesResponse> GetImpactedPrices(string priceSeriesId, DateTime assessedDateTime)
        {
            var parentPriceSeries = await priceSeriesService.GetPriceSeriesById(priceSeriesId);

            if (parentPriceSeries == null)
            {
                return GetImpactedPricesResponse.Error(ErrorCodes.ParentPriceSeriesNotFound);
            }

            var impactedDerivedPriceSeriesIds = new HashSet<string>();
            var impactedReferencePriceSeriesIds = new HashSet<string>();
            var impactedCalculatedPriceSeriesIds = new HashSet<string>();

            await PopulateImpactedPriceIdsRecursive(
                impactedDerivedPriceSeriesIds,
                impactedReferencePriceSeriesIds,
                [priceSeriesId],
                assessedDateTime,
                []);

            var derivedPriceSeries = await priceSeriesService.GetPriceSeriesByIds(impactedDerivedPriceSeriesIds.ToList());

            foreach (var series in derivedPriceSeries.Where(series => !PriceSeries.IsValidDerivedSeries(series)))
            {
                impactedCalculatedPriceSeriesIds.Add(series.Id);
                impactedDerivedPriceSeriesIds.Remove(series.Id);
            }

            var impactedPrices = new ImpactedPrices
            {
                SeriesName = parentPriceSeries.SeriesShortName?.English ?? parentPriceSeries.SeriesName.English,
                ImpactedDerivedPriceSeriesIds = impactedDerivedPriceSeriesIds.ToList(),
                ImpactedReferencePriceSeriesIds = impactedReferencePriceSeriesIds.ToList(),
                ImpactedCalculatedPriceSeriesIds = impactedCalculatedPriceSeriesIds.ToList(),
            };

            return new GetImpactedPricesResponse { IsSuccess = true, ImpactedPrices = impactedPrices };
        }

        public async Task<IDictionary<string, bool>> HasImpactedPrices(
            List<string> priceSeriesIds,
            DateTime assessedDateTime)
        {
            var hasImpactedPricesDict = new Dictionary<string, bool>();

            var hasDerivedPricesDict = new Dictionary<string, bool>();

            foreach (var priceSeriesId in priceSeriesIds)
            {
                hasDerivedPricesDict[priceSeriesId] = false;

                var calculationServiceValidDerivedPriceSeriesIds = await priceSeriesService.GetCalculationServiceValidDerivedPriceSeriesIds(priceSeriesId);

                if (calculationServiceValidDerivedPriceSeriesIds.Count > 0)
                {
                    hasDerivedPricesDict[priceSeriesId] = true;
                }
                else
                {
                    var priceEntryValidDerivedPriceSeriesIds = await priceSeriesService.GetPriceEntryValidDerivedPriceSeriesIds(priceSeriesId);

                    hasDerivedPricesDict[priceSeriesId] = await AreAnyDerivedPriceImpacted(priceEntryValidDerivedPriceSeriesIds, priceSeriesId, assessedDateTime);
                }
            }

            var hasReferencingPricesDict = await referencePriceService.HasReferencePriceSeriesItems(
                priceSeriesIds,
                assessedDateTime);

            foreach (var priceSeriesId in priceSeriesIds)
            {
                hasImpactedPricesDict[priceSeriesId] =
                    hasDerivedPricesDict[priceSeriesId] || hasReferencingPricesDict[priceSeriesId];
            }

            return hasImpactedPricesDict;
        }

        private async Task PopulateImpactedPriceIdsRecursive(
            HashSet<string> impactedDerivedPriceSeriesIds,
            HashSet<string> impactedReferencePriceSeriesIds,
            List<string> priceSeriesIds,
            DateTime assessedDateTime,
            HashSet<string> visitedPriceSeriesIds)
        {
            var filteredPriceSeriesIds = priceSeriesIds.Where(id => !visitedPriceSeriesIds.Contains(id)).ToList();

            if (filteredPriceSeriesIds.Count == 0)
            {
                return;
            }

            priceSeriesIds = filteredPriceSeriesIds;

            visitedPriceSeriesIds.UnionWith(priceSeriesIds);

            List<string> allDerivedPriceSeriesIds = [];

            foreach (var priceSeriesId in priceSeriesIds)
            {
                var priceEntryValidDerivedPriceSeriesIds =
                    await priceSeriesService.GetPriceEntryValidDerivedPriceSeriesIds(priceSeriesId);

                foreach (var derivedSeriesId in priceEntryValidDerivedPriceSeriesIds)
                {
                    var isDerivedPriceImpacted = await IsDerivedPriceImpactedBasedOnPeriod(priceSeriesId, derivedSeriesId, assessedDateTime);

                    if (isDerivedPriceImpacted)
                    {
                        allDerivedPriceSeriesIds.Add(derivedSeriesId);
                    }
                }

                var calculationServiceValidDerivedPriceSeriesIds =
                    await priceSeriesService.GetCalculationServiceValidDerivedPriceSeriesIds(priceSeriesId);

                allDerivedPriceSeriesIds.AddRange(calculationServiceValidDerivedPriceSeriesIds);
            }

            var referencePriceSeriesIds = await referencePriceService.GetPriceSeriesIdsForReferencePriceSeriesItems(priceSeriesIds, assessedDateTime);

            impactedDerivedPriceSeriesIds.UnionWith(allDerivedPriceSeriesIds);
            impactedReferencePriceSeriesIds.UnionWith(referencePriceSeriesIds);

            var allPriceSeriesIds = allDerivedPriceSeriesIds.Concat(referencePriceSeriesIds).ToList();

            if (allPriceSeriesIds.Count > 0)
            {
                await PopulateImpactedPriceIdsRecursive(
                    impactedDerivedPriceSeriesIds,
                    impactedReferencePriceSeriesIds,
                    allPriceSeriesIds,
                    assessedDateTime,
                    visitedPriceSeriesIds);
            }
        }

        private async Task<bool> IsDerivedPriceImpactedBasedOnPeriod(string priceSeriesId, string derivedPriceSeriesId, DateTime assessedDateTime)
        {
            var derivedPriceSeries = await priceSeriesService.GetPriceSeriesById(derivedPriceSeriesId);

            var isDerivedPriceSeriesImpacted = await derivedPriceService.IsDerivedSeriesImpacted(priceSeriesId, derivedPriceSeries, assessedDateTime);

            return isDerivedPriceSeriesImpacted;
        }

        private async Task<bool> AreAnyDerivedPriceImpacted(List<string> priceEntryValidDerivedPriceSeriesIds, string priceSeriesId, DateTime assessedDateTime)
        {
            foreach (var derivedSeriesId in priceEntryValidDerivedPriceSeriesIds)
            {
                var isDerivedPriceImpacted = await IsDerivedPriceImpactedBasedOnPeriod(priceSeriesId, derivedSeriesId, assessedDateTime);

                if (isDerivedPriceImpacted)
                {
                    return true;
                }
            }

            return false;
        }

        private static class ErrorCodes
        {
            public const string ParentPriceSeriesNotFound = "PARENT_PRICE_SERIES_NOT_FOUND";
        }
    }
}
