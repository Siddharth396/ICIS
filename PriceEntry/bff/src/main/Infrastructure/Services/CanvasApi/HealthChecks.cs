namespace Infrastructure.Services.CanvasApi
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using Infrastructure.HealthChecks;

    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Diagnostics.HealthChecks;

    [ExcludeFromCodeCoverage]
    public static class HealthChecks
    {
        public static IHealthChecksBuilder AddCanvasApiServiceCheck(this IHealthChecksBuilder builder)
        {
            builder.AddUrlGroup(
                provider =>
                {
                    var versionEndpoint = provider.GetRequiredService<CanvasApiSettings>().GetVersionEndpointFullUrl();
                    return new Uri(versionEndpoint);
                },
                CanvasApiSettings.ConfigurationSectionName,
                HealthStatus.Degraded,
                [HealthCheckTag.Readiness]);

            return builder;
        }
    }
}
