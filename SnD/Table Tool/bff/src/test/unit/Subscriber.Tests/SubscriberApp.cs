namespace Subscriber.Tests
{
    using System;

    using dotenv.net;

    using global::Subscriber.Auth;
    using global::Subscriber.Infrastructure;

    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Internal;

    using Test.Infrastructure.Stubs;

    public class SubscriberApp : IDisposable
    {
        private readonly IServiceScope serviceScope;

        private bool disposed;

        public SubscriberApp(IHost host)
        {
            serviceScope = host.Services.CreateScope();
            ServiceProvider = serviceScope.ServiceProvider;

        }

        public IServiceProvider ServiceProvider { get; }


        public static SubscriberApp Build()
        {
            var program = new Program("subscriber.appsettings.json", "subscriber.env", AddCustomConfiguration);
            var host = program.CreateHostBuilder(new string[] { })
               .ConfigureWebHostDefaults(
                    builder =>
                    {
                        builder.UseConfiguration(program.GetConfiguration())
                           .UseStartup(ctx => new Startup(ctx.Configuration, RegisterOverrides));
                    })
               .Build();

            return new SubscriberApp(host);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public T GetService<T>()
        {
            return ServiceProvider.GetService<T>()!;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                TearDown();
            }

            disposed = true;
        }

        private static void RegisterOverrides(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IUserContext, TestUserContext>();
            serviceCollection.AddSingleton(provider => (TestUserContext)provider.GetService<IUserContext>()!)
               .AddSingleton<ISystemClock>(new TestClock())
               .AddSingleton(p => ((TestClock)p.GetService<ISystemClock>()!));
        }

        private static void AddCustomConfiguration(IConfigurationBuilder builder)
        {
            DotEnv.Config(true, "subscriber.test.env");
        }

        private void TearDown()
        {
            serviceScope?.Dispose();
        }
    }
}
