namespace BusinessLayer.Registration
{
    using BusinessLayer.Commentary.Repositories;
    using BusinessLayer.ContentBlock.Repositories;
    using BusinessLayer.ContentPackageGroup.Repositories;
    using BusinessLayer.DataPackage.Repositories;
    using BusinessLayer.PriceEntry.Repositories;
    using BusinessLayer.PriceSeriesSelection.Repositories;
    using BusinessLayer.PricingContentPackage.Repositories;
    using BusinessLayer.UserPreference.Repositories;

    using Microsoft.Extensions.DependencyInjection;

    public static class RepositoriesRegistration
    {
        public static void RegisterRepositories(this IServiceCollection service)
        {
            service.AddScoped<PriceEntryRepository>();
            service.AddScoped<GridConfigurationRepository>();
            service.AddScoped<IPriceSeriesItemRepository, PriceSeriesItemRepository>();
            service.AddScoped<ContentBlockRepository>();
            service.AddScoped<PriceSeriesRepository>();
            service.AddScoped<CommentaryRepository>();
            service.AddScoped<UserPreferenceRepository>();
            service.AddScoped<PriceDeltaTypeRepository>();
            service.AddScoped<ReferenceMarketRepository>();
            service.AddScoped<GasPriceSeriesItemRepository>();
            service.AddScoped<IDataPackageRepository, DataPackageRepository>();
            service.AddScoped<ContentPackageRepository>();
            service.AddScoped<DataPackageMetadataRepository>();
            service.AddScoped<ContentPackageGroupRepository>();
        }
    }
}