namespace BusinessLayer.PriceEntry.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using BusinessLayer.PriceEntry.Repositories.Models;

    using Infrastructure.MongoDB.Models;
    using Infrastructure.Services.Workflow;

    public interface IPriceSeriesItemsDomainService
    {
        Task CorrectionCancelled(IEnumerable<string> priceSeriesItemIds);

        Task<List<string>> CorrectionPublished(IEnumerable<string> priceSeriesItemIds, AuditInfo auditInfo);

        Task<BasePriceItem> GetNextPriceSeriesItem(
            List<string> seriesIds,
            DateTime assessedDateTime,
            DateTime? fulfilmentFrom,
            DateTime? fulfilmentUntil);

        Task<BasePriceItem?> GetPriceSeriesItem(
            string seriesId,
            DateTime assessedDateTime,
            DateTime? fulfilmentFrom,
            DateTime? fulfilmentUntil);

        Task<BasePriceItem?> GetPriceSeriesItem(string seriesId, DateTime assessedDateTime);

        Task<BasePriceItem?> GetPriceSeriesItem(
            List<string> priceSeriesIds,
            DateTime assessedDateTime,
            DateTime? fulfilmentFrom,
            DateTime? fulfilmentUntil);

        Task<BasePriceItem?> GetPriceSeriesItemBySeriesItemId(string priceSeriesItemId, DateTime assessedDateTime);

        Task<string> GetPriceSeriesItemIdBySeriesId(string seriesId, DateTime assessedDateTime);

        Task<List<BasePriceItem>> GetPriceSeriesItems(List<string> priceSeriesIds, DateTime assessedDateTime);

        Task<List<BasePriceItem>> GetPriceSeriesItems(
            DateTime assessedDateTime,
            DateTime fulfilmentFrom,
            DateTime fulfilmentUntil);

        Task<List<BasePriceItem>> GetPriceSeriesItems(DateTime assessedDateTime);

        Task<List<BasePriceItem>> GetPriceSeriesItemsBySeriesItemIds(List<string> priceSeriesItemIds);

        Task<BasePriceItem> SavePriceSeriesItem(BasePriceItem item);

        Task StatusChanged(IEnumerable<string> priceSeriesItemIds, WorkflowStatus status, AuditInfo auditInfo);

        Task StatusChangedDuringCorrection(
            IEnumerable<string> priceSeriesItemIds,
            WorkflowStatus status,
            AuditInfo auditInfo);

        Task<List<string>> GetPriceSeriesIdsForReferencePriceSeriesItems(List<string> seriesIds, DateTime assessedDateTime);

        Task<(DateTime? FulfilmentFrom, DateTime? FulfilmentUntil)> GetFulfilmentDates(string priceSeriesId, DateTime assessedDateTime, BasePriceItem? current);

        Task UpdatePeriods(IEnumerable<string> priceSeriesIds, DateTime assessedDateTime);

        Task<BasePriceItem?> GetPreviousPriceSeriesItem(
            string seriesId,
            DateTime assessedDateTime);
    }
}
