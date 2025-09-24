namespace BusinessLayer.PriceSeriesSelection.Validators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using BusinessLayer.PriceEntry.Services.Factories;
    using BusinessLayer.PriceSeriesSelection.Repositories.Models;

    using PriceSeriesGrid = BusinessLayer.ContentBlock.Repositories.Models.PriceSeriesGrid;

    using PriceSeriesGridInput = BusinessLayer.ContentBlock.DTOs.Input.PriceSeriesGrid;

    public class PriceSeriesGridsValidator
    {
        private readonly IList<PriceSeries> priceSeries;

        private readonly List<PriceSeriesGridInput> priceSeriesGrids;

        public PriceSeriesGridsValidator(
            IList<PriceSeries> priceSeries,
            List<PriceSeriesGridInput> priceSeriesGrids)
        {
            this.priceSeries = priceSeries;
            this.priceSeriesGrids = priceSeriesGrids;
        }

        public PriceSeriesGridsValidationResult Validate()
        {
            var priceSeriesGridsValidationResult = new PriceSeriesGridsValidationResult();

            var priceSeriesIds = priceSeriesGrids
                               .Where(x => x.PriceSeriesIds != null)
                               .SelectMany(x => x.PriceSeriesIds!).ToList();

            var result = ValidatePriceSeries(priceSeriesIds);

            if (result.Success)
            {
                foreach (var priceSeriesGridInput in priceSeriesGrids)
                {
                    var validationResult = ValidatePriceSeriesGrid(priceSeriesGridInput.PriceSeriesIds);

                    if (validationResult.IsValid)
                    {
                        var priceSeriesGrid = new PriceSeriesGrid
                        {
                            PriceSeriesGridId = priceSeriesGridInput.Id ?? Guid.NewGuid().ToString(),
                            PriceSeriesIds = priceSeriesGridInput.PriceSeriesIds,
                            Title = priceSeriesGridInput.Title,
                            SeriesItemTypeCode = validationResult.SeriesItemTypeCode?.Value
                        };
                        priceSeriesGridsValidationResult.PriceSeriesGrids.Add(priceSeriesGrid);
                    }
                    else
                    {
                        priceSeriesGridsValidationResult.ErrorCodes.AddRange(validationResult.ErrorCodes);
                    }
                }
            }
            else
            {
                priceSeriesGridsValidationResult.ErrorCodes.AddRange(result.ErrorCodes);
            }

            return priceSeriesGridsValidationResult;
        }

        private (bool Success, List<string> ErrorCodes) ValidatePriceSeries(List<string> priceSeriesIds)
        {
            List<string> errorCodes = new List<string>();

            if (priceSeriesIds.Count == 0)
            {
                return (true, new List<string>());
            }

            var schedulesCount = priceSeries
               .SelectMany(x => x.PublicationSchedules?.Select(y => y.ScheduleId) ?? new List<string> { Guid.Empty.ToString() })
               .Distinct()
               .Count();

            if (schedulesCount > 1)
            {
                errorCodes.Add(ErrorCodes.MultipleSchedules);
            }

            var isDuplicatePriceSeries = priceSeriesIds
                                        .GroupBy(x => x)
                                        .Any(g => g.Count() > 1);

            if (isDuplicatePriceSeries)
            {
                errorCodes.Add(ErrorCodes.DuplicatePriceSeriesId);
            }

            if (priceSeries.Select(x => x.Commodity.Guid).Distinct().Count() > 1)
            {
                errorCodes.Add(ErrorCodes.MultipleCommoditiesInContentBlock);
            }

            return (errorCodes.Count == 0, errorCodes);
        }

        private PriceSeriesGridsValidationResult ValidatePriceSeriesGrid(List<string>? priceSeriesIds)
        {
            var result = new PriceSeriesGridsValidationResult();

            if (priceSeriesIds is null)
            {
                return result;
            }

            var gridPriceSeries = this.priceSeries.Where(x => priceSeriesIds.Contains(x.Id)).ToList();

            var seriesItemTypeCodes = gridPriceSeries.Select(x => x.SeriesItemTypeCode).Distinct().ToList();

            if (seriesItemTypeCodes.Count > 1)
            {
                result.ErrorCodes.Add(ErrorCodes.MultipleSeriesItemTypeCodes);
            }

            if (seriesItemTypeCodes.Count == 1)
            {
                result.SeriesItemTypeCode = SeriesItemTypeCodeFactory.GetSeriesItemTypeCode(seriesItemTypeCodes.First());
            }

            return result;
        }

        private static class ErrorCodes
        {
            public const string MultipleSeriesItemTypeCodes = "MULTIPLE_SERIES_ITEM_TYPE_CODES";
            public const string MultipleSchedules = "MULTIPLE_SCHEDULES";
            public const string DuplicatePriceSeriesId = "DUPLICATE_PRICE_SERIES_ID";
            public const string MultipleCommoditiesInContentBlock = "MULTIPLE_COMMODITIES_IN_CONTENT_BLOCK";
        }
    }
}
