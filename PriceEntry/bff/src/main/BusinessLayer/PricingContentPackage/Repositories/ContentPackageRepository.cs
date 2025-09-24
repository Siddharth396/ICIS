namespace BusinessLayer.PricingContentPackage.Repositories
{
    using System.Threading.Tasks;

    using BusinessLayer.DataPackage.Models;
    using BusinessLayer.PricingContentPackage.Repositories.Models;

    using Infrastructure.MongoDB.Repositories;
    using Infrastructure.MongoDB.Transactions;

    using MongoDB.Driver;
    using MongoDB.Driver.Linq;

    public class ContentPackageRepository : BaseRepository<ContentPackage>
    {
        public const string CollectionName = "pricing_content_packages";

        public ContentPackageRepository(IMongoDatabase database, IMongoContext mongoContext)
            : base(database, mongoContext)
        {
        }

        public override string DbCollectionName => CollectionName;

        public async Task<ContentPackage?> GetContentPackage(DataPackageId dataPackageId)
        {
            return await Query().Where(x => x.ContentPackageId == dataPackageId.Value).SingleOrDefaultAsync();
        }

        public Task Save(ContentPackage contentPackage)
        {
            var filter =
                Builders<ContentPackage>.Filter.Where(x => x.ContentPackageId == contentPackage.ContentPackageId);
            return ReplaceOneAsync(filter, contentPackage, new ReplaceOptions { IsUpsert = true });
        }
    }
}
