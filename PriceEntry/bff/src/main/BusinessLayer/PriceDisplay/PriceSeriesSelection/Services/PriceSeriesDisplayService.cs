namespace BusinessLayer.PriceDisplay.PriceSeriesSelection.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using BusinessLayer.Helpers;
    using BusinessLayer.PriceDisplay.PriceSeriesSelection.DTOs.Output;
    using BusinessLayer.PriceDisplay.PriceSeriesSelection.Repositories;
    using BusinessLayer.PriceDisplay.PriceSeriesSelection.Repositories.Models;

    using Microsoft.Extensions.Internal;

    public class PriceSeriesDisplayService : IPriceSeriesDisplayService
    {
        private readonly PriceSeriesRepository priceSeriesItemRepository;
        private readonly ISystemClock systemClock;

        public PriceSeriesDisplayService(PriceSeriesRepository priceSeriesItemRepository, ISystemClock systemClock)
        {
            this.priceSeriesItemRepository = priceSeriesItemRepository;
            this.systemClock = systemClock;
        }

        public async Task<PriceSeriesSelectionItem> GetActivePriceSeries(
            List<Guid> commodities)
        {
            var priceSeries = await priceSeriesItemRepository.GetActivePriceSeries(commodities, systemClock);

            return GetPriceSeries(priceSeries);
        }

        public async Task<PriceSeriesSelectionItem> GetAllPriceSeries(
            List<Guid> commodities)
        {
            var priceSeries = await priceSeriesItemRepository.GetPriceSeriesForDisplayTool(commodities);

            return GetPriceSeries(priceSeries);
        }

        private PriceSeriesSelectionItem GetPriceSeries(IEnumerable<PriceSeries> priceSeries)
        {
            return new PriceSeriesSelectionItem()
            {
                PriceSeriesDetails = priceSeries.Select(y =>
                {
                    var name = y.SeriesName.English;

                    return new PriceSeriesDetail()
                    {
                        Id = y.Id,
                        Name = y.GetPriceSeriesName(name, systemClock.UtcNow.Date),
                        SeriesItemTypeCode = y.SeriesItemTypeCode,
                        ItemFrequencyId = y.ItemFrequency.Guid,
                        RegionId = y.Location.Region.Guid,
                        PriceCategoryId = y.PriceCategory.Guid,
                        PriceSettlementTypeId = y.PriceSettlementType?.Guid ?? Guid.Empty,
                    };
                }),

                Regions = priceSeries.Select(region =>
                                new DropdownFilterItem
                                {
                                    Id = region.Location.Region.Guid,
                                    Name = region.Location.Region?.Name.English?.ToPascalCase() ?? string.Empty
                                }).DistinctBy(x => x.Id),

                PriceCategories = priceSeries.Select(cat =>
                                new DropdownFilterItem
                                {
                                    Id = cat.PriceCategory.Guid,
                                    Name = cat.PriceCategory.Name.English?.ToPascalCase() ?? string.Empty
                                }).DistinctBy(x => x.Id),

                AssessedFrequencies = priceSeries.Select(af =>
                                new DropdownFilterItem
                                {
                                    Id = af.ItemFrequency.Guid,
                                    Name = af.ItemFrequency.Name.English?.ToPascalCase() ?? string.Empty
                                }).DistinctBy(x => x.Id),

                TransactionTypes = priceSeries.Select(tt =>
                                new DropdownFilterItem
                                {
                                    Id = tt.PriceSettlementType?.Guid ?? Guid.Empty,
                                    Name = tt.PriceSettlementType?.Name.English?.ToPascalCase() ?? "N/A"
                                }).DistinctBy(x => x.Id)
            };
        }
    }
}
