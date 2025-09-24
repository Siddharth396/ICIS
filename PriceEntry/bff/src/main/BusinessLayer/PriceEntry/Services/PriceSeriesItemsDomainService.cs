namespace BusinessLayer.PriceEntry.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using BusinessLayer.Helpers;
    using BusinessLayer.PriceEntry.Repositories;
    using BusinessLayer.PriceEntry.Repositories.Models;
    using BusinessLayer.PriceEntry.ValueObjects;
    using BusinessLayer.PriceSeries.Services;

    using Infrastructure.Attributes.BusinessAnnotations;
    using Infrastructure.MongoDB.Models;
    using Infrastructure.Services.Workflow;

    [DomainService]
    public class PriceSeriesItemsDomainService : IPriceSeriesItemsDomainService
    {
        private readonly IPriceSeriesItemRepository priceSeriesItemRepository;

        private readonly IAbsolutePeriodDomainService absolutePeriodDomainService;

        private readonly IPriceSeriesDomainService priceSeriesDomainService;

        private readonly PriceEntryRepository priceEntryRepository;

        public PriceSeriesItemsDomainService(
            IPriceSeriesItemRepository priceSeriesItemRepository,
            IAbsolutePeriodDomainService absolutePeriodDomainService,
            IPriceSeriesDomainService priceSeriesDomainService,
            PriceEntryRepository priceEntryRepository)
        {
            this.priceSeriesItemRepository = priceSeriesItemRepository;
            this.absolutePeriodDomainService = absolutePeriodDomainService;
            this.priceSeriesDomainService = priceSeriesDomainService;
            this.priceEntryRepository = priceEntryRepository;
        }

        public Task CorrectionCancelled(IEnumerable<string> priceSeriesItemIds)
        {
            return priceSeriesItemRepository.SetPendingChangesStatusAsNullFor(priceSeriesItemIds);
        }

        public async Task<List<string>> CorrectionPublished(IEnumerable<string> priceSeriesItemIds, AuditInfo auditInfo)
        {
            var priceSeriesItemsToUpdate = await priceSeriesItemRepository.GetPriceSeriesItemsBySeriesItemIds(priceSeriesItemIds);

            foreach (var item in priceSeriesItemsToUpdate)
            {
                var newItem = item.PendingChanges;
                var previousVersionsList = item.PreviousVersions ?? new List<BasePriceItem>();
                item.PreviousVersions = null;
                item.PendingChanges = null;

                newItem!.LastModified = auditInfo;
                newItem.PendingChanges = null;
                newItem.Status = WorkflowStatus.CorrectionPublished.Value;
                newItem.PreviousVersions = [item, .. previousVersionsList];

                await priceSeriesItemRepository.SavePriceEntryGridData(newItem);
            }

            return priceSeriesItemsToUpdate.Select(x => x.Id).ToList();
        }

        public Task<BasePriceItem> GetNextPriceSeriesItem(
            List<string> seriesIds,
            DateTime assessedDateTime,
            DateTime? fulfilmentFrom,
            DateTime? fulfilmentUntil)
        {
            return priceSeriesItemRepository.GetNextPriceSeriesItem(
                seriesIds,
                assessedDateTime,
                fulfilmentFrom,
                fulfilmentUntil);
        }

        public Task<BasePriceItem?> GetPriceSeriesItem(
            List<string> priceSeriesIds,
            DateTime assessedDateTime,
            DateTime? fulfilmentFrom,
            DateTime? fulfilmentUntil)
        {
            return priceSeriesItemRepository.GetPriceSeriesItem(
                priceSeriesIds,
                assessedDateTime,
                fulfilmentFrom,
                fulfilmentUntil);
        }

        public Task<BasePriceItem?> GetPriceSeriesItem(string seriesId, DateTime assessedDateTime)
        {
            return priceSeriesItemRepository.GetPriceSeriesItem(seriesId, assessedDateTime);
        }

        public async Task<BasePriceItem?> GetPriceSeriesItem(
            string seriesId,
            DateTime assessedDateTime,
            DateTime? fulfilmentFrom,
            DateTime? fulfilmentUntil)
        {
            return await priceSeriesItemRepository.GetPriceSeriesItem(
                       seriesId,
                       assessedDateTime,
                       fulfilmentFrom,
                       fulfilmentUntil);
        }

        public Task<BasePriceItem?> GetPriceSeriesItemBySeriesItemId(string priceSeriesItemId, DateTime assessedDateTime)
        {
            return priceSeriesItemRepository.GetPriceSeriesItemBySeriesItemId(priceSeriesItemId, assessedDateTime);
        }

        public Task<string> GetPriceSeriesItemIdBySeriesId(string seriesId, DateTime assessedDateTime)
        {
            return priceSeriesItemRepository.GetPriceSeriesItemIdBySeriesId(seriesId, assessedDateTime);
        }

        public Task<List<BasePriceItem>> GetPriceSeriesItems(DateTime assessedDateTime)
        {
            return priceSeriesItemRepository.GetPriceSeriesItems(assessedDateTime);
        }

        public Task<List<BasePriceItem>> GetPriceSeriesItems(
            DateTime assessedDateTime,
            DateTime fulfilmentFrom,
            DateTime fulfilmentUntil)
        {
            return priceSeriesItemRepository.GetPriceSeriesItems(assessedDateTime, fulfilmentFrom, fulfilmentUntil);
        }

        public Task<List<BasePriceItem>> GetPriceSeriesItems(List<string> priceSeriesIds, DateTime assessedDateTime)
        {
            return priceSeriesItemRepository.GetPriceSeriesItems(priceSeriesIds, assessedDateTime);
        }

        public Task<List<BasePriceItem>> GetPriceSeriesItemsBySeriesItemIds(List<string> priceSeriesItemIds)
        {
            return priceSeriesItemRepository.GetPriceSeriesItemsBySeriesItemIds(priceSeriesItemIds);
        }

        public Task<BasePriceItem> SavePriceSeriesItem(BasePriceItem item)
        {
            return priceSeriesItemRepository.SavePriceEntryGridData(item);
        }

        public Task StatusChanged(IEnumerable<string> priceSeriesItemIds, WorkflowStatus status, AuditInfo auditInfo)
        {
            return priceSeriesItemRepository.SetStatusFor(priceSeriesItemIds, status, auditInfo);
        }

        public Task StatusChangedDuringCorrection(
            IEnumerable<string> priceSeriesItemIds,
            WorkflowStatus status,
            AuditInfo auditInfo)
        {
            return priceSeriesItemRepository.SetPendingChangesStatusFor(priceSeriesItemIds, status, auditInfo);
        }

        public Task<List<string>> GetPriceSeriesIdsForReferencePriceSeriesItems(List<string> seriesIds, DateTime assessedDateTime)
        {
            return priceSeriesItemRepository.GetPriceSeriesIdsForReferencePriceSeriesItems(seriesIds, assessedDateTime);
        }

        public async Task<BasePriceItem?> GetPreviousPriceSeriesItem(
            string seriesId,
            DateTime assessedDateTime)
        {
            var priceSeries = await priceSeriesDomainService.GetPriceSeriesById(seriesId);

            return await GetPreviousPriceSeriesItem(
                        priceSeries.Id,
                        assessedDateTime,
                        priceSeries?.GetPriceSeriesLink(),
                        priceSeries?.HasRelativeDefPerspective());
        }

        public async Task<(DateTime?, DateTime?)> GetFulfilmentDates(string priceSeriesId, DateTime assessedDateTime, BasePriceItem? current)
        {
            return await GetFulfilmentDates(priceSeriesId, assessedDateTime, current?.FulfilmentFromDate, current?.FulfilmentUntilDate);
        }

        public async Task UpdatePeriods(IEnumerable<string> priceSeriesIds, DateTime assessedDateTime)
        {
            var priceSeriesAggregations = await priceEntryRepository.GetPriceSeriesDetails(priceSeriesIds, assessedDateTime);
            var referencePriceSeriesAggregations = priceSeriesAggregations.Where(x => ReferencePeriodCode.IsAllowed(x.ReferencePeriod?.Code)).ToList();

            var absolutePeriods = await absolutePeriodDomainService.GetAbsolutePeriods(referencePriceSeriesAggregations, assessedDateTime);

            var priceSeriesItemPeriods = new Dictionary<string, string>();

            foreach (var priceSeriesAggregation in referencePriceSeriesAggregations)
            {
                var absolutePeriod = absolutePeriodDomainService.FilterAbsolutePeriod(absolutePeriods, priceSeriesAggregation, assessedDateTime);

                var expectedPeriodLabel = absolutePeriod?.Label ?? string.Empty;

                var existingPeriodLabel = priceSeriesAggregation.PriceSeriesItem.PeriodLabel ?? string.Empty;

                if (!existingPeriodLabel.Equals(expectedPeriodLabel, StringComparison.OrdinalIgnoreCase))
                {
                    priceSeriesItemPeriods.Add(priceSeriesAggregation.PriceSeriesItem.Id!, expectedPeriodLabel);
                }
            }

            await priceSeriesItemRepository.UpdatePeriods(priceSeriesItemPeriods);
        }

        private async Task<(DateTime? FulfilmentFrom, DateTime? FulfilmentUntil)> GetFulfilmentDates(
            string priceSeriesId,
            DateTime assessedDateTime,
            DateTime? currentFulfilmentFromDate,
            DateTime? currentFulfilmentUntilDate)
        {
            if (currentFulfilmentFromDate != null && currentFulfilmentUntilDate != null)
            {
                return (currentFulfilmentFromDate, currentFulfilmentUntilDate);
            }

            var absolutePeriod = await absolutePeriodDomainService.GetAbsolutePeriod(priceSeriesId, assessedDateTime);

            DateTime? fulfilmentFrom = null;
            DateTime? fulfilmentUntil = null;

            if (RelativeFulfilmentPeriodCode.IsAllowed(absolutePeriod?.PeriodCode))
            {
                fulfilmentFrom = UtcDateTime.GetUtcDateTime(absolutePeriod?.FromDate);
                fulfilmentUntil = UtcDateTime.GetUtcDateTime(absolutePeriod?.UntilDate);
            }

            return (fulfilmentFrom, fulfilmentUntil);
        }

        private async Task<BasePriceItem?> GetPreviousPriceSeriesItem(
            string seriesId,
            DateTime assessedDateTime,
            PriceSeriesLink? seriesLink,
            bool? hasRelDefPerspective,
            DateTime? currentFulfilmentFromDate = null,
            DateTime? currentFulfilmentUntilDate = null)
        {
            DateTime? fulfilmentFrom = null;
            DateTime? fulfilmentUntil = null;

            List<string> seriesIds = [seriesId];

            if (hasRelDefPerspective.HasValue && !hasRelDefPerspective.Value)
            {
                if (seriesLink != null)
                {
                    seriesIds.Add(seriesLink.ObjectSeriesId);
                }

                (fulfilmentFrom, fulfilmentUntil) = await GetFulfilmentDates(
                                                        seriesId,
                                                        assessedDateTime,
                                                        currentFulfilmentFromDate,
                                                        currentFulfilmentUntilDate);
            }

            return await priceSeriesItemRepository.GetPreviousPriceSeriesItem(
                    seriesIds,
                    assessedDateTime,
                    fulfilmentFrom,
                    fulfilmentUntil);
        }
    }
}
