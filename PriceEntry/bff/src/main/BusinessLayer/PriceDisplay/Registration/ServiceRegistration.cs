namespace BusinessLayer.PriceDisplay.Registration
{
    using BusinessLayer.PriceDisplay.ContentBlock.Services;
    using BusinessLayer.PriceDisplay.GridConfiguration.Services;
    using BusinessLayer.PriceDisplay.PriceSeries.Services;
    using BusinessLayer.PriceDisplay.PriceSeriesSelection.Services;

    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceRegistration
    {
        public static void RegisterServicesForDisplay(this IServiceCollection service)
        {
            service.AddScoped<IPriceSeriesService, PriceSeriesService>();
            service.AddScoped<IPriceSeriesDisplayService, PriceSeriesDisplayService>();
            service.AddScoped<IContentBlockServiceForDisplay, ContentBlockServiceForDisplay>();
            service.AddScoped<IGridConfigurationService, GridConfigurationService>();
            service.AddScoped<IFilterService, FilterService>();
        }
    }
}
