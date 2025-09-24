namespace BusinessLayer.PriceDisplay.PriceSeriesSelection.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using BusinessLayer.PriceDisplay.PriceSeriesSelection.DTOs.Output;

    public interface IPriceSeriesDisplayService
    {
        Task<PriceSeriesSelectionItem> GetActivePriceSeries(
            List<Guid> commodities);

        Task<PriceSeriesSelectionItem> GetAllPriceSeries(
            List<Guid> commodities);
    }
}
