namespace Infrastructure.MongoDB
{
    using System.Diagnostics.CodeAnalysis;

    using global::HealthChecks.MongoDb;

    using global::MongoDB.Driver;

    using Infrastructure.Configuration.Model;
    using Infrastructure.HealthChecks;

    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Diagnostics.HealthChecks;

    [ExcludeFromCodeCoverage]
    public static class HealthChecks
    {
        public static IHealthChecksBuilder AddMongoDbCheck(this IHealthChecksBuilder builder, MongoDbOptions options)
        {
            builder.Add(
                new HealthCheckRegistration(
                    options.ConfigKey,
                    sp =>
                    {
                        var url = sp.GetRequiredService<MongoUrl>();
                        return new MongoDbHealthCheck(MongoClientSettings.FromUrl(url));
                    },
                    null,
                    [HealthCheckTag.Liveness, HealthCheckTag.Readiness],
                    options.HealthcheckTimeout));

            return builder;
        }
    }
}
