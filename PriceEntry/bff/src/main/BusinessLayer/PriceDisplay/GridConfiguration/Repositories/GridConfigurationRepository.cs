namespace BusinessLayer.PriceDisplay.GridConfiguration.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using BusinessLayer.PriceDisplay.GridConfiguration.Repositories.Models;

    using Infrastructure.Configuration;
    using Infrastructure.MongoDB.Repositories;
    using Infrastructure.MongoDB.Transactions;

    using Microsoft.Extensions.Configuration;

    using MongoDB.Driver;
    using MongoDB.Driver.Linq;

    public class GridConfigurationRepository : BaseRepository<GridConfiguration>
    {
        public const string CollectionName = "price_display.grid_configuration";

        private readonly IConfiguration configuration;

        public GridConfigurationRepository(IMongoDatabase database, IMongoContext mongoContext, IConfiguration configuration)
            : base(database, mongoContext)
        {
            this.configuration = configuration;
        }

        public override string DbCollectionName => CollectionName;

        public async Task<DTOs.GridConfiguration> GetGridConfiguration(IEnumerable<string> seriesItemTypeCodes)
        {
            var gridConfigurations = await Query()
                                          .Where(configuration => configuration.SeriesItemTypeCodes.Count() == seriesItemTypeCodes.Count() &&
                                                                  configuration.SeriesItemTypeCodes.All(collectionSeriesItemTypeCode => seriesItemTypeCodes.Any(seriesItemTypeCode => seriesItemTypeCode.Equals(collectionSeriesItemTypeCode, StringComparison.OrdinalIgnoreCase))))
                                          .SingleAsync();

            return MapToGridConfiguration(gridConfigurations);
        }

        private static IEnumerable<DTOs.Column> MapColumns(IEnumerable<Column> columns)
        {
            return columns.Select((column, index) => new DTOs.Column
            {
                Field = column.Field,
                HeaderName = column.HeaderName,
                CellDataType = column.CellDataType,
                DisplayOrder = index,
                ColumnOrder = index,
                Values = column.Values,
                Pinned = column.Pinned,
                Type = column.Type,
                TooltipField = column.TooltipField,
                CellType = column.CellType,
                Hideable = column.Hideable,
                CustomConfig = column.CustomConfig != null ? new DTOs.CustomConfig
                {
                    PriceDelta = column.CustomConfig.PriceDelta != null ? new DTOs.PriceDelta
                    {
                        PrecisionField = column.CustomConfig.PriceDelta.PrecisionField,
                        PriceDeltaField = column.CustomConfig.PriceDelta.PriceDeltaField,
                        PriceField = column.CustomConfig.PriceDelta.PriceField
                    }
                        : null,
                }
                    : null,
                MinWidth = column.MinWidth,
                MaxWidth = column.MaxWidth,
                Width = column.Width,
                AlternateFields = column.AlternateFields?.Select(MapToAlternateField) ?? [],
                AutoSize = column.AutoSize
            });
        }

        private static DTOs.AlternateField MapToAlternateField(AlternateField alternateField)
        {
            return new(alternateField.SeriesItemTypeCodes, alternateField.Field, alternateField.PriceDeltaField);
        }

        private DTOs.GridConfiguration MapToGridConfiguration(GridConfiguration gridConfiguration)
        {
            return new DTOs.GridConfiguration
            {
                Columns = configuration.IsAuthoring() ? MapColumns(gridConfiguration.Columns) :
                                                        MapColumns(gridConfiguration.Columns.Where(x => x.IsVisibleToSubscriber)),
                Sort = gridConfiguration.Sort != null ? new DTOs.Sort
                {
                    Name = gridConfiguration.Sort.Name,
                    Order = gridConfiguration.Sort.Order
                }
                : null,
                SeriesItemTypeCodes = gridConfiguration.SeriesItemTypeCodes
            };
        }
    }
}
