using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SnD.EventProcessor.Poller.Tests.Infrastructure;
using Xunit;
using Moq;
using SnD.EventProcessor.Poller.Contracts;
using SnD.EventProcessor.Poller.Services;

namespace SnD.EventProcessor.Poller.Tests.UnitTests
{
    public class EventProcessorBackgroundServiceTests: IClassFixture<CustomServiceProvider>
    {
        private readonly IServiceProvider _serviceProvider;
        private const int _fiveSecondsDelay = 5;

        public EventProcessorBackgroundServiceTests(CustomServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider.GetServiceProvider();
        }

        [Fact]
        public async Task ExecuteAsync_should_call_get_poller_config_once()
        {
            // Arrange
            var pollerConfig = Mock.Get(_serviceProvider.GetService<IPollerConfigHelper>());
            pollerConfig.Setup(x => x.GetPollerConfig()).Returns(TestPollerConfig.PollerConfigWithFiveSecondsInterval);
            var pollerService = _serviceProvider.GetService<IHostedService>() as EventProcessorBackgroundService;

            // Act
            await pollerService?.StartAsync(CancellationToken.None);
            await Task.Delay(_fiveSecondsDelay);
            await pollerService?.StopAsync(CancellationToken.None);

            // Assert
            pollerConfig.Verify(c => c.GetPollerConfig(), Times.AtLeastOnce);
        }
    }
}
