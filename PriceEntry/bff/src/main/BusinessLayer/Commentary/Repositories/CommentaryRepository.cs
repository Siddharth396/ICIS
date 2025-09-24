namespace BusinessLayer.Commentary.Repositories
{
    using System;
    using System.Threading.Tasks;

    using BusinessLayer.Commentary.Repositories.Models;

    using Infrastructure.MongoDB.Repositories;
    using Infrastructure.MongoDB.Transactions;

    using MongoDB.Driver;
    using MongoDB.Driver.Linq;

    public class CommentaryRepository : BaseRepository<Commentary>
    {
        public const string CollectionName = "commentaries";

        public CommentaryRepository(IMongoDatabase mongoDatabase, IMongoContext mongoContext)
            : base(mongoDatabase, mongoContext)
        {
        }

        public override string DbCollectionName => CollectionName;

        public async Task Delete(string contentBlockId, string commentaryId)
        {
            var filter = Builders<Commentary>.Filter.Where(
                x => x.ContentBlockId == contentBlockId && x.CommentaryId == commentaryId);
            await DeleteOneAsync(filter);
        }

        public async Task<Commentary?> GetCommentary(string contentBlockId, DateTime assessedDateTime)
        {
            var result = await Query()
                            .Where(x => x.ContentBlockId == contentBlockId && x.AssessedDateTime == assessedDateTime)
                            .FirstOrDefaultAsync();
            return result;
        }

        public async Task<Commentary> SaveCommentary(Commentary commentary)
        {
            var filter = Builders<Commentary>.Filter.Where(
                x => x.CommentaryId == commentary.CommentaryId && x.ContentBlockId == commentary.ContentBlockId);

            await ReplaceOneAsync(filter, commentary, new ReplaceOptions { IsUpsert = true });

            return commentary;
        }

        public async Task SetPendingChangesToNullOnCommentary(string contentBlockId, Commentary commentary)
        {
            var filter = Builders<Commentary>.Filter.Where(
                x => x.ContentBlockId == contentBlockId && x.CommentaryId == commentary.CommentaryId);

            var pendingChangesUpdate = Builders<Commentary>.Update.Set(x => x.PendingChanges, null);
            await UpdateOneAsync(filter, pendingChangesUpdate);
        }
    }
}
