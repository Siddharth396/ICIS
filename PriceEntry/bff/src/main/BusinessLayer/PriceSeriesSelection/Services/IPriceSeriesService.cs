namespace BusinessLayer.PriceSeriesSelection.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using BusinessLayer.PriceSeriesSelection.DTOs.Output;
    using BusinessLayer.PriceSeriesSelection.Repositories.Models;

    public interface IPriceSeriesService
    {
        Task<List<PriceSeriesSelectionItem>> GetPriceSeriesWithFilters(
            Guid commodityId,
            Guid priceCategoryId,
            Guid regionId,
            Guid priceSettlementTypeId,
            Guid itemFrequencyId);

        Task<PriceSeries> GetPriceSeriesById(string priceSeriesId);

        Task<List<PriceSeries>> GetPriceSeriesByIds(List<string> priceSeriesIds);

        Task<List<string>> GetPriceEntryValidDerivedPriceSeriesIds(string priceSeriesIds);

        Task<List<string>> GetCalculationServiceValidDerivedPriceSeriesIds(string priceSeriesId);

        Task<List<PriceSeries>> GetActivePriceSeriesByIds(List<string> priceSeriesIds, DateTime assessedDateTime);

        Task<List<Filter>> GetFilters(IEnumerable<string> selectedPriceSeriesIds);
    }
}
