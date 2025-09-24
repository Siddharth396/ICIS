namespace BusinessLayer.PriceSeries.Services
{
    using System.Threading.Tasks;

    using BusinessLayer.PriceSeriesSelection.Repositories.Models;

    public interface IPriceSeriesDomainService
    {
        Task<PriceSeries> GetPriceSeriesById(string priceSeriesId);
    }
}
