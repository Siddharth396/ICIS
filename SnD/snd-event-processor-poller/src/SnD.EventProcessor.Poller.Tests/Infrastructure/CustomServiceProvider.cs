using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using SnD.EventProcessor.Poller.Contracts;
using SnD.EventProcessor.Poller.Services;
using System;

namespace SnD.EventProcessor.Poller.Tests.Infrastructure
{
    public class CustomServiceProvider
    {
        public IServiceProvider GetServiceProvider()    
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.test.json", true, true)
                .Build();
            IServiceCollection services = new ServiceCollection();
            services.AddSingleton(configuration);
            services.AddSingleton(Mock.Of<IPollerConfigHelper>());
            services.AddSingleton(Mock.Of<IMessagePoller>());
            services.AddSingleton(Mock.Of<ILogger<EventProcessorBackgroundService>>());
            services.AddHostedService<EventProcessorBackgroundService>();
            var serviceProvider = services.BuildServiceProvider();

            return serviceProvider;
        }
    }
}
