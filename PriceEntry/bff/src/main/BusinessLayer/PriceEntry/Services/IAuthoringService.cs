namespace BusinessLayer.PriceEntry.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using BusinessLayer.PriceEntry.DTOs.Input;
    using BusinessLayer.PriceEntry.DTOs.Output;
    using BusinessLayer.PriceEntry.Repositories.Models;
    using BusinessLayer.PriceEntry.Services.Calculators.Periods;
    using BusinessLayer.PriceEntry.ValueObjects;
    using BusinessLayer.UserPreference.DTOs.Output;

    using Infrastructure.Services.Workflow;

    using GridConfiguration = BusinessLayer.PriceEntry.DTOs.Output.GridConfiguration;
    using PriceSeries = BusinessLayer.PriceEntry.DTOs.Output.PriceSeries;
    using PriceSeriesGrid = BusinessLayer.ContentBlock.DTOs.Output.PriceSeriesGrid;

    public interface IAuthoringService : ISubscriberService
    {
        Task<List<PriceSeriesAggregation>> GetPriceSeriesFromAggregation(
            List<string> priceSeriesIds,
            DateTime assessedDateTime);

        Task<List<PriceSeries>> GetPriceSeriesDetailsForGrid(
            PriceSeriesGrid priceSeriesGrid,
            DateTime assessedDateTime,
            UserPreference? userPreference,
            List<PriceSeriesAggregation> priceSeriesAggregations,
            List<PeriodCalculatorOutputItem> absolutePeriods);

        Task<PriceEntryDataSaveResponse> SavePriceEntryData(PriceItemInput priceItemInput);

        Task<List<GridConfiguration>> GetGridConfigurations(
            IReadOnlyList<SeriesItemTypeCode> seriesItemTypeCode);

        GridConfiguration GetGridConfigurationWithUserPreferences(
            string priceSeriesGridId,
            GridConfiguration gridConfiguration,
            UserPreference? userPreference);

        GridConfiguration GetGridConfigurationWithCorrectionColumns(
            GridConfiguration gridConfiguration,
            WorkflowStatus dataPackageWorkflowStatus);
    }
}
