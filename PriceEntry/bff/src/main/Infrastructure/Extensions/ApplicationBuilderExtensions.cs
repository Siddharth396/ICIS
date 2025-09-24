namespace Infrastructure.Extensions
{
    using System.Diagnostics.CodeAnalysis;

    using Infrastructure.Configuration;
    using Infrastructure.HealthChecks;
    using Infrastructure.Logging;
    using Infrastructure.Middleware;
    using Infrastructure.MongoDB;
    using Infrastructure.PubNub;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    using Prometheus;

    using Serilog;

    [ExcludeFromCodeCoverage]
    public static class ApplicationBuilderExtensions
    {
        public static WebApplication ConfigureApplication(this WebApplication app)
        {
            app.ConfigureCommonMiddlewarePipeline(app.Configuration, app.Services.GetRequiredService<ILoggerFactory>());
            return app;
        }

        public static void RunApplication(this WebApplication app)
        {
            Log.Logger.Information("Starting in {Environment}", app.Environment.EnvironmentName);

            app.Run();
        }

        private static void ConfigureCommonMiddlewarePipeline(
            this IApplicationBuilder app,
            IConfiguration configuration,
            ILoggerFactory loggerFactory)
        {
            app.UseElasticApm(configuration);

            app.UseCustomExceptionHandling();

            // Ensure that we pick up x-forwarded-proto etc
            app.UseForwardedHeaders();

            app.UseRouting();
            app.UseCors();
            app.UseJSNLog(loggerFactory, configuration);

            app.UseHttpMetrics();
            app.MapReadinessHealthCheck("/internal/health/readiness");
            app.MapLivenessHealthCheck("/internal/health/liveness");

            // app.UseSubscriberAuth("/v1/version", "/internal/*");
            app.UseSerilogEnrichers();
            if (configuration.IsAuthoring())
            {
                app.UsePubNubNotificationMiddleware();
            }

            app.UseMongoDbTransactionMiddleware();
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
