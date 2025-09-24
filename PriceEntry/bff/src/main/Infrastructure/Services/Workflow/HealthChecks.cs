namespace Infrastructure.Services.Workflow
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using Infrastructure.HealthChecks;

    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Diagnostics.HealthChecks;

    [ExcludeFromCodeCoverage]
    public static class HealthChecks
    {
        public static IHealthChecksBuilder AddWorkflowSystemCheck(this IHealthChecksBuilder builder)
        {
            builder.AddUrlGroup(
                provider =>
                {
                    var versionEndpoint = provider.GetRequiredService<WorkflowSettings>().GetVersionEndpointFullUrl();
                    return new Uri(versionEndpoint);
                },
                WorkflowSettings.ConfigurationSectionName,
                HealthStatus.Degraded,
                [HealthCheckTag.Readiness]);

            return builder;
        }
    }
}