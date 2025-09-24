namespace Infrastructure.Extensions
{
    using System.Diagnostics.CodeAnalysis;

    using Infrastructure.Configuration;
    using Infrastructure.Logging;
    using Infrastructure.Middleware;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    using Prometheus;

    using Subscriber.Auth;

    [ExcludeFromCodeCoverage]
    public static class ApplicationBuilderExtensions
    {
        public static void ConfigureCommonMiddilewares(this IApplicationBuilder app, IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            app.UseSerilogEnrichers();

            app.UseElasticApm(configuration);

            app.UseCustomExceptionHandling();

            // Ensure that we pick up x-forwarded-proto etc
            app.UseForwardedHeaders();

            app.UseRouting();
            app.UseCors();
            app.UseJSNLog(loggerFactory, configuration);

            app.UseHttpMetrics();
            app.MapReadinessHealthCheck("/internal/health/readiness");
            app.MapLivenessHelathCheck("/internal/health/liveness");
            app.MapDatabaseHealthCheck("/internal/health/database");
            app.MapServiceHealthCheck("/internal/health/service");
            //app.UseSubscriberAuth("/v1/version", "/internal/*");

            app.UseEndpoints(
                endpoints =>
                {
                    endpoints.MapControllers();
                    endpoints.MapGraphQL("/v1/graphql");
                    endpoints.MapMetrics("internal/metrics");
                });
        }
    }
}
