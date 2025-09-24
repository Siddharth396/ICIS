namespace Test.Infrastructure.Mongo
{
    using System.Threading.Tasks;

    using BusinessLayer.Commentary.Repositories;
    using BusinessLayer.ContentBlock.Repositories;
    using BusinessLayer.ContentPackageGroup.Repositories;
    using BusinessLayer.DataPackage.Repositories;
    using BusinessLayer.PriceDisplay.ContentBlock.Repositories;
    using BusinessLayer.PriceEntry.Repositories;
    using BusinessLayer.PricingContentPackage.Repositories;
    using BusinessLayer.UserPreference.Repositories;

    using MongoDB.Driver;

    using MongoDbSeeder.Repositories;

    using GasPriceSeriesItemRepository = MongoDbSeeder.Repositories.GasPriceSeriesItemRepository;
    using GridConfigurationRepository = MongoDbSeeder.Repositories.GridConfigurationRepository;
    using PriceDeltaTypeRepository = MongoDbSeeder.Repositories.PriceDeltaTypeRepository;
    using PriceDisplayGridConfigurationRepository = MongoDbSeeder.Repositories.PriceDisplay.GridConfigurationRepository;
    using ReferenceMarketRepository = MongoDbSeeder.Repositories.ReferenceMarketRepository;

    public class Database
    {
        private readonly IMongoDatabase mongoDatabase;

        public Database(IMongoDatabase mongoDatabase)
        {
            this.mongoDatabase = mongoDatabase;
        }

        public async Task ResetNonReferenceDataCollections()
        {
            await mongoDatabase.DropCollectionAsync(PriceSeriesItemRepository.CollectionName);
            await mongoDatabase.DropCollectionAsync(ContentBlockRepository.CollectionName);
            await mongoDatabase.DropCollectionAsync(UserPreferenceRepository.CollectionName);
            await mongoDatabase.DropCollectionAsync(DataPackageRepository.CollectionName);
            await mongoDatabase.DropCollectionAsync(CommentaryRepository.CollectionName);
            await mongoDatabase.DropCollectionAsync(ContentPackageRepository.CollectionName);
            await mongoDatabase.DropCollectionAsync(ContentBlockRepositoryForDisplay.CollectionName);
            await mongoDatabase.DropCollectionAsync(DataPackageMetadataRepository.CollectionName);
            await mongoDatabase.DropCollectionAsync(ContentPackageGroupRepository.CollectionName);
        }

        public void PopulateReferenceDataCollections()
        {
            new GridConfigurationRepository(mongoDatabase).SeedCollection();
            new PriceSeriesRepository(mongoDatabase).SeedCollection();
            new GasPriceSeriesItemRepository(mongoDatabase).SeedCollection();
            new ReferenceMarketRepository(mongoDatabase).SeedCollection();
            new PriceDeltaTypeRepository(mongoDatabase).SeedCollection();
            new PriceDisplayGridConfigurationRepository(mongoDatabase).SeedCollection();
        }
    }
}
