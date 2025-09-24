namespace BusinessLayer.PriceEntry.Handler
{
    using System.Threading.Tasks;

    using BusinessLayer.ContentBlock.Services;

    using Infrastructure.EventDispatcher;
    using Infrastructure.PubNub;
    using Infrastructure.PubNub.Models;

    public class SendPubNubEventHandler : IEventHandler<PriceSeriesItemSavedEvent>
    {
        private readonly IPubNubNotificationService pubNubNotificationService;

        private readonly IContentBlockService contentBlockService;

        public SendPubNubEventHandler(
            IPubNubNotificationService pubNubNotificationService,
            IContentBlockService contentBlockService)
        {
            this.contentBlockService = contentBlockService;
            this.pubNubNotificationService = pubNubNotificationService;
        }

        public async Task HandleAsync(PriceSeriesItemSavedEvent priceSeriesItemSavedEvent)
        {
            var contentBlockIds = await contentBlockService.GetContentBlockIds(priceSeriesItemSavedEvent.SeriesId);

            if (contentBlockIds.Count == 0)
            {
                return;
            }

            pubNubNotificationService.AddPubNubNotification(
                PriceItemEvent.CreatePubNubData(contentBlockIds, priceSeriesItemSavedEvent.AssessedDateTime));
        }
    }
}
