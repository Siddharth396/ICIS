namespace BusinessLayer.PriceEntry.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using BusinessLayer.PriceEntry.Repositories.Models;

    using PriceSeries = PriceSeriesSelection.Repositories.Models.PriceSeries;

    public interface IHalfMonthPriceSeriesItemService
    {
        Task<(BasePriceItem?, BasePriceItem?)> GetHalfMonthPriceSeriesItems(List<BasePriceItem> priceSeriesItems, PriceSeries priceSeries, DateTime assessedDateTime);

        Task<bool> IsHalfMonthPriceSeriesItem(BasePriceItem priceSeriesItem, PriceSeries priceSeries, DateTime assessedDateTime);
    }
}
