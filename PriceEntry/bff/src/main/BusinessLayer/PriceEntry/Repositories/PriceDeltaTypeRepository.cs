namespace BusinessLayer.PriceEntry.Repositories
{
    using System.Threading.Tasks;

    using Infrastructure.MongoDB.Repositories;
    using Infrastructure.MongoDB.Transactions;

    using MongoDB.Driver;
    using MongoDB.Driver.Linq;

    using PriceDeltaTypeModel = BusinessLayer.PriceEntry.Repositories.Models.PriceDeltaType;
    using PriceDeltaTypeValObj = BusinessLayer.PriceEntry.ValueObjects.PriceDeltaType;

    public class PriceDeltaTypeRepository : BaseRepository<PriceDeltaTypeModel>
    {
        public const string CollectionName = "price_delta_types";

        public PriceDeltaTypeRepository(IMongoDatabase database, IMongoContext mongoContext)
            : base(database, mongoContext)
        {
        }

        public override string DbCollectionName => CollectionName;

        public async Task<PriceDeltaTypeModel> GetPriceDeltaType(PriceDeltaTypeValObj code)
        {
            var priceDeltaType = await Query()
                                    .Where(x => x.Code == code.Value && x.IsDeleted == false)
                                    .SingleAsync();
            return priceDeltaType;
        }
    }
}