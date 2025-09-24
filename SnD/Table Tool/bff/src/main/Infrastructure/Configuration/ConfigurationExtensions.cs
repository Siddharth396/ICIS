namespace Infrastructure.Configuration
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;




    using Microsoft.Extensions.Configuration;

    [ExcludeFromCodeCoverage]
    public static class ConfigurationExtensions
    {
        public static string[] GetCorsOrigins(this IConfiguration configuration)
        {
            var origins = configuration.GetValue("CorsOrigins", string.Empty);

            return origins.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();
        }

       

        public static T GetOptions<T>(this IConfiguration configuration)
            where T : class, IConfigurationOptions, new()
        {
            var options = new T();
            configuration.GetSection(options.ConfigKey).Bind(options);

            return options;
        }

        public static bool IsTraceEnabled(this IConfiguration configuration)
        {
            return configuration.GetValue<bool?>("TraceEnabled", false) ?? false;
        }

        public static bool IsDevMode(this IConfiguration configuration)
        {
            return configuration.GetValue<bool?>("DevMode", false) ?? false;
        }

        public static bool IsMutationEnabled(this IConfiguration configuration)
        {
            return configuration.GetValue<bool?>("MutationEnabled", false) ?? false;
        }
    }
}