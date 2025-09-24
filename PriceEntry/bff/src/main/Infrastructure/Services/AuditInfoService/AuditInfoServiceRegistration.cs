namespace Infrastructure.Services.AuditInfoService
{
    using Microsoft.Extensions.DependencyInjection;

    public static class AuditInfoServiceRegistration
    {
        public static IServiceCollection RegisterAuditInfoService(this IServiceCollection services)
        {
            services.AddScoped<IAuditInfoService, AuditInfoService>();

            return services;
        }
    }
}
