namespace BusinessLayer.PriceDisplay.PriceSeries.Services
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;

    using BusinessLayer.Helpers;
    using BusinessLayer.PriceDisplay.ContentBlock.Repositories.Models;
    using BusinessLayer.PriceDisplay.PriceSeries.DTOs;
    using BusinessLayer.PriceDisplay.PriceSeries.Repositories;
    using BusinessLayer.PriceDisplay.PriceSeries.Repositories.Models;

    using Microsoft.Extensions.Internal;

    using Serilog;
    using Serilog.Context;

    using PriceDeltaTypeValue = BusinessLayer.PriceDisplay.PriceSeries.ValueObjects;

    public class PriceSeriesService : IPriceSeriesService
    {
        private readonly ILogger logger;
        private readonly PriceSeriesForDisplayRepository priceSeriesRepository;
        private readonly ISystemClock clock;

        public PriceSeriesService(
            PriceSeriesForDisplayRepository priceSeriesRepository, ILogger logger, ISystemClock systemClock)
        {
            this.logger = logger.ForContext<PriceSeriesService>();
            this.priceSeriesRepository = priceSeriesRepository;
            this.clock = systemClock;
        }

        public async Task<IEnumerable<PriceSeriesResponse>> GetPriceSeriesDetails(IList<RowForDisplay> rowsForDisplay)
        {
            var seriesIds = GetSeriesIds(rowsForDisplay);

            var seriesItemDetails = await priceSeriesRepository.GetPriceSeriesItemDetail(seriesIds);

            var result = MapToPriceSeries(seriesItemDetails);

            logger.Debug("Price series details retrieved successfully");

            return GetOrderedPriceSeries(result, rowsForDisplay);
        }

        public async Task<IEnumerable<PriceSeriesResponse>> GetPriceSeriesDetails(IList<string> seriesIds)
        {
            var seriesItemDetails = await priceSeriesRepository.GetPriceSeriesItemDetail(seriesIds);

            var result = MapToPriceSeries(seriesItemDetails);

            logger.Debug("Price series details retrieved successfully");

            return result;
        }

        public async Task<IEnumerable<PriceSeriesResponse>> GetPublishedPriceSeriesDetails(IList<RowForDisplay> rowsForDisplay, long assessedDateTime)
        {
            var fromDate = assessedDateTime.FromUnixTimeMilliseconds();

            using (LogContext.PushProperty("AssessedDateTime", fromDate))
            {
                var seriesIds = GetSeriesIds(rowsForDisplay);

                var seriesItemDetails = await priceSeriesRepository.GetPublishedPriceSeriesItemDetail(seriesIds, fromDate, clock.UtcNow.Date);
                var result = MapToPriceSeries(seriesItemDetails);

                logger.Debug("Price series details for the selected date retrieved successfully");

                return GetOrderedPriceSeries(result, rowsForDisplay);
            }
        }

        public async Task<IEnumerable<PriceSeriesResponse>> GetCurrentOrPendingPriceSeriesDetails(
            IList<RowForDisplay> rowsForDisplay,
            DateTime assessedDateTime)
        {
            using (LogContext.PushProperty("AssessedDateTime", assessedDateTime))
            {
                var seriesIds = GetSeriesIds(rowsForDisplay);
                var seriesItemDetails = await priceSeriesRepository.GetCurrentOrPendingPriceSeriesDetails(seriesIds, assessedDateTime);
                var result = MapToPriceSeries(seriesItemDetails);

                logger.Debug("Price series details for the selected date retrieved successfully");

                return GetOrderedPriceSeries(result, rowsForDisplay);
            }
        }

        private static string GetUnitDisplayName(PriceSeriesAggregation priceSeries)
        {
            return !string.IsNullOrWhiteSpace(priceSeries.CurrencyUnit.Code)
                   && !string.IsNullOrWhiteSpace(priceSeries.MeasureUnit.Symbol)
                       ? $"{priceSeries.CurrencyUnit.Code}/{priceSeries.MeasureUnit.Symbol}"
                       : string.Empty;
        }

        private static List<string> GetSeriesIds(IEnumerable<RowForDisplay> rowsForDisplay)
        {
            return rowsForDisplay
                    .Select(x => x.PriceSeriesId)
                    .Where(id => !string.IsNullOrWhiteSpace(id))
                    .ToList();
        }

        private static IEnumerable<PriceSeriesResponse> GetOrderedPriceSeries(
            IEnumerable<PriceSeriesResponse> priceSeriesResponses,
            IEnumerable<RowForDisplay> rowForDisplays)
        {
            return priceSeriesResponses.OrderBy(x =>
            {
                var row = rowForDisplays.FirstOrDefault(r => r.PriceSeriesId == x.Id);
                return row?.DisplayOrder ?? int.MaxValue;
            });
        }

        private IEnumerable<PriceSeriesResponse> MapToPriceSeries(List<PriceSeriesAggregation> seriesItemDetails)
        {
            return seriesItemDetails?
               .Select(item =>
               {
                   bool isNonMarketAdjustment = PriceDeltaTypeValue.PriceDeltaType.NonMarketAdjustment.Matches(item.PriceDeltaType?.Code);
                   return new PriceSeriesResponse()
                   {
                       Id = item.Id,
                       PriceSeriesName = item.SeriesShortName?.English ?? item.SeriesName.English,
                       UnitDisplay = GetUnitDisplayName(item),
                       ItemFrequencyName = item.ItemFrequency?.Name?.English,
                       DataUsed = item.PriceSeriesItem?.DataUsed,
                       PriceDeltaType = item.PriceDeltaType?.Code ?? string.Empty,
                       Price = item.PriceSeriesItem?.Price,
                       PriceLow = item.PriceSeriesItem?.PriceLow,
                       PriceHigh = item.PriceSeriesItem?.PriceHigh,
                       PriceMid = item.PriceSeriesItem?.PriceMid,
                       PriceDelta = isNonMarketAdjustment ? item.PriceSeriesItem?.AdjustedPriceDelta : item.PriceSeriesItem?.PriceDelta,
                       PriceLowDelta = isNonMarketAdjustment ? item.PriceSeriesItem?.AdjustedPriceLowDelta : item.PriceSeriesItem?.PriceLowDelta,
                       PriceMidDelta = isNonMarketAdjustment ? item.PriceSeriesItem?.AdjustedPriceMidDelta : item.PriceSeriesItem?.PriceMidDelta,
                       PriceHighDelta = isNonMarketAdjustment ? item.PriceSeriesItem?.AdjustedPriceHighDelta : item.PriceSeriesItem?.PriceHighDelta,
                       Period = item.PriceSeriesItem?.PeriodLabel,
                       Status = item.PriceSeriesItem?.Status,
                       AssessedDateTime = item.PriceSeriesItem?.AssessedDateTime.ToString("dd MMM yyyy", DateTimeFormatInfo.InvariantInfo) ?? null,
                       LastModifiedDate = item.PriceSeriesItem?.LastModified.Timestamp.ToString("ddd dd MMM yyyy", DateTimeFormatInfo.InvariantInfo),
                       SeriesItemTypeCode = item.SeriesItemTypeCode,
                       PreviousVersion = item.PriceSeriesItem?.PreviousVersions?.OrderByDescending(x => x.LastModified.Timestamp).FirstOrDefault()
                   };
               }) ?? [];
        }
    }
}
