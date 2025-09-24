namespace BusinessLayer.PriceSeries.Services
{
    using System.Threading.Tasks;

    using BusinessLayer.PriceSeriesSelection.Repositories;
    using BusinessLayer.PriceSeriesSelection.Repositories.Models;

    using Infrastructure.Attributes.BusinessAnnotations;

    [DomainService]
    public class PriceSeriesDomainService : IPriceSeriesDomainService
    {
        private readonly PriceSeriesRepository priceSeriesRepository;

        public PriceSeriesDomainService(PriceSeriesRepository priceSeriesRepository)
        {
            this.priceSeriesRepository = priceSeriesRepository;
        }

        public Task<PriceSeries> GetPriceSeriesById(string priceSeriesId)
        {
            return priceSeriesRepository.GetPriceSeriesById(priceSeriesId);
        }
    }
}
