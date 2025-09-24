namespace Test.Infrastructure.Subscriber
{
    using System.IO;
    using System.Text;

    using dotenv.net;

    using global::Subscriber.Auth;
    using global::Subscriber.Infrastructure;

    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc.Testing;
    using Microsoft.AspNetCore.TestHost;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Internal;
    using Microsoft.Extensions.Logging;

    using MongoDB.Driver;

    using Test.Infrastructure.Mongo;
    using Test.Infrastructure.Stubs;

    public class SubscriberBffApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            LoadConfigurationFromEnvFiles(builder);

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
                    services.AddSingleton(provider => (TestUserContext)provider.GetService<IUserContext>()!)
                       .AddSingleton<ISystemClock>(new TestClock())
                       .AddSingleton(p => (TestClock)p.GetService<ISystemClock>()!);

                    services.AddSingleton<IMongoClient>(new MongoClient(MongoServer.Runner.ConnectionString))
                       .AddSingleton(
                            p =>
                            {
                                var client = p.GetService<IMongoClient>()!;
                                return client.GetDatabase("test_database");
                            })
                       .AddSingleton<Database>()
                       .AddSingleton(new MongoUrl(MongoServer.Runner.ConnectionString));

                    services.AddLogging(loggingBuilder => loggingBuilder.ClearProviders());
                });
        }

        private static void LoadConfigurationFromEnvFiles(IWebHostBuilder builder)
        {
            var envPaths = new[]
            {
                Path.Combine(Directory.GetCurrentDirectory(), "subscriber.env"),
                Path.Combine(Directory.GetCurrentDirectory(), "subscriber.test.env")
            };
            var read = DotEnv.Read(new DotEnvOptions(false, envPaths, Encoding.UTF8));
            foreach (var (key, value) in read)
            {
                builder.UseSetting(key, value);
            }
        }
    }
}
