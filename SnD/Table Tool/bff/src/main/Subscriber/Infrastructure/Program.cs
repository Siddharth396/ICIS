namespace Subscriber.Infrastructure
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Net;
    using System.Runtime.Loader;
    using System.Text;
    using System.Threading;

    using dotenv.net;
    using dotenv.net.DependencyInjection.Infrastructure;

    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;

    using Serilog;

    public class Program
    {
        [ExcludeFromCodeCoverage]
        public Program()
            : this("appsettings.json", ".env")
        {
        }

        public Program(
            string appSettingsPath,
            string envFilename,
            Action<IConfigurationBuilder>? addCustomConfiguration = null)
        {
            AppSettingsPath = appSettingsPath;
            EnvFilename = envFilename;
            AddCustomConfiguration = addCustomConfiguration;
        }

        // Hack used to allow unit tests to add overrides to the existing configuration (e.g. override appsettings.json configs)
        public Action<IConfigurationBuilder>? AddCustomConfiguration { private get; set; }

        public string AppSettingsPath { get; set; }

        public string EnvFilename { get; set; }

        [ExcludeFromCodeCoverage]
        public static void Main(string[] args)
        {
            AssemblyLoadContext.Default.Unloading += SigTermEventHandler;
            Console.CancelKeyPress += CancelHandler;

            var program = new Program();
            var configuration = program.GetConfiguration();

            program.CreateHostBuilder(args)
               .ConfigureWebHostDefaults(
                    builder =>
                    {
                        builder.UseConfiguration(configuration)
                           .UseUrls(configuration.GetValue<string>("Hostings:Urls"))
                           .UseStartup(ctx => new Startup(ctx.Configuration));
                    })
               .Build()
               .Run();
        }

        public void BuildConfiguration(IConfigurationBuilder builder)
        {
            builder.SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile(
                AppSettingsPath,
                false,
                false);

            builder.AddEnvironmentVariables();

            AddCustomConfiguration?.Invoke(builder);
        }

        [ExcludeFromCodeCoverage]
        public IHostBuilder CreateHostBuilder(string[] args)
        {
            var configuration = GetConfiguration();

            ServicePointManager.DefaultConnectionLimit = configuration.GetValue<int>("Hostings:ConnectionLimit");
            ThreadPool.GetMaxThreads(out _, out var completionThreads);
            ThreadPool.SetMinThreads(configuration.GetValue<int>("Hostings:MinThreadCount"), completionThreads);

            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(configuration).CreateLogger();

            var hostBuilder = Host.CreateDefaultBuilder(args)
               .UseSerilog()
               .ConfigureAppConfiguration(
                    (hostContext, config) =>
                    {
                        var env = hostContext.HostingEnvironment;
                        Console.WriteLine($"Starting in '{env.EnvironmentName}' environment", env.EnvironmentName);

                        BuildConfiguration(config);
                    });

            return hostBuilder;
        }

        public IConfiguration GetConfiguration()
        {
            DotEnv.Config(new DotEnvOptions { Encoding = Encoding.UTF8, EnvFile = EnvFilename, ThrowOnError = false });

            var builder = new ConfigurationBuilder();
            BuildConfiguration(builder);

            return builder.BuildAndReplacePlaceholders();
        }

        [ExcludeFromCodeCoverage]
        private static void CancelHandler(object? sender, ConsoleCancelEventArgs e)
        {
            Console.WriteLine("Exiting BFF...");
        }

        [ExcludeFromCodeCoverage]
        private static void SigTermEventHandler(AssemblyLoadContext obj)
        {
            Console.WriteLine("Unloading BFF...");
        }
    }
}
