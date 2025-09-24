namespace Infrastructure.Extensions
{
    using System.Diagnostics.CodeAnalysis;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Diagnostics.HealthChecks;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Diagnostics.HealthChecks;

    using Prometheus;


    [ExcludeFromCodeCoverage]
    public static class HealthCheckExtensions
    {
        public static void AddHealthCheckExtension(this IServiceCollection services, string connString)
        {
            services.AddHealthChecks()
                .AddCheck("SnDBff", () => HealthCheckResult.Healthy("Service is up and running"), tags: new[] { "Service", "All" })
                .AddSqlServer(connString, "Select 1", tags: new[] { "DB", "All" })
                .ForwardToPrometheus();
        }
        public static IApplicationBuilder MapLivenessHelathCheck(this IApplicationBuilder app, string url)
        {
            // There is no health check registered for now that could affect the liveness
            var options = new HealthCheckOptions { Predicate = _ => true };

            app.UseHealthChecks(url, options);
            return app;
        }

        public static IApplicationBuilder MapReadinessHealthCheck(this IApplicationBuilder app, string url)
        {
            // Check all registered health checks
            var options = new HealthCheckOptions { Predicate = _ => true };

            app.UseHealthChecks(url, options);
            return app;
        }
        public static IApplicationBuilder MapDatabaseHealthCheck(this IApplicationBuilder app, string url)
        {
            // Check all registered health checks
            var options = new HealthCheckOptions { Predicate = _ => true };

            app.UseHealthChecks(url, options);
            return app;
        }
        public static IApplicationBuilder MapServiceHealthCheck(this IApplicationBuilder app, string url)
        {
            // Check all registered health checks
            var options = new HealthCheckOptions { Predicate = _ => true };

            app.UseHealthChecks(url, options);
            return app;
        }


        //public static IHealthChecksBuilder AddMongoDbCheck(
        //    this IHealthChecksBuilder services,
        //    IConfiguration config,
        //    MongoDbOptions options)
        //{
        //    var mongoUrl = config.GetMongoDbUrl();

        //    //services.AddMongoDb(
        //    //    MongoClientSettings.FromUrl(mongoUrl),
        //    //    name: options.ConfigKey,
        //    //    timeout: options.HealthcheckTimeout);

        //    return services;
        //}

    }
}
