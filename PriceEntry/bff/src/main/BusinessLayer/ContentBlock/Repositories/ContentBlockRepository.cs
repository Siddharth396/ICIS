namespace BusinessLayer.ContentBlock.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using BusinessLayer.ContentBlock.Repositories.Models;
    using BusinessLayer.ContentBlock.ValueObjects;

    using Infrastructure.MongoDB.Repositories;
    using Infrastructure.MongoDB.Transactions;

    using MongoDB.Driver;
    using MongoDB.Driver.Linq;

    public class ContentBlockRepository : BaseRepository<ContentBlock>
    {
        public const string CollectionName = "content_block_definitions";

        public ContentBlockRepository(IMongoDatabase database, IMongoContext mongoContext)
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

        public Task SaveContentBlock(ContentBlock contentBlock)
        {
            return InsertOneAsync(contentBlock);
        }

        public async Task<ContentBlock?> GetContentBlockByVersion(string contentBlockId, Version version)
        {
            return await Query()
               .Where(x => x.ContentBlockId == contentBlockId && x.Version == version.Value)
               .FirstOrDefaultAsync();
        }

        public async Task<ContentBlock?> GetLatestContentBlock(string contentBlockId)
        {
            return await Query()
                      .Where(x => x.ContentBlockId == contentBlockId)
                      .OrderByDescending(x => x.Version)
                      .FirstOrDefaultAsync();
        }

        public async Task<List<string>> GetContentBlockIds(string priceSeriesId)
        {
            return await Query()
                      .Where(
                           x => x.PriceSeriesGrids != null
                                    && x.PriceSeriesGrids.Any(
                                        y => y.PriceSeriesIds != null && y.PriceSeriesIds.Contains(priceSeriesId)))
                      .Select(x => x.ContentBlockId)
                      .Distinct()
                      .ToListAsync();
        }
    }
}
