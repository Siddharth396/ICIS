namespace BusinessLayer.PriceDisplay.GridConfiguration.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using BusinessLayer.PriceDisplay.ContentBlock.Repositories.Models;
    using BusinessLayer.PriceDisplay.GridConfiguration.DTOs;
    using BusinessLayer.PriceDisplay.GridConfiguration.Repositories;
    using Serilog;

    public class GridConfigurationService : IGridConfigurationService
    {
        private readonly ILogger logger;
        private readonly GridConfigurationRepository gridConfigurationRepository;

        public GridConfigurationService(ILogger logger, GridConfigurationRepository gridConfigurationRepository)
        {
            this.logger = logger.ForContext<GridConfigurationService>();
            this.gridConfigurationRepository = gridConfigurationRepository;
        }

        public async Task<GridConfiguration> GetGridConfiguration(
            IEnumerable<string> seriesItemTypeCodes,
            string contentBlockId,
            IEnumerable<ColumnForDisplay>? existingSelectedColumns)
        {
            var localLogger = logger.ForContext("ContentBlockId", contentBlockId)
                                    .ForContext("SeriesItemTypeCodes", seriesItemTypeCodes);

            var gridConfiguration = await gridConfigurationRepository.GetGridConfiguration(seriesItemTypeCodes);

            if (existingSelectedColumns is not null)
            {
                gridConfiguration.Columns = MapToExistingSelectedColumns(gridConfiguration.Columns, existingSelectedColumns);
            }

            localLogger.Debug($"Successfully fetched grid configuration for {seriesItemTypeCodes}");

            return gridConfiguration!;
        }

        private static IEnumerable<Column> MapToExistingSelectedColumns(
            IEnumerable<Column> gridConfigurationColumns,
            IEnumerable<ColumnForDisplay> existingSelectedColumns)
        {
            return gridConfigurationColumns.Select(gridConfiguration =>
            {
                var column = existingSelectedColumns.FirstOrDefault(columns => columns.Field.Equals(gridConfiguration.Field, StringComparison.OrdinalIgnoreCase));

                if (column is not null)
                {
                    gridConfiguration.DisplayOrder = column.DisplayOrder;
                    gridConfiguration.Hidden = column.Hidden;
                }

                return gridConfiguration;
            }).OrderBy(x => x.DisplayOrder)!;
        }
    }
}
