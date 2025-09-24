namespace BusinessLayer.PriceEntry.Handler
{
    using System.Threading.Tasks;

    using BusinessLayer.PriceEntry.Services;

    using Infrastructure.EventDispatcher;

    public class SaveDerivedPriceEventHandler : IEventHandler<PriceSeriesItemSavedEvent>
    {
        private readonly IDerivedPriceService derivedPriceService;

        public SaveDerivedPriceEventHandler(
            IDerivedPriceService derivedPriceService)
        {
            this.derivedPriceService = derivedPriceService;
        }

        public async Task HandleAsync(PriceSeriesItemSavedEvent priceSeriesItemSavedEvent)
        {
            await derivedPriceService.UpdateDerivedPrices(
                priceSeriesItemSavedEvent.SeriesId,
                priceSeriesItemSavedEvent.AssessedDateTime,
                priceSeriesItemSavedEvent.OperationType);
        }
    }
}