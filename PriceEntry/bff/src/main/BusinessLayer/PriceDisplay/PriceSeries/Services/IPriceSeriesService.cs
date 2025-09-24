namespace BusinessLayer.PriceDisplay.PriceSeries.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using BusinessLayer.PriceDisplay.ContentBlock.Repositories.Models;
    using BusinessLayer.PriceDisplay.PriceSeries.DTOs;

    public interface IPriceSeriesService
    {
        Task<IEnumerable<PriceSeriesResponse>> GetPriceSeriesDetails(
            IList<RowForDisplay> rowsForDisplay);

        Task<IEnumerable<PriceSeriesResponse>> GetPriceSeriesDetails(IList<string> seriesIds);

        Task<IEnumerable<PriceSeriesResponse>> GetPublishedPriceSeriesDetails(
            IList<RowForDisplay> rowsForDisplay,
            long assessedDateTime);

        Task<IEnumerable<PriceSeriesResponse>> GetCurrentOrPendingPriceSeriesDetails(
            IList<RowForDisplay> rowsForDisplay,
            DateTime assessedDateTime);
    }
}