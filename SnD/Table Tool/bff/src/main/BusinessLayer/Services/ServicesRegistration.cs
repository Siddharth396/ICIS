namespace BusinessLayer.Services
{
    using System.Diagnostics.CodeAnalysis;

    using BusinessLayer.Services.Capacity;
    using BusinessLayer.Services.Common;
    using BusinessLayer.Services.Outage;
    using BusinessLayer.Services.Product;
    using BusinessLayer.Services.Region;
    using BusinessLayer.Services.TableTool;
    using BusinessLayer.Validation;

    using Microsoft.Extensions.DependencyInjection;

    [ExcludeFromCodeCoverage]
    public static class ServicesRegistration
    {
        public static void RegisterServices(this IServiceCollection service)
        {
            service.AddScoped<IOutageService, OutageService>();
            service.AddScoped<ICapacityDevelopmentService, CapacityDevelopmentService>();
            service.AddScoped<IRegionService, RegionService>();
            service.AddScoped<IProductService, ProductService>();
            service.AddScoped<IContentBlockService, ContentBlockService>();
            service.AddScoped<IContentBlockValidationService, ContentBlockValidationService>();
            service.AddScoped<ISqlQueryReader, SqlQueryReader>();
        }
    }
}
