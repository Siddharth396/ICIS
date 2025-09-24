namespace BusinessLayer.PriceDisplay.PriceSeriesSelection.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using BusinessLayer.PriceDisplay.PriceSeriesSelection.DTOs.Output;
    using BusinessLayer.PriceDisplay.PriceSeriesSelection.Repositories.Models;

    using Infrastructure.MongoDB.Repositories;
    using Infrastructure.MongoDB.Transactions;

    using Microsoft.Extensions.Internal;

    using MongoDB.Driver;
    using MongoDB.Driver.Linq;

    public class PriceSeriesRepository : BaseRepository<PriceSeries>
    {
        public const string CollectionName = "price_series";

        public PriceSeriesRepository(IMongoDatabase database, IMongoContext mongoContext)
            : base(database, mongoContext)
        {
        }

        public override string DbCollectionName => CollectionName;

        public async Task<IList<PriceSeries>> GetPriceSeriesForDisplayTool(List<Guid> commodities)
        {
            return await Query()
              .Where(x => commodities.Contains(x.Commodity.Guid))
              .ToListAsync();
        }

        public async Task<IEnumerable<PriceSeries>> GetActivePriceSeries(
           List<Guid> commodities,
           ISystemClock systemClock)
        {
            return await Query()
              .Where(x => commodities.Contains(x.Commodity.Guid) &&
                          systemClock.UtcNow.Date >= x.LaunchDate &&
                          (x.TerminationDate == null || systemClock.UtcNow.Date <= x.TerminationDate))
              .ToListAsync();
        }

        public async Task<IEnumerable<DropdownFilterItem>> GetCommodities()
        {
            return await Query()
                .Select(x => new DropdownFilterItem
                {
                    Id = x.Commodity.Guid,
                    Name = x.Commodity.Name.English ?? string.Empty
                }).ToListAsync();
        }
    }
}
