namespace BusinessLayer.PriceEntry.Services.Calculators.Periods
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IPeriodCalculator
    {
        Task<List<PeriodCalculatorOutputItem>> CalculatePeriods(PeriodCalculatorInput input);
    }
}
