namespace BusinessLayer.PriceEntry.Services.SeriesItemTypes.SingleValueWithReference
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using BusinessLayer.PriceEntry.DTOs.Input;
    using BusinessLayer.PriceEntry.Handler;
    using BusinessLayer.PriceEntry.Repositories.Models;

    public interface IReferencePriceService
    {
        Task<ReferencePrice> GetReferencePrice(
            string referenceMarketName,
            DateTime assessedDateTime,
            DateTime? fulfilmentFrom,
            DateTime? fulfilmentUntil,
            string seriesId);

        Task<List<string>> UpdateGasReferencePrice(GasPricePayload gasPricePayload);

        Task UpdateLngReferencePrice(PriceSeriesItemSavedEvent priceSeriesItemSavedEvent);

        Task<(DateTime? FulfilmentFrom, DateTime? FulfilmentUntil)> GetFulfilmentDatesForM1Period(DateTime assessedDateTime);

        Task<List<string>> GetPriceSeriesIdsForReferencePriceSeriesItems(List<string> priceSeriesId, DateTime assessedDateTime);

        Task<IDictionary<string, bool>> HasReferencePriceSeriesItems(List<string> priceSeriesIds, DateTime assessedDateTime);

        bool UseMonthPlusOnePeriod(DateTime? fulfilmentFromDateTime, DateTime? fulfilmentUntilDateTime, string? relativeFulfilmentPeriod);
    }
}
