namespace BusinessLayer.UserPreference.Repositories
{
    using System.Threading.Tasks;

    using BusinessLayer.UserPreference.Repositories.Models;

    using Infrastructure.MongoDB.Repositories;
    using Infrastructure.MongoDB.Transactions;

    using MongoDB.Driver;
    using MongoDB.Driver.Linq;

    public class UserPreferenceRepository : BaseRepository<UserPreference>
    {
        public const string CollectionName = "user_preferences";

        public UserPreferenceRepository(IMongoDatabase database, IMongoContext mongoContext)
            : base(database, mongoContext)
        {
        }

        public override string DbCollectionName => CollectionName;

        public async Task<UserPreference?> GetUserPreference(string userId, string contentBlockId)
        {
            return await Query()
                .Where(x => x.UserId == userId && x.ContentBlockId == contentBlockId)
                .FirstOrDefaultAsync();
        }

        public async Task SaveUserPreference(UserPreference userPreference)
        {
            var filter = Builders<UserPreference>.Filter.Where(x => x.UserId == userPreference.UserId && x.ContentBlockId == userPreference.ContentBlockId);
            await ReplaceOneAsync(filter, userPreference, new ReplaceOptions { IsUpsert = true });
        }
    }
}
