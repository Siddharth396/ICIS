using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;
using SnD.EventProcessor.Poller.Constants;
using SnD.EventProcessor.Poller.Contracts;
using SnD.EventProcessor.Poller.Services;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;

namespace SnD.EventProcessor.Poller
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostContext, config) =>
            {
                var applicationVersion = $"{Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyFileVersionAttribute>().Version}";
                Console.WriteLine($"Application Version - {applicationVersion}");
                var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                Console.WriteLine($"Environment: {environment}");
                config.SetBasePath(Environment.CurrentDirectory)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile($"./secrets/appsettings.secrets.json", optional: true, reloadOnChange: true)
                    .AddJsonFile($"./conf/appsettings.{environment}.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables();
            })
            .ConfigureServices((hostContext, services) =>
                {
                    AddPollyRetryConfig(services);
                    services.AddHostedService<EventProcessorBackgroundService>();
                    services.AddTransient<IPollerConfigHelper, PollerConfigHelper>();
                    services.AddTransient<ISnDEventProcessorClient, SnDEventProcessorClient>();
                    services.AddTransient<ISQSClientService, SQSClientService>();
                    services.AddTransient<ISQSMessageProcessor, SQSMessageProcessor>();
                    services.AddTransient<IPollerConfigHelper, PollerConfigHelper>();
                    services.AddTransient<IMessagePoller, SQSMessagePoller>();
                    services.AddTransient<IAuthorizationService, AuthorizationService>();
                });

        private static void AddPollyRetryConfig(IServiceCollection services)
        {
            //services.AddHttpClient("HttpClientWithPolyRetry")
            //.AddTransientHttpErrorPolicy(policyBuilder 
            //    => policyBuilder.WaitAndRetryAsync(
            //        3, 
            //        retryNumber => TimeSpan.FromSeconds(Math.Pow(2, retryNumber)),
            //        (exception, timeSpan, retryCount, contxt)
            //              => { Console.WriteLine($"retry({retryCount}) after {timeSpan} seconds."); })
            //    );

            var policy = Policy.Handle<Exception>()
                .OrResult<HttpResponseMessage>(x =>
                {
                    x.Headers.TryGetValues("ServiceName", out var serviceNames);
                    var serviceName = serviceNames?.FirstOrDefault() ?? string.Empty;
                    var isKp = serviceName.Equals("KafkaProducer", StringComparison.OrdinalIgnoreCase);
                    return isKp ? RetryableHttpStatusCodes.KPCodes.Contains(x.StatusCode) : RetryableHttpStatusCodes.Codes.Contains(x.StatusCode);
                })
                .WaitAndRetryAsync(
                    3,
                    retryNumber => TimeSpan.FromSeconds(Math.Pow(2, retryNumber)),
                    (exception, timeSpan, retryCount, context)
                        =>
                    { Console.WriteLine($"retry({retryCount}) after {timeSpan} seconds."); });

            services.AddHttpClient("HttpClientWithPolyRetry").AddPolicyHandler(policy);
        }
    }
}
