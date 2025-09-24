namespace BusinessLayer.PriceEntry.Services
{
    using System;
    using System.Threading.Tasks;

    using BusinessLayer.PriceSeriesSelection.Repositories.Models;

    using Infrastructure.Services.Workflow;

    public interface IDerivedPriceService
    {
        Task UpdateDerivedPrices(string seriesId, DateTime assessedDateTime, OperationType operationType);

        Task<bool> IsDerivedSeriesImpacted(
            string seriesId,
            PriceSeries derivedPriceSeries,
            DateTime assessedDateTime);
    }
}
