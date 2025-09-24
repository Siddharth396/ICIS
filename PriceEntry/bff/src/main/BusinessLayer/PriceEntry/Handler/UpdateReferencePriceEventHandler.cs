namespace BusinessLayer.PriceEntry.Handler
{
    using System.Threading.Tasks;

    using BusinessLayer.PriceEntry.Services.SeriesItemTypes.SingleValueWithReference;

    using Infrastructure.EventDispatcher;

    public class UpdateReferencePriceEventHandler : IEventHandler<PriceSeriesItemSavedEvent>
    {
        private readonly IReferencePriceService referencePriceService;

        public UpdateReferencePriceEventHandler(IReferencePriceService referencePriceService)
        {
            this.referencePriceService = referencePriceService;
        }

        public async Task HandleAsync(PriceSeriesItemSavedEvent priceSeriesItemSavedEvent)
        {
            await this.referencePriceService.UpdateLngReferencePrice(priceSeriesItemSavedEvent);
        }
    }
}
