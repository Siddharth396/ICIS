namespace BusinessLayer.PriceSeriesSelection.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using BusinessLayer.PriceEntry.ValueObjects;
    using BusinessLayer.PriceSeriesSelection.Repositories.Models;

    using Infrastructure.MongoDB.Repositories;
    using Infrastructure.MongoDB.Transactions;

    using Microsoft.Extensions.Internal;

    using MongoDB.Driver;
    using MongoDB.Driver.Linq;

    using PriceDerivationType = BusinessLayer.PriceEntry.ValueObjects.PriceDerivationType;

    public class PriceSeriesRepository : BaseRepository<PriceSeries>
    {
        public const string CollectionName = "price_series";
        private readonly ISystemClock clock;

        public PriceSeriesRepository(
            IMongoDatabase database,
            IMongoContext mongoContext,
            ISystemClock clock)
            : base(database, mongoContext)
        {
            this.clock = clock;
        }

        public override string DbCollectionName => CollectionName;

        public async Task<List<PriceSeries>> GetPriceSeriesWithFilters(
            Guid commodityId,
            Guid priceCategoryId,
            Guid regionId,
            Guid? priceSettlementTypeId,
            Guid itemFrequencyId)
        {
            return await Query()
               .Where(x => x.Commodity.Guid == commodityId
                        && x.PriceCategory.Guid == priceCategoryId
                        && x.PriceSettlementType!.Guid == priceSettlementTypeId
                        && x.ItemFrequency.Guid == itemFrequencyId
                        && x.Location.Region.Guid == regionId
                        && (x.PriceDerivationType == null || x.PriceDerivationType.Code != PriceDerivationType.Spread.Value)
                        && (x.TerminationDate == null || clock.UtcNow.Date <= x.TerminationDate))
               .ToListAsync();
        }

        public Task<List<PriceSeries>> GetPriceSeriesByIds(IEnumerable<string> priceSeriesIds)
        {
            return Query().Where(x => priceSeriesIds.Contains(x.Id)).ToListAsync();
        }

        public Task<PriceSeries> GetPriceSeriesById(string priceSeriesId)
        {
            return Query().Where(x => x.Id == priceSeriesId).FirstOrDefaultAsync();
        }

        public Task<List<PriceSeries>> GetDerivedPriceSeries(string priceSeriesId)
        {
            return Query().Where(x => x.DerivationInputs != null && x.DerivationInputs.Any(y => y.SeriesId == priceSeriesId)).ToListAsync();
        }

        public async Task<PriceSeries> GetSubjectPriceSeriesFromObjectPriceSeriesId(
            string objectPriceSeriesId,
            PriceSeriesLinkReasonCode hasSubsequentAssessmentForSameFulfilmentPeriod)
        {
            return await Query()
               .Where(
                    x => x.PriceSeriesLinks != null
                         && x.PriceSeriesLinks.Any(
                             y => y.ObjectSeriesId == objectPriceSeriesId
                                  && hasSubsequentAssessmentForSameFulfilmentPeriod.Value == y.ReasonCode))
               .FirstOrDefaultAsync();
        }

        public Task<List<PriceSeries>> GetActivePriceSeriesByIds(IEnumerable<string> priceSeriesIds, DateTime assessedDateTime)
        {
            var assessedDate = assessedDateTime.Date;
            return Query()
               .Where(x => priceSeriesIds.Contains(x.Id) &&
                     (x.TerminationDate == null || assessedDate <= x.TerminationDate) &&
                      x.LaunchDate <= assessedDate)
               .ToListAsync();
        }

        public async Task<List<PriceSeries>> GetPriceSeriesToExtractFilters()
        {
            return await Query()
                .Where(x => x.Commodity != null && x.Commodity.Guid != null &&
                            x.PriceCategory != null &&
                            x.ItemFrequency != null &&
                            x.Location != null && x.Location.Region != null && x.Location.Region.Guid != null &&
                            (x.PriceDerivationType == null || x.PriceDerivationType.Code != PriceDerivationType.Spread.Value))
                .ToListAsync();
        }

        public async Task<PriceSeries> GetLngDerivedPriceSeries()
        {
            return await Query()
            .Where(x => CommodityName.LNG.Value.ToUpper() == x.Commodity.Name.English
                        && PriceCategoryCode.Derived.Value == x.PriceCategory.Code)
            .FirstAsync();
        }
    }
}
