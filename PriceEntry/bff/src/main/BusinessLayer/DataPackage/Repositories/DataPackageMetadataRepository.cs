namespace BusinessLayer.DataPackage.Repositories
{
    using System.Threading.Tasks;

    using BusinessLayer.DataPackage.Repositories.Models;
    using BusinessLayer.PriceEntry.ValueObjects;

    using Infrastructure.MongoDB.Repositories;
    using Infrastructure.MongoDB.Transactions;

    using MongoDB.Driver;
    using MongoDB.Driver.Linq;

    public class DataPackageMetadataRepository : BaseRepository<DataPackageMetadata>
    {
        public const string CollectionName = "data_package_metadata";

        public DataPackageMetadataRepository(IMongoDatabase database, IMongoContext mongoContext)
            : base(database, mongoContext)
        {
        }

        public override string DbCollectionName => CollectionName;

        public Task Save(DataPackageMetadata dataPackageMetadata)
        {
            var filters = Builders<DataPackageMetadata>.Filter.And(
                Builders<DataPackageMetadata>.Filter.Eq(
                    d => d.DataPackageIdMetadata.ContentBlockId,
                    dataPackageMetadata.DataPackageIdMetadata.ContentBlockId),
                Builders<DataPackageMetadata>.Filter.Eq(
                    d => d.DataPackageIdMetadata.ContentBlockVersion,
                    dataPackageMetadata.DataPackageIdMetadata.ContentBlockVersion),
                Builders<DataPackageMetadata>.Filter.Eq(
                    d => d.DataPackageIdMetadata.AssessedDateTime,
                    dataPackageMetadata.DataPackageIdMetadata.AssessedDateTime));

            return ReplaceOneAsync(
                filter: filters,
                replacement: dataPackageMetadata,
                options: new ReplaceOptions { IsUpsert = true });
        }

        public async Task<DataPackageMetadata?> GetBy(DataPackageKey dataPackageKey)
        {
            return await Query()
                      .Where(
                           x => x.DataPackageIdMetadata.ContentBlockId == dataPackageKey.ContentBlockId
                                && x.DataPackageIdMetadata.ContentBlockVersion == dataPackageKey.ContentBlockVersion.Value
                                && x.DataPackageIdMetadata.AssessedDateTime == dataPackageKey.AssessedDateTime)
                      .SingleOrDefaultAsync();
        }
    }
}
