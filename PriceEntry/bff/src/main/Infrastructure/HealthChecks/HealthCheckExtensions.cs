namespace Infrastructure.HealthChecks
{
    using System.Diagnostics.CodeAnalysis;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Diagnostics.HealthChecks;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Diagnostics.HealthChecks;

    [ExcludeFromCodeCoverage]
    public static class HealthCheckExtensions
    {
        public static IHealthChecksBuilder AddDefaultHealthCheck(this IHealthChecksBuilder builder)
        {
            builder.AddCheck(
                "self",
                () => HealthCheckResult.Healthy(),
                [HealthCheckTag.Liveness, HealthCheckTag.Readiness]);

            return builder;
        }

        public static IApplicationBuilder MapLivenessHealthCheck(this IApplicationBuilder app, string url)
        {
            // Only health checks tagged with the "liveness" tag must pass for app to be considered alive
            var options = new HealthCheckOptions { Predicate = r => r.Tags.Contains(HealthCheckTag.Liveness) };

            app.UseHealthChecks(url, options);
            return app;
        }

        public static IApplicationBuilder MapReadinessHealthCheck(this IApplicationBuilder app, string url)
        {
            // Only health checks tagged with the "readiness" tag must pass for app to receive traffic
            var options = new HealthCheckOptions { Predicate = r => r.Tags.Contains(HealthCheckTag.Readiness) };

            app.UseHealthChecks(url, options);
            return app;
        }
    }
}
