namespace BusinessLayer.PriceEntry.Services
{
    using System.Threading.Tasks;

    using BusinessLayer.PriceEntry.ValueObjects;

    public interface IPriceSeriesItemEditabilityService
    {
        Task<bool> IsPriceSeriesItemEditable(
            DataPackageKey dataPackageKey,
            bool isReviewMode,
            string? priceSeriesItemStatus);
    }
}
