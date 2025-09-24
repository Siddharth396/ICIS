namespace Infrastructure.Services.PeriodGenerator
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using Infrastructure.HealthChecks;

    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Diagnostics.HealthChecks;

    [ExcludeFromCodeCoverage]
    public static class HealthChecks
    {
        public static IHealthChecksBuilder AddPeriodGeneratorServiceCheck(this IHealthChecksBuilder builder)
        {
            builder.AddUrlGroup(
                provider =>
                {
                    var versionEndpoint = provider.GetRequiredService<PeriodGeneratorSettings>()
                       .GetVersionEndpointFullUrl();
                    return new Uri(versionEndpoint);
                },
                PeriodGeneratorSettings.ConfigurationSectionName,
                HealthStatus.Degraded,
                [HealthCheckTag.Readiness]);

            return builder;
        }
    }
}
