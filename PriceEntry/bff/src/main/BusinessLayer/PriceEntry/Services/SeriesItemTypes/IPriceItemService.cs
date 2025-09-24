namespace BusinessLayer.PriceEntry.Services.SeriesItemTypes
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using BusinessLayer.PriceEntry.DTOs.Input;
    using BusinessLayer.PriceEntry.Repositories.Models;
    using BusinessLayer.PriceEntry.Services.Calculators.Periods;

    using PriceSeriesOutput = BusinessLayer.PriceEntry.DTOs.Output.PriceSeries;

    public interface IPriceItemService
    {
        Task<BasePriceItem> SavePriceEntryData(PriceItemInput priceItemInput);

        Task<(bool Valid, List<string> PriceSeriesItemIds)> ValidatePriceSeriesItems(List<string> seriesIds, DateTime assessedDateTime);

        Task<List<PriceSeriesOutput>> ExtendPriceSeries(
            List<PriceSeriesOutput> priceSeries,
            DateTime assessedDateTime,
            List<PeriodCalculatorOutputItem> absolutePeriods);

        Task<bool> AreAllReferencePriceSeriesPublishedOrInSameContentBlock(List<string> priceSeriesIds, DateTime assessedDateTime, List<string> otherGridsPriceSeriesIds);

        Task DeltaCorrectionForNextDate(List<string> priceSeriesItemIds);
    }
}