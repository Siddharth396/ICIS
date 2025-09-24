namespace BusinessLayer.PriceDisplay.Registration
{
    using BusinessLayer.PriceDisplay.ContentBlock.Repositories;
    using BusinessLayer.PriceDisplay.GridConfiguration.Repositories;
    using BusinessLayer.PriceDisplay.PriceSeries.Repositories;
    using BusinessLayer.PriceDisplay.PriceSeriesSelection.Repositories;

    using Microsoft.Extensions.DependencyInjection;

    public static class RepositoriesRegistration
    {
        public static void RegisterRepositoriesForDisplay(this IServiceCollection service)
        {
            service.AddScoped<PriceSeriesRepository>();
            service.AddScoped<ContentBlockRepositoryForDisplay>();
            service.AddScoped<GridConfigurationRepository>();
            service.AddScoped<PriceSeriesForDisplayRepository>();
        }
    }
}
