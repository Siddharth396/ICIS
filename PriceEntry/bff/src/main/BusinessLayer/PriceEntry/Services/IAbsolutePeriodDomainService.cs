namespace BusinessLayer.PriceEntry.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using BusinessLayer.PriceEntry.Repositories.Models;
    using BusinessLayer.PriceEntry.Services.Calculators.Periods;
    using BusinessLayer.PriceSeriesSelection.Repositories.Models;

    public interface IAbsolutePeriodDomainService
    {
        Task<PeriodCalculatorOutputItem?> GetAbsolutePeriod(PriceSeries priceSeries, DateTime assessedDateTime);

        Task<List<PeriodCalculatorOutputItem>> GetAbsolutePeriods(List<PriceSeriesAggregation> priceSeriesAggregations, DateTime assessedDateTime);

        PeriodCalculatorOutputItem? FilterAbsolutePeriod(
            List<PeriodCalculatorOutputItem> absolutePeriods,
            PriceSeriesAggregation priceSeriesAggregation,
            DateTime assessedDateTime);

        Task<PeriodCalculatorOutputItem?> GetAbsolutePeriod(string seriesId, DateTime assessedDateTime);
    }
}
