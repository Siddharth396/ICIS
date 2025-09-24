namespace Test.Infrastructure.Services
{
    using System;
    using System.Threading.Tasks;

    using Test.Infrastructure.Mongo.Repositories;

    public class PriceSeriesTestService
    {
        private readonly PriceSeriesRepository priceSeriesRepository;

        public PriceSeriesTestService(PriceSeriesRepository priceSeriesRepository)
        {
            this.priceSeriesRepository = priceSeriesRepository;
        }

        public Task SetTerminationDate(string seriesId, DateTime? terminationDate)
        {
            return priceSeriesRepository.SetTerminationDate(seriesId, terminationDate);
        }
    }
}
