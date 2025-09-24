namespace BusinessLayer.PriceDisplay.PriceSeriesSelection.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using BusinessLayer.PriceDisplay.PriceSeriesSelection.DTOs.Output;

    public interface IFilterService
    {
        Task<IEnumerable<DropdownFilterItem>> GetCommodities();
    }
}
