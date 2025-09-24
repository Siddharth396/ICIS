namespace Test.Infrastructure.Authoring
{
    using System.IO;
    using System.Text;

    using dotenv.net;

    using global::Authoring.Infrastructure;

    using global::Infrastructure.Services.CanvasApi;
    using global::Infrastructure.Services.PeriodGenerator;
    using global::Infrastructure.Services.Workflow;

    using global::Subscriber.Auth;

    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc.Testing;
    using Microsoft.AspNetCore.TestHost;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Internal;
    using Microsoft.Extensions.Logging;

    using MongoDB.Driver;

    using Test.Infrastructure.Mongo;
    using Test.Infrastructure.Mongo.Repositories;
    using Test.Infrastructure.Services;
    using Test.Infrastructure.Stubs;

    public class AuthoringBffApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            LoadConfigurationFromEnvFiles(builder);

            // Validate DI setup at test startup.
            builder.UseDefaultServiceProvider(
                options =>
                {
                    options.ValidateScopes = true;
                    options.ValidateOnBuild = true;
                });

            builder.ConfigureTestServices(
                services =>
                {
                    services.AddSingleton<IUserContext, TestUserContext>();
                    services
                       .AddSingleton(provider => (TestUserContext)provider.GetService<IUserContext>()!)
                       .AddSingleton<ISystemClock>(new TestClock())
                       .AddSingleton(p => (TestClock)p.GetService<ISystemClock>()!);

                    services
                       .AddSingleton<PeriodGeneratorServiceStub>()
                       .AddSingleton<IPeriodGeneratorService>(p => p.GetRequiredService<PeriodGeneratorServiceStub>());

                    services
                       .AddSingleton<CanvasApiServiceStub>()
                       .AddSingleton<ICanvasApiService>(p => p.GetRequiredService<CanvasApiServiceStub>());
                    services
                       .AddSingleton<DataPackageWorkflowServiceStub>()
                       .AddSingleton<IDataPackageWorkflowService>(
                            p => p.GetRequiredService<DataPackageWorkflowServiceStub>());

                    services
                       .AddSingleton<IMongoClient>(new MongoClient(MongoServer.Runner.ConnectionString))
                       .AddSingleton(
                            p =>
                            {
                                var client = p.GetService<IMongoClient>()!;
                                return client.GetDatabase("test_database");
                            })
                       .AddSingleton<Database>()
                       .AddSingleton<GasPriceSeriesItemTestService>()
                       .AddSingleton(new MongoUrl(MongoServer.Runner.ConnectionString));

                    services.AddLogging(loggingBuilder => loggingBuilder.ClearProviders());

                    services.AddSingleton<PriceSeriesRepository>().AddSingleton<PriceSeriesTestService>();
                    services.AddSingleton<GenericRepository>();

                    PopulateReferenceDataCollections(services);
                });
        }

        private static void LoadConfigurationFromEnvFiles(IWebHostBuilder builder)
        {
            var envPaths = new[]
            {
                Path.Combine(Directory.GetCurrentDirectory(), "authoring.env"),
                Path.Combine(Directory.GetCurrentDirectory(), "authoring.test.env")
            };
            var read = DotEnv.Read(new DotEnvOptions(false, envPaths, Encoding.UTF8));
            foreach (var (key, value) in read)
            {
                builder.UseSetting(key, value);
            }
        }

        private void PopulateReferenceDataCollections(IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var database = scope.ServiceProvider.GetService<Database>()!;

            database.PopulateReferenceDataCollections();
        }
    }
}
