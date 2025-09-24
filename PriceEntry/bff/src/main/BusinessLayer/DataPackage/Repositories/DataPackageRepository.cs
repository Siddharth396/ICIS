namespace BusinessLayer.DataPackage.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading.Tasks;

    using BusinessLayer.DataPackage.Models;
    using BusinessLayer.DataPackage.Repositories.Models;

    using Infrastructure.MongoDB.Models;
    using Infrastructure.MongoDB.Repositories;
    using Infrastructure.MongoDB.Transactions;
    using Infrastructure.Services.Workflow;

    using MongoDB.Driver;
    using MongoDB.Driver.Linq;

    public class DataPackageRepository : BaseRepository<DataPackage>, IDataPackageRepository
    {
        public const string CollectionName = "content_packages";

        public DataPackageRepository(IMongoDatabase database, IMongoContext mongoContext)
            : base(database, mongoContext)
        {
        }

        public override string DbCollectionName => CollectionName;

        public Task Save(DataPackage dataPackage)
        {
            var filter = Builders<DataPackage>.Filter.Where(x => x.Id == dataPackage.Id);
            return ReplaceOneAsync(filter, dataPackage, new ReplaceOptions { IsUpsert = true });
        }

        public async Task<DataPackage?> GetDataPackage(DataPackageId dataPackageId)
        {
            var result = await Query().Where(x => x.Id == dataPackageId.ToString()).FirstOrDefaultAsync();
            return result;
        }

        [ExcludeFromCodeCoverage(Justification = "Excluding from coverage until LNG is switched to the advanced workflow")]
        public async Task<List<DataPackage>> GetDataPackagesByPriceSeriesItemId(string priceSeriesItemId)
        {
            var result = await Query().Where(x => x.PriceSeriesItemGroups.Any(y => y.PriceSeriesItemIds.Contains(priceSeriesItemId)))
                                    .ToListAsync();

            return result.Select(x => x.PendingChangesOrOriginal()).ToList();
        }

        public async Task<int?> GetContentBlockVersion(string contentBlockId, DateTime assessedDateTime)
        {
            var contentBlockDefinition = await Query()
               .Where(x => x.AssessedDateTime == assessedDateTime && x.ContentBlock.Id == contentBlockId)
               .OrderByDescending(x => x.LastModified.Timestamp)
               .Select(x => x.ContentBlock)
               .FirstOrDefaultAsync();

            // Do like this to avoid returning the default(int) = 0 in case there are none, to ensure null is returned instead
            return contentBlockDefinition?.Version;
        }

        public Task SetDataPackagePendingStatus(
            DataPackageId dataPackageId,
            WorkflowStatus status,
            AuditInfo auditInfo,
            Commentary? commentary)
        {
            var filter = Builders<DataPackage>.Filter.Where(x => x.Id == dataPackageId.Value);
            var update = Builders<DataPackage>.Update
                .Set(x => x.PendingChanges!.Status, status.Value)
                .Set(x => x.PendingChanges!.LastModified, auditInfo)
                .Set(x => x.PendingChanges!.Commentary, commentary);

            return UpdateOneAsync(filter, update);
        }

        public Task SetDataPackagePendingStatusAsNull(DataPackageId dataPackageId)
        {
            var filter = Builders<DataPackage>.Filter.Where(x => x.Id == dataPackageId.Value);
            var update = Builders<DataPackage>.Update.Set(x => x.PendingChanges, null);
            return UpdateOneAsync(filter, update);
        }
    }
}
