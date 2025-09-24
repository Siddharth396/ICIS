namespace BusinessLayer.PriceEntry.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using BusinessLayer.PriceEntry.Repositories.Models;

    using PriceSeries = PriceSeriesSelection.Repositories.Models.PriceSeries;

    public class HalfMonthPriceSeriesItemService : IHalfMonthPriceSeriesItemService
    {
        private readonly IAbsolutePeriodDomainService absolutePeriodDomainService;

        public HalfMonthPriceSeriesItemService(IAbsolutePeriodDomainService absolutePeriodDomainService)
        {
            this.absolutePeriodDomainService = absolutePeriodDomainService;
        }

        public async Task<(BasePriceItem?, BasePriceItem?)> GetHalfMonthPriceSeriesItems(List<BasePriceItem> priceSeriesItems, PriceSeries priceSeries, DateTime assessedDateTime)
        {
            var absolutePeriod = await absolutePeriodDomainService.GetAbsolutePeriod(priceSeries, assessedDateTime);

            var halfMonthPeriodOne = priceSeriesItems.FirstOrDefault(
                x => DateOnly.FromDateTime(x.FulfilmentFromDate.GetValueOrDefault()) == absolutePeriod?.FromDate);

            var halfMonthPeriodTwo = priceSeriesItems.FirstOrDefault(
                x => DateOnly.FromDateTime(x.FulfilmentUntilDate.GetValueOrDefault()) == absolutePeriod?.UntilDate);

            return (halfMonthPeriodOne, halfMonthPeriodTwo);
        }

        public async Task<bool> IsHalfMonthPriceSeriesItem(BasePriceItem priceSeriesItem, PriceSeries priceSeries, DateTime assessedDateTime)
        {
            var absolutePeriod = await absolutePeriodDomainService.GetAbsolutePeriod(priceSeries, assessedDateTime);
            return DateOnly.FromDateTime(priceSeriesItem.FulfilmentFromDate.GetValueOrDefault()) == absolutePeriod?.FromDate
                || DateOnly.FromDateTime(priceSeriesItem.FulfilmentUntilDate.GetValueOrDefault()) == absolutePeriod?.UntilDate;
        }
    }
}
