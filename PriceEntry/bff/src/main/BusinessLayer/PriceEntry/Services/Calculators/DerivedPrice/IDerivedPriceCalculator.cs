namespace BusinessLayer.PriceEntry.Services.Calculators.DerivedPrice
{
    using System;
    using System.Threading.Tasks;

    using BusinessLayer.PriceSeriesSelection.Repositories.Models;

    using Infrastructure.Services.Workflow;

    public interface IDerivedPriceCalculator
    {
        Task<decimal?> CalculatePrice(PriceSeries priceSeries, DateTime assessedDateTime, OperationType operationType);
    }
}
