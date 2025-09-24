namespace Infrastructure.Services.OAuth
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using Infrastructure.HealthChecks;

    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Diagnostics.HealthChecks;

    [ExcludeFromCodeCoverage]
    public static class HealthChecks
    {
        public static IHealthChecksBuilder AddOAuthSystemCheck(this IHealthChecksBuilder builder)
        {
            builder.AddUrlGroup(
                provider =>
                {
                    var versionEndpoint = provider.GetRequiredService<OAuthOptions>().GetVersionEndpointFullUrl();
                    return new Uri(versionEndpoint);
                },
                OAuthOptions.ConfigurationSectionName,
                HealthStatus.Degraded,
                [HealthCheckTag.Readiness]);

            return builder;
        }
    }
}
