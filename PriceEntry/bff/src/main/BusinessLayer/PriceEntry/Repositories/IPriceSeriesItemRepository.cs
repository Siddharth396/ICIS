namespace BusinessLayer.PriceEntry.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using BusinessLayer.PriceEntry.Repositories.Models;

    using Infrastructure.MongoDB.Models;
    using Infrastructure.Services.Workflow;

    public interface IPriceSeriesItemRepository
    {
        Task<BasePriceItem?> GetPriceSeriesItem(string seriesId, DateTime assessedDateTime);

        Task<List<BasePriceItem>> GetPriceSeriesItems(IEnumerable<string> seriesIds, DateTime assessedDateTime);

        Task<BasePriceItem> GetPreviousPriceSeriesItem(
            IEnumerable<string> seriesIds,
            DateTime assessedDateTime,
            DateTime? fulfilmentFrom = null,
            DateTime? fulfilmentUntil = null);

        Task<BasePriceItem> SavePriceEntryGridData(BasePriceItem priceItem);

        Task SetStatusFor(IEnumerable<string> priceSeriesItemIds, WorkflowStatus status, AuditInfo auditInfo);

        Task<BasePriceItem?> GetPriceSeriesItem(
            List<string> seriesIds,
            DateTime assessedDateTime,
            DateTime? fulfilmentFrom,
            DateTime? fulfilmentUntil);

        Task<List<BasePriceItem>> GetPriceSeriesItems(
           DateTime assessedDateTime,
           DateTime fulfilmentFrom,
           DateTime fulfilmentUntil);

        Task SetPendingChangesStatusAsNullFor(IEnumerable<string> priceSeriesItemIds);

        Task SetPendingChangesStatusFor(IEnumerable<string> priceSeriesItemIds, WorkflowStatus status, AuditInfo auditInfo);

        Task<List<BasePriceItem>> GetPriceSeriesItems(DateTime assessedDateTime);

        Task<BasePriceItem?> GetPriceSeriesItem(
            string seriesId,
            DateTime assessedDateTime,
            DateTime? fulfilmentFrom,
            DateTime? fulfilmentUntil);

        Task<List<BasePriceItem>> GetPriceSeriesItemsBySeriesItemIds(IEnumerable<string> seriesItemIds);

        Task<BasePriceItem?> GetPriceSeriesItemBySeriesItemId(string seriesItemId, DateTime assessedDateTime);

        Task<string> GetPriceSeriesItemIdBySeriesId(string seriesId, DateTime assessedDateTime);

        Task<BasePriceItem> GetNextPriceSeriesItem(
            IEnumerable<string> seriesIds,
            DateTime assessedDateTime,
            DateTime? fulfilmentFrom = null,
            DateTime? fulfilmentUntil = null);

        Task<List<string>> GetPriceSeriesIdsForReferencePriceSeriesItems(List<string> seriesIds, DateTime assessedDateTime);

        Task UpdatePeriods(Dictionary<string, string> priceSeriesItemPeriods);
    }
}
