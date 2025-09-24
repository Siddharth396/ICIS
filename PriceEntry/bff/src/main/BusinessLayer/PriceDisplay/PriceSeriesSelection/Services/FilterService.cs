namespace BusinessLayer.PriceDisplay.PriceSeriesSelection.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using BusinessLayer.Helpers;
    using BusinessLayer.PriceDisplay.PriceSeriesSelection.DTOs.Output;
    using BusinessLayer.PriceDisplay.PriceSeriesSelection.Repositories;

    public class FilterService : IFilterService
    {
        private readonly PriceSeriesRepository priceSeriesRepository;

        public FilterService(PriceSeriesRepository priceSeriesRepository)
        {
            this.priceSeriesRepository = priceSeriesRepository;
        }

        public async Task<IEnumerable<DropdownFilterItem>> GetCommodities()
        {
            var commodities = await priceSeriesRepository.GetCommodities();
            return commodities
                .DistinctBy(x => x.Id)
                .OrderBy(x => x.Name)
                .Select(x => new DropdownFilterItem
                {
                    Id = x.Id,
                    Name = x.Name.ToPascalCase()
                }).AsEnumerable();
        }
    }
}
