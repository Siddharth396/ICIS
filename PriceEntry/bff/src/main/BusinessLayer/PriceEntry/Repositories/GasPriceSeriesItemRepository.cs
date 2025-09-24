namespace BusinessLayer.PriceEntry.Repositories
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using BusinessLayer.PriceEntry.Repositories.Models;

    using Infrastructure.MongoDB.Repositories;
    using Infrastructure.MongoDB.Transactions;

    using MongoDB.Driver;
    using MongoDB.Driver.Linq;

    public class GasPriceSeriesItemRepository : BaseRepository<GasPriceSeriesItem>
    {
        public const string CollectionName = "gas_price_series_items";

        public GasPriceSeriesItemRepository(IMongoDatabase database, IMongoContext mongoContext)
            : base(database, mongoContext)
        {
        }

        public override string DbCollectionName => CollectionName;

        public async Task<GasPriceSeriesItem?> GetGasPriceSeriesItem(
            string marketCode,
            DateTime assessedDateTime,
            DateTime? fulfilmentFrom,
            DateTime? fulfilmentUntil)
        {
            return await Query().
                Where(x => x.MarketCode == marketCode
                        && x.AssessedDateTime == assessedDateTime
                        && x.FulfilmentFromDate == fulfilmentFrom
                        && x.FulfilmentUntilDate == fulfilmentUntil)
                .OrderBy(x => x.Status)
                .FirstOrDefaultAsync();
        }
    }
}
