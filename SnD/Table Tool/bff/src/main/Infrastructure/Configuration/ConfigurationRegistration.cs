namespace Infrastructure.Configuration
{
    using System.Diagnostics.CodeAnalysis;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    [ExcludeFromCodeCoverage]
    public static class ConfigurationRegistration
    {
        public static IServiceCollection RegisterConfigurationSections(this IServiceCollection services, IConfiguration configuration)
        {
            //// Add registrations for any IOptions configurations

            return services;
        }
    }
}