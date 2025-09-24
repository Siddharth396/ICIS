namespace BusinessLayer.PriceEntry.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using BusinessLayer.PriceEntry.Repositories.Models;
    using BusinessLayer.PriceEntry.ValueObjects;

    using global::AutoMapper;

    using Infrastructure.MongoDB.Repositories;
    using Infrastructure.MongoDB.Transactions;

    using MongoDB.Driver;
    using MongoDB.Driver.Linq;

    public class GridConfigurationRepository : BaseRepository<GridConfiguration>
    {
        public const string CollectionName = "grid_configuration";

        private readonly IMapper mapper;

        public GridConfigurationRepository(IMongoDatabase database, IMongoContext mongoContext, IMapper mapper)
            : base(database, mongoContext)
        {
            this.mapper = mapper;
        }

        public override string DbCollectionName => CollectionName;

        public async Task<List<DTOs.Output.GridConfiguration>> GetGridConfigurations(IReadOnlyList<SeriesItemTypeCode> seriesItemTypeCodes)
        {
            var gridConfigurations = await Query()
                      .Where(configuration =>
                           seriesItemTypeCodes.Any(code => code.Value == configuration.SeriesItemTypeCode))
                      .ToListAsync();

            return mapper.Map<List<DTOs.Output.GridConfiguration>>(gridConfigurations);
        }
    }
}
