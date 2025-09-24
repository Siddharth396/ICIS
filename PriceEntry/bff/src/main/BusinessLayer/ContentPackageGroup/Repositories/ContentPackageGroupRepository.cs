namespace BusinessLayer.ContentPackageGroup.Repositories
{
    using System.Linq;
    using System.Threading.Tasks;

    using BusinessLayer.ContentPackageGroup.Repositories.Models;
    using BusinessLayer.ContentPackageGroup.ValueObjects;

    using Infrastructure.MongoDB.Repositories;
    using Infrastructure.MongoDB.Transactions;

    using MongoDB.Driver;
    using MongoDB.Driver.Linq;

    public class ContentPackageGroupRepository : BaseRepository<ContentPackageGroup>
    {
        public const string CollectionName = "content_package_groups";

        public ContentPackageGroupRepository(IMongoDatabase database, IMongoContext mongoContext)
            : base(database, mongoContext)
        {
        }

        public override string DbCollectionName => CollectionName;

        public Task UpsertContentPackageGroup(
               ContentPackageGroup contentPackageGroup)
        {
            var filter = Builders<ContentPackageGroup>.Filter.Eq(g => g.SequenceId, contentPackageGroup.SequenceId);
            return ReplaceOneAsync(filter, contentPackageGroup, new ReplaceOptions { IsUpsert = true });
        }

        public async Task<ContentPackageGroup?> GetBySequenceId(SequenceId sequenceId)
        {
            return await Query().Where(x => x.SequenceId == sequenceId.Value).SingleOrDefaultAsync();
        }

        public async Task<string> GetSequenceId(string contentBlockId, int version)
        {
            return await Query()
                        .Where(x => x.ContentPackageGroupContentBlocks.Any(x => x.ContentBlockId == contentBlockId && x.Version == version))
                        .Select(x => x.SequenceId)
                        .SingleAsync();
        }
    }
}
