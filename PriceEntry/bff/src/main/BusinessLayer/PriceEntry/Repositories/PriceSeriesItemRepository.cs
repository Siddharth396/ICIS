namespace BusinessLayer.PriceEntry.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using BusinessLayer.PriceEntry.Repositories.Models;

    using Infrastructure.MongoDB.Models;
    using Infrastructure.MongoDB.Repositories;
    using Infrastructure.MongoDB.Transactions;
    using Infrastructure.Services.Workflow;

    using MongoDB.Driver;
    using MongoDB.Driver.Linq;

    using Serilog;

    public class PriceSeriesItemRepository : BaseRepository<BasePriceItem>, IPriceSeriesItemRepository
    {
        public const string CollectionName = "price_series_items";

        private readonly ILogger logger;

        public PriceSeriesItemRepository(IMongoDatabase database, IMongoContext mongoContext, ILogger logger)
             : base(database, mongoContext)
        {
            this.logger = logger.ForContext<IPriceSeriesItemRepository>();
        }

        public override string DbCollectionName => CollectionName;

        public async Task<BasePriceItem?> GetPriceSeriesItem(string seriesId, DateTime assessedDateTime)
        {
            var result = await Query()
                            .Where(x => x.SeriesId == seriesId && x.AssessedDateTime == assessedDateTime)
                            .FirstOrDefaultAsync();
            return result;
        }

        public async Task<BasePriceItem?> GetPriceSeriesItem(
           List<string> seriesIds,
           DateTime assessedDateTime,
           DateTime? fulfilmentFrom,
           DateTime? fulfilmentUntil)
        {
            var result = await Query()
                .Where(x => seriesIds.Contains(x.SeriesId)
                        && x.AssessedDateTime == assessedDateTime
                        && (x.FulfilmentFromDate == null || x.FulfilmentFromDate == fulfilmentFrom)
                        && (x.FulfilmentUntilDate == null || x.FulfilmentUntilDate == fulfilmentUntil))
                .SingleOrDefaultAsync();

            return result;
        }

        public async Task<BasePriceItem> GetPreviousPriceSeriesItem(
            IEnumerable<string> seriesIds,
            DateTime assessedDateTime,
            DateTime? fulfilmentFrom = null,
            DateTime? fulfilmentUntil = null)
        {
            var result = await Query()
                            .Where(x => seriesIds.Contains(x.SeriesId)
                                        && x.AssessedDateTime < assessedDateTime
                                        && (fulfilmentFrom == null || x.FulfilmentFromDate == fulfilmentFrom)
                                        && (fulfilmentUntil == null || x.FulfilmentUntilDate == fulfilmentUntil)
                                        && (x.Status == WorkflowStatus.Published.Value || x.Status == WorkflowStatus.CorrectionPublished.Value))
                            .OrderByDescending(x => x.AssessedDateTime)
                            .FirstOrDefaultAsync();
            return result;
        }

        public async Task<BasePriceItem> SavePriceEntryGridData(BasePriceItem priceItem)
        {
            var filter = Builders<BasePriceItem>.Filter.Where(x => x.Id == priceItem.Id);
            await ReplaceOneAsync(filter, priceItem, new ReplaceOptions { IsUpsert = true });

            return priceItem;
        }

        public Task SetStatusFor(IEnumerable<string> priceSeriesItemIds, WorkflowStatus status, AuditInfo auditInfo)
        {
            var filter = Builders<BasePriceItem>.Filter.Where(x => priceSeriesItemIds.Contains(x.Id));
            var update = Builders<BasePriceItem>.Update
                            .Set(x => x.Status, status.Value)
                            .Set(x => x.LastModified, auditInfo);

            return UpdateManyAsync(filter, update);
        }

        public async Task<List<BasePriceItem>> GetPriceSeriesItems(IEnumerable<string> seriesIds, DateTime assessedDateTime)
        {
            var result = await Query()
                            .Where(x => seriesIds.Contains(x.SeriesId) && x.AssessedDateTime == assessedDateTime)
                            .ToListAsync();
            return result;
        }

        public async Task<List<BasePriceItem>> GetPriceSeriesItems(
           DateTime assessedDateTime,
           DateTime fulfilmentFrom,
           DateTime fulfilmentUntil)
        {
            var result = await Query()
                            .Where(
                                 x => x.AssessedDateTime == assessedDateTime
                                      && (x.FulfilmentFromDate == null || x.FulfilmentFromDate == fulfilmentFrom)
                                      && (x.FulfilmentUntilDate == null || x.FulfilmentUntilDate == fulfilmentUntil))
                            .ToListAsync();

            return result;
        }

        public Task SetPendingChangesStatusAsNullFor(IEnumerable<string> priceSeriesItemIds)
        {
            var filter = Builders<BasePriceItem>.Filter.Where(x => priceSeriesItemIds.Contains(x.Id));
            var update = Builders<BasePriceItem>.Update
                           .Set(x => x.PendingChanges, null);
            return UpdateManyAsync(filter, update);
        }

        public Task SetPendingChangesStatusFor(IEnumerable<string> priceSeriesItemIds, WorkflowStatus status, AuditInfo auditInfo)
        {
            var localLogger = logger.ForContext("Set Pending Change Status To", status);
            localLogger.Debug($"Price Series Items To Set Pending Changes Status For {string.Join(",", priceSeriesItemIds)}");

            var filter = Builders<BasePriceItem>.Filter.Where(x => priceSeriesItemIds.Contains(x.Id));
            var update = Builders<BasePriceItem>.Update
                           .Set(x => x.PendingChanges!.Status, status.Value)
                           .Set(x => x.PendingChanges!.LastModified, auditInfo);
            return UpdateManyAsync(filter, update);
        }

        public async Task<List<BasePriceItem>> GetPriceSeriesItems(DateTime assessedDateTime)
        {
            var result = await Query()
                            .Where(x => x.AssessedDateTime == assessedDateTime)
                            .ToListAsync();
            return result;
        }

        public async Task<BasePriceItem?> GetPriceSeriesItem(
            string seriesId,
            DateTime assessedDateTime,
            DateTime? fulfilmentFrom,
            DateTime? fulfilmentUntil)
        {
            var result = await Query()
                            .Where(
                                 x => x.SeriesId == seriesId
                                      && x.AssessedDateTime == assessedDateTime
                                      && (x.FulfilmentFromDate == null || x.FulfilmentFromDate == fulfilmentFrom)
                                      && (x.FulfilmentUntilDate == null || x.FulfilmentUntilDate == fulfilmentUntil))
                            .SingleOrDefaultAsync();
            return result;
        }

        public async Task<List<BasePriceItem>> GetPriceSeriesItemsBySeriesItemIds(IEnumerable<string> seriesItemIds)
        {
            var result = await Query()
                            .Where(x => seriesItemIds.Contains(x.Id))
                            .ToListAsync();
            return result;
        }

        public async Task<BasePriceItem?> GetPriceSeriesItemBySeriesItemId(string seriesItemId, DateTime assessedDateTime)
        {
            var result = await Query()
                            .Where(x => x.Id == seriesItemId && x.AssessedDateTime == assessedDateTime)
                            .SingleOrDefaultAsync();
            return result;
        }

        public async Task<string> GetPriceSeriesItemIdBySeriesId(string seriesId, DateTime assessedDateTime)
        {
            var result = await Query()
               .Where(x => x.SeriesId == seriesId && x.AssessedDateTime == assessedDateTime)
               .Select(x => x.Id)
               .SingleOrDefaultAsync();

            return result;
        }

        public async Task<BasePriceItem> GetNextPriceSeriesItem(
            IEnumerable<string> seriesIds,
            DateTime assessedDateTime,
            DateTime? fulfilmentFrom = null,
            DateTime? fulfilmentUntil = null)
        {
            var result = await Query()
                           .Where(x => seriesIds.Contains(x.SeriesId)
                            && x.AssessedDateTime > assessedDateTime
                            && (fulfilmentFrom == null || x.FulfilmentFromDate == fulfilmentFrom)
                            && (fulfilmentUntil == null || x.FulfilmentUntilDate == fulfilmentUntil)
                            && (x.Status == WorkflowStatus.Published.Value || x.Status == WorkflowStatus.CorrectionPublished.Value))
                           .OrderBy(x => x.AssessedDateTime)
                           .FirstOrDefaultAsync();
            return result;
        }

        public async Task<List<string>> GetPriceSeriesIdsForReferencePriceSeriesItems(List<string> seriesIds, DateTime assessedDateTime)
        {
            var result = await QueryOfType<PriceItemSingleValueWithReference>()
                            .Where(x => x.AssessedDateTime == assessedDateTime && x.ReferencePrice != null && x.ReferencePrice.SeriesId != null && seriesIds.Contains(x.ReferencePrice.SeriesId))
                            .Select(y => y.SeriesId)
                            .ToListAsync();

            return result;
        }

        public Task UpdatePeriods(Dictionary<string, string> priceSeriesItemPeriods)
        {
            if (priceSeriesItemPeriods.Count == 0)
            {
                return Task.CompletedTask;
            }

            var localLogger = logger.ForContext("Set Periods", priceSeriesItemPeriods);
            localLogger.Debug($"Price Series Items To Set Periods For {string.Join(",", priceSeriesItemPeriods.Keys)}");

            var writeModels = new List<WriteModel<BasePriceItem>>();

            foreach (var priceSeriesItemPeriod in priceSeriesItemPeriods)
            {
                var filter = Builders<BasePriceItem>.Filter.Eq(x => x.Id, priceSeriesItemPeriod.Key);

                var update = Builders<BasePriceItem>.Update.Set(x => x.PeriodLabel, priceSeriesItemPeriod.Value);

                var updateModel = new UpdateOneModel<BasePriceItem>(filter, update);

                writeModels.Add(updateModel);
            }

            return UpdateBulkAsync(writeModels);
        }

        protected override IMongoQueryable<BasePriceItem> Query()
        {
            var seriesItemTypeCodes = PriceItemDiscriminators.KnownTypeDiscriminators;

            var filter = Builders<BasePriceItem>.Filter.In("series_item_type_code", seriesItemTypeCodes);
            return base.Query().Where(doc => filter.Inject());
        }
    }
}
