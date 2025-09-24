namespace BusinessLayer.PriceSeriesSelection.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using BusinessLayer.Helpers;
    using BusinessLayer.PriceEntry.ValueObjects;
    using BusinessLayer.PriceSeriesSelection.DTOs.Output;
    using BusinessLayer.PriceSeriesSelection.Repositories;
    using BusinessLayer.PriceSeriesSelection.Repositories.Models;
    using BusinessLayer.PriceSeriesSelection.ValueObjects;

    using PriceDerivationType = BusinessLayer.PriceEntry.ValueObjects.PriceDerivationType;

    public class PriceSeriesService : IPriceSeriesService
    {
        private static readonly Guid NotApplicableSettlementTypeId = Guid.Empty;

        private readonly PriceSeriesRepository priceSeriesRepository;

        public PriceSeriesService(PriceSeriesRepository priceSeriesRepository)
        {
            this.priceSeriesRepository = priceSeriesRepository;
        }

        public async Task<List<PriceSeriesSelectionItem>> GetPriceSeriesWithFilters(
            Guid commodityId,
            Guid priceCategoryId,
            Guid regionId,
            Guid priceSettlementTypeId,
            Guid itemFrequencyId)
        {
            var priceSettlementTypeIdParam = NotApplicableSettlementTypeId.Equals(priceSettlementTypeId)
                                        ? (Guid?)null
                                        : priceSettlementTypeId;

            var priceSeries = await priceSeriesRepository.GetPriceSeriesWithFilters(
                                  commodityId,
                                  priceCategoryId,
                                  regionId,
                                  priceSettlementTypeIdParam,
                                  itemFrequencyId);

            var lngDerivedPriceSeries = await priceSeriesRepository.GetLngDerivedPriceSeries();

            return priceSeries
               .Where(ps => priceCategoryId != lngDerivedPriceSeries.PriceCategory.Guid ||
                        (priceCategoryId == lngDerivedPriceSeries.PriceCategory.Guid
                         && commodityId == lngDerivedPriceSeries.Commodity.Guid
                         && (PeriodTypeCode.HalfMonth.Matches(ps.RelativeFulfilmentPeriod?.PeriodType.Code) ||
                             PeriodTypeCode.MidMonth.Matches(ps.RelativeFulfilmentPeriod?.PeriodType.Code))
                         && PriceDerivationType.Index.Matches(ps.PriceDerivationType?.Code)))
               .GroupBy(x => x.SeriesItemTypeCode).Select(
                    y => new PriceSeriesSelectionItem
                    {
                        SeriesItemTypeCode = y.Key,
                        PriceSeriesDetails = y.Select(ps =>
                                new PriceSeriesDetail
                                {
                                    Id = ps.Id,
                                    Name = ps.SeriesName.English,
                                    ScheduleId = ps.PublicationSchedules?.FirstOrDefault()?.ScheduleId,
                                })
                           .OrderBy(x => x.Name)
                           .ToList(),
                    })
               .OrderByDescending(x => x.SeriesItemTypeCode)
               .ToList();
        }

        public async Task<PriceSeries> GetPriceSeriesById(string priceSeriesId)
        {
            return await priceSeriesRepository.GetPriceSeriesById(priceSeriesId);
        }

        public async Task<List<PriceSeries>> GetPriceSeriesByIds(List<string> priceSeriesIds)
        {
            return await priceSeriesRepository.GetPriceSeriesByIds(priceSeriesIds);
        }

        public async Task<List<string>> GetPriceEntryValidDerivedPriceSeriesIds(string priceSeriesId)
        {
            var priceEntryDerivedPriceSeries = await priceSeriesRepository.GetDerivedPriceSeries(priceSeriesId);
            return priceEntryDerivedPriceSeries.Where(PriceSeries.IsValidDerivedSeries).Select(x => x.Id).ToList();
        }

        public async Task<List<string>> GetCalculationServiceValidDerivedPriceSeriesIds(string priceSeriesId)
        {
            var priceEntryDerivedPriceSeries = await priceSeriesRepository.GetDerivedPriceSeries(priceSeriesId);
            return priceEntryDerivedPriceSeries.Where(x => !PriceSeries.IsValidDerivedSeries(x)).Select(y => y.Id).ToList();
        }

        public async Task<List<PriceSeries>> GetActivePriceSeriesByIds(List<string> priceSeriesIds, DateTime assessedDateTime)
        {
            return await priceSeriesRepository.GetActivePriceSeriesByIds(priceSeriesIds, assessedDateTime);
        }

        public async Task<List<Filter>> GetFilters(IEnumerable<string> selectedPriceSeriesIds)
        {
            var series = await priceSeriesRepository.GetPriceSeriesToExtractFilters();

            var selectedSeries = series.Where(x => selectedPriceSeriesIds.Contains(x.Id)).ToList();
            var hasSelected = selectedSeries.Count > 0;

            var commodity = new Filter
            {
                Name = FilterName.Commodity.Value,
                FilterDetails = series.DistinctBy(x => x.Commodity.Guid)
                    .Select(y => new FilterDetail
                    {
                        Id = y.Commodity.Guid,
                        Name = GetFilterName(y.Commodity.Name.English),
                        IsDefault = hasSelected && selectedSeries.Any(x => x.Commodity.Guid == y.Commodity.Guid),
                    })
                    .OrderBy(x => x.Name)
                    .ToList(),
            };

            var region = new Filter
            {
                Name = FilterName.Region.Value,
                FilterDetails = series.DistinctBy(x => x.Location.Region.Guid)
                    .Select(y => new FilterDetail
                    {
                        Id = y.Location.Region.Guid,
                        Name = GetFilterName(y.Location.Region.Name.English),
                        IsDefault = hasSelected && selectedSeries.Any(x => x.Location.Region.Guid == y.Location.Region.Guid),
                    })
                    .OrderBy(x => x.Name)
                    .ToList(),
            };

            var priceCategory = new Filter
            {
                Name = FilterName.PriceCategory.Value,
                FilterDetails = series.DistinctBy(x => x.PriceCategory.Guid)
                   .Select(y => new FilterDetail
                   {
                       Id = y.PriceCategory.Guid,
                       Name = GetFilterName(y.PriceCategory.Name.English),
                       IsDefault = hasSelected && selectedSeries.Any(x => x.PriceCategory.Guid == y.PriceCategory.Guid),
                   })
                   .OrderBy(x => x.Name)
                   .ToList(),
            };

            var priceSettlementType = new Filter
            {
                Name = FilterName.PriceSettlementType.Value,
                FilterDetails = series.Where(x => x.PriceSettlementType is not null)
                    .DistinctBy(x => x.PriceSettlementType!.Guid)
                    .Select(y => new FilterDetail
                    {
                        Id = y.PriceSettlementType!.Guid,
                        Name = GetFilterName(y.PriceSettlementType.Name.English),
                        IsDefault = hasSelected && selectedSeries.Any(x => x.PriceSettlementType?.Guid == y.PriceSettlementType!.Guid),
                    })
                    .OrderBy(x => x.Name)
                    .ToList(),
            };

            priceSettlementType.FilterDetails.Insert(0, new FilterDetail
            {
                Id = NotApplicableSettlementTypeId,
                Name = "N/A",
                IsDefault = selectedSeries.Any(x => x.PriceSettlementType == null)
            });

            var itemFrequency = new Filter
            {
                Name = FilterName.Frequency.Value,
                FilterDetails = series.DistinctBy(x => x.ItemFrequency.Guid)
                    .Select(y => new FilterDetail
                    {
                        Id = y.ItemFrequency.Guid,
                        Name = GetFilterName(y.ItemFrequency.Name.English),
                        IsDefault = hasSelected && selectedSeries.Any(x => x.ItemFrequency.Guid == y.ItemFrequency.Guid),
                    })
                    .OrderBy(x => x.Name)
                    .ToList(),
            };

            return new List<Filter>
            {
                commodity,
                region,
                priceCategory,
                priceSettlementType,
                itemFrequency
            };
        }

        private static string GetFilterName(string? name)
        {
            return string.IsNullOrEmpty(name) ? string.Empty : name.ToPascalCase();
        }
    }
}
