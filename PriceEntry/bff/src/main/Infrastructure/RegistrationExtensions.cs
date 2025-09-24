namespace Infrastructure
{
    using Infrastructure.PubNub;
    using Infrastructure.Services.AuditInfoService;
    using Infrastructure.Services.CanvasApi;
    using Infrastructure.Services.OAuth;
    using Infrastructure.Services.PeriodGenerator;
    using Infrastructure.Services.Workflow;

    using Microsoft.Extensions.DependencyInjection;

    public static class RegistrationExtensions
    {
        public static IServiceCollection RegisterInfrastructure(this IServiceCollection services)
        {
            services
               .RegisterPeriodGeneratorServices()
               .RegisterWorkflowServices()
               .RegisterAuditInfoService()
               .RegisterOAuthServices()
               .RegisterPubNub()
               .RegisterCanvasApiServices();

            return services;
        }

        public static IServiceCollection RegisterInfrastructureForSubscriber(this IServiceCollection services)
        {
            services
                .RegisterAuditInfoService()
                .RegisterOAuthServices();

            return services;
        }
    }
}
