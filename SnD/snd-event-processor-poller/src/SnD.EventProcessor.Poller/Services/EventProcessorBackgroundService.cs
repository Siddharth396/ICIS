using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SnD.EventProcessor.Poller.Contracts;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SnD.EventProcessor.Poller.Services
{
    public class EventProcessorBackgroundService : BackgroundService, IHostedService
    {
        private readonly ILogger<EventProcessorBackgroundService> _logger;
        private readonly IPollerConfigHelper _configHelper;
        private readonly IMessagePoller _messagePoller;

        public EventProcessorBackgroundService(
            ILogger<EventProcessorBackgroundService> logger,
            IPollerConfigHelper configHelper,
            IMessagePoller messagePoller
            )
        {
            _logger = logger;
            _configHelper = configHelper;
            _messagePoller = messagePoller;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Run(async () =>
            {
                try
                {
                    var pollerConfig = _configHelper.GetPollerConfig();
                    var initialDelayInSeconds = new Random().Next(0, pollerConfig.MaxInitialDelayInSeconds);
                    //await Task.Delay(initialDelayInSeconds * 1000, stoppingToken);

                    _logger.LogInformation($"Poller background service has started. Start Time: { DateTime.UtcNow } (UTC)");
                    while (!stoppingToken.IsCancellationRequested)
                    {
                        _logger.LogInformation($"Poller running at: {DateTime.UtcNow} (UTC)");
                        await _messagePoller.PollQueuesAsync(stoppingToken);
                        await Task.Delay(pollerConfig.IntervalInSeconds * 1000, stoppingToken);
                    }
                    _logger.LogInformation($"Poller background service has completed polling. Time: { DateTime.UtcNow } (UTC)");
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, $"Error while executing Poller background service. Time { DateTime.UtcNow } (UTC)");
                    throw;
                }
            });
        }
    }
}
