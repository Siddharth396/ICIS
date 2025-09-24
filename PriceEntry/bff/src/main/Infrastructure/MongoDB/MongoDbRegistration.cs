namespace Infrastructure.MongoDB
{
    using System.Diagnostics.CodeAnalysis;

    using Elastic.Apm.MongoDb;

    using global::MongoDB.Driver;

    using Infrastructure.Configuration;
    using Infrastructure.MongoDB.Transactions;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    [ExcludeFromCodeCoverage]
    public static class MongoDbRegistration
    {
        public static IServiceCollection AddMongoDb(this IServiceCollection services)
        {
            services.AddSingleton<MongoUrl>(
                provider =>
                {
                    var config = provider.GetRequiredService<IConfiguration>();
                    return config.GetMongoDbUrl();
                });

            services.AddSingleton<IMongoClient>(
                provider =>
                {
                    var url = provider.GetRequiredService<MongoUrl>();
                    var settings = MongoClientSettings.FromUrl(url);

                    settings.ClusterConfigurator = builder => builder.Subscribe(new MongoDbEventSubscriber());

                    var client = new MongoClient(settings);

                    return client;
                });

            services.AddSingleton<IMongoDatabase>(
                provider =>
                {
                    var client = provider.GetRequiredService<IMongoClient>();
                    var url = provider.GetRequiredService<MongoUrl>();

                    return client.GetDatabase(
                        url.DatabaseName,
                        new MongoDatabaseSettings { ReadPreference = ReadPreference.PrimaryPreferred });
                });

            services.AddScoped<MongoContext>().AddScoped<IMongoContext>(p => p.GetRequiredService<MongoContext>());

            return services;
        }

        public static IApplicationBuilder UseMongoDbTransactionMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<MongoDbTransactionMiddleware>();

            return app;
        }
    }
}
