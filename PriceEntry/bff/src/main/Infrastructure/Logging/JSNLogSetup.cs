namespace Infrastructure.Logging
{
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Text.RegularExpressions;

    using Infrastructure.Configuration;

    using JSNLog;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    [ExcludeFromCodeCoverage]
    public static class JSNLogSetup
    {
        public static IApplicationBuilder UseJSNLog(
            this IApplicationBuilder builder,
            ILoggerFactory loggerFactory,
            IConfiguration configuration)
        {
            var origins = configuration.GetCorsOrigins();
            var jsnlogConfiguration = new JsnlogConfiguration
            {
                corsAllowedOriginsRegex = string.Join('|', origins.Select(Regex.Escape))
            };
            builder.UseJSNLog(new JSNLogToSerilogAdapter(loggerFactory), jsnlogConfiguration);

            return builder;
        }
    }
}