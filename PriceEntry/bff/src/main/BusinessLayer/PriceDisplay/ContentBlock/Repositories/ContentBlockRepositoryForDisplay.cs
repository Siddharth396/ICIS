namespace BusinessLayer.PriceDisplay.ContentBlock.Repositories
{
    using System.Threading.Tasks;

    using BusinessLayer.ContentBlock.ValueObjects;
    using BusinessLayer.PriceDisplay.ContentBlock.Repositories.Models;

    using Infrastructure.MongoDB.Repositories;
    using Infrastructure.MongoDB.Transactions;

    using MongoDB.Driver;
    using MongoDB.Driver.Linq;

    public class ContentBlockRepositoryForDisplay : BaseRepository<ContentBlockForDisplay>
    {
        public const string CollectionName = "price_display.content_block_definitions";

        public ContentBlockRepositoryForDisplay(IMongoDatabase database, IMongoContext mongoContext)
            : base(database, mongoContext)
        {
        }

        public override string DbCollectionName => CollectionName;

        public async Task<Version> GetLatestVersionForContentBlock(string contentBlockId)
        {
            var versionNumber = await Query()
               .Where(x => x.ContentBlockId == contentBlockId)
               .OrderByDescending(x => x.Version)
               .Select(x => x.Version)
               .FirstOrDefaultAsync();

            return Version.From(versionNumber);
        }

        public Task SaveContentBlock(ContentBlockForDisplay contentBlock)
        {
            return InsertOneAsync(contentBlock);
        }

        public async Task<ContentBlockForDisplay?> GetContentBlockByVersion(string contentBlockId, Version version)
        {
            return await Query()
               .Where(x => x.ContentBlockId == contentBlockId && x.Version == version.Value)
               .SingleOrDefaultAsync();
        }

        public async Task<ContentBlockForDisplay?> GetLatestContentBlock(string contentBlockId)
        {
            return await Query()
                      .Where(x => x.ContentBlockId == contentBlockId)
                      .OrderByDescending(x => x.Version)
                      .FirstOrDefaultAsync();
        }
    }
}
