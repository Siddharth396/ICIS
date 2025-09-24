namespace Infrastructure.PubNub
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;

    using Infrastructure.PubNub.Models;

    using Microsoft.Extensions.Options;

    using Polly;

    using PubnubApi;

    using Serilog;

    [ExcludeFromCodeCoverage]
    public class PubNubNotificationService : IPubNubNotificationService
    {
        private readonly ILogger logger = Log.ForContext<PubNubNotificationService>();

        private readonly Pubnub pubNub;

        private readonly PubNubOptions options;

        private readonly List<PubNubData<PriceItemEvent>> notifications = new();

        public PubNubNotificationService(Pubnub pubNub, IOptions<PubNubOptions> options)
        {
            this.pubNub = pubNub;
            this.options = options.Value;
        }

        public void AddPubNubNotification(PubNubData<PriceItemEvent> data)
        {
            notifications.Add(data);
        }

        public async Task SendPubNubNotifications()
        {
            if (notifications.Count == 0)
            {
                return;
            }

            if (!options.Enabled)
            {
                logger.Warning("PubNub is disabled. Users won't receive notifications until this feature is enabled.");
                return;
            }

            if (string.IsNullOrWhiteSpace(pubNub.PNConfig.PublishKey) || (string.IsNullOrWhiteSpace(pubNub.PNConfig.SubscribeKey)))
            {
                logger.Error("PubNub is not configured properly. Please check the configuration.");
                return;
            }

            foreach (var notification in notifications)
            {
                await SendPubNubNotification(notification);
            }
        }

        private async Task SendPubNubNotification(PubNubData<PriceItemEvent> data)
        {
            logger.Information("Pubnub is enabled sending request");

            var retryPolicy = Policy.HandleResult<PNResult<PNPublishResult>>(r => r.Status == null || r.Status.Error)
               .WaitAndRetryAsync(
                    options.RetryCount,
                    attempt => TimeSpan.FromMilliseconds(attempt * 500),
                    (result, span) =>
                    {
                        logger.Warning(
                            result.Exception ?? result.Result?.Status?.ErrorData?.Throwable,
                            "Retry publishing message to PubNub for notification {@data}",
                            data);
                    });

            var pubNubResponse = await retryPolicy.ExecuteAsync(() => pubNub.Publish().Message(data).Channel(data.Type).ExecuteAsync());

            if (pubNubResponse.Status == null || pubNubResponse.Status.Error)
            {
                logger.Error(pubNubResponse.Status?.ErrorData.Throwable, "Error while publishing message to PubNub for notification {@data}. Error is {error}", data, pubNubResponse.Status?.ErrorData.Information);
            }
            else
            {
                logger.Information("Successfully published message to PubNub for notification {@data}. Response is {response}", data, pubNubResponse);
            }
        }
    }
}
