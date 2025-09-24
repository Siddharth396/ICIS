namespace BusinessLayer.DataPackage.Handler
{
    using System.Threading.Tasks;

    using BusinessLayer.PricingContentPackage.Services;

    using Infrastructure.EventDispatcher;

    public class SendContentPackageToCanvasEventHandler : IEventHandler<DataPackageUpdatedEvent>
    {
        private readonly IContentPackageService contentPackageService;

        public SendContentPackageToCanvasEventHandler(IContentPackageService contentPackageService)
        {
            this.contentPackageService = contentPackageService;
        }

        public Task HandleAsync(DataPackageUpdatedEvent @event)
        {
            return contentPackageService.SaveAndNotifyOnContentPackageUpdates(@event.DataPackage, @event.UserId);
        }
    }
}
