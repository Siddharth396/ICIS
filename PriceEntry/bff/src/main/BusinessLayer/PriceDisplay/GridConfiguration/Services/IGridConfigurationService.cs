namespace BusinessLayer.PriceDisplay.GridConfiguration.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using BusinessLayer.PriceDisplay.ContentBlock.Repositories.Models;
    using BusinessLayer.PriceDisplay.GridConfiguration.DTOs;

    public interface IGridConfigurationService
    {
        Task<GridConfiguration> GetGridConfiguration(
            IEnumerable<string> seriesItemTypeCode,
            string contentBlockId,
            IEnumerable<ColumnForDisplay>? columns);
    }
}
