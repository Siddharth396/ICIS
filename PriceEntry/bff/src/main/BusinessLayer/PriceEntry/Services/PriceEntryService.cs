namespace BusinessLayer.PriceEntry.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using BusinessLayer.Helpers;
    using BusinessLayer.PriceEntry.DTOs.Input;
    using BusinessLayer.PriceEntry.DTOs.Output;
    using BusinessLayer.PriceEntry.Handler;
    using BusinessLayer.PriceEntry.Repositories;
    using BusinessLayer.PriceEntry.Repositories.Models;
    using BusinessLayer.PriceEntry.Services.Calculators.Periods;
    using BusinessLayer.PriceEntry.Services.Factories;
    using BusinessLayer.PriceEntry.ValueObjects;
    using BusinessLayer.UserPreference.DTOs.Output;

    using global::AutoMapper;

    using Infrastructure.Attributes.BusinessAnnotations;
    using Infrastructure.EventDispatcher;
    using Infrastructure.Services.Workflow;

    using Serilog;
    using Serilog.Context;

    using GridConfiguration = BusinessLayer.PriceEntry.DTOs.Output.GridConfiguration;
    using PriceSeries = BusinessLayer.PriceEntry.DTOs.Output.PriceSeries;
    using PriceSeriesGrid = BusinessLayer.ContentBlock.DTOs.Output.PriceSeriesGrid;

    [ApplicationService]
    public class PriceEntryService : IAuthoringService
    {
        private readonly GridConfigurationRepository gridConfigurationRepository;

        private readonly PriceEntryRepository priceEntryRepository;

        private readonly ISeriesItemTypeServiceFactory seriesItemTypeServiceFactory;

        private readonly ILogger logger;

        private readonly IEventDispatcher eventDispatcher;

        private readonly IMapper mapper;

        private readonly IAbsolutePeriodDomainService absolutePeriodDomainService;

        public PriceEntryService(
            PriceEntryRepository priceEntryRepository,
            GridConfigurationRepository gridConfigurationRepository,
            ISeriesItemTypeServiceFactory seriesItemTypeServiceFactory,
            ILogger logger,
            IEventDispatcher eventDispatcher,
            IMapper mapper,
            IAbsolutePeriodDomainService absolutePeriodDomainService)
        {
            this.logger = logger.ForContext<PriceEntryService>();
            this.priceEntryRepository = priceEntryRepository;
            this.seriesItemTypeServiceFactory = seriesItemTypeServiceFactory;
            this.gridConfigurationRepository = gridConfigurationRepository;
            this.eventDispatcher = eventDispatcher;
            this.mapper = mapper;
            this.absolutePeriodDomainService = absolutePeriodDomainService;
        }

        public async Task<List<GridConfiguration>> GetGridConfigurations(IReadOnlyList<SeriesItemTypeCode> seriesItemTypeCode)
        {
            return await gridConfigurationRepository.GetGridConfigurations(seriesItemTypeCode);
        }

        public GridConfiguration GetGridConfigurationWithUserPreferences(
           string priceSeriesGridId,
           GridConfiguration gridConfiguration,
           UserPreference? userPreference)
        {
            var gridConfigurationCopy = gridConfiguration with { };

            var priceSeriesGridUserPreference = userPreference?.PriceSeriesGrids.SingleOrDefault(x => x.Id == priceSeriesGridId);

            if (priceSeriesGridUserPreference is null)
            {
                return gridConfigurationCopy;
            }

            var userColumnPreferences = priceSeriesGridUserPreference.Columns.ToDictionary(x => x.Field);

            var newColumns = new List<DTOs.Output.Column>();

            foreach (var column in gridConfigurationCopy.Columns)
            {
                if (userColumnPreferences.TryGetValue(column.Field, out var preference))
                {
                    newColumns.Add(column with
                    {
                        Hidden = preference.Hidden,
                        DisplayOrder = preference.DisplayOrder
                    });
                }
                else
                {
                    newColumns.Add(column);
                }
            }

            return gridConfigurationCopy with { Columns = newColumns.OrderBy(c => c.DisplayOrder) };
        }

        public GridConfiguration GetGridConfigurationWithCorrectionColumns(
            GridConfiguration gridConfiguration,
            WorkflowStatus dataPackageWorkflowStatus)
        {
            var gridConfigurationCopy = gridConfiguration with { };

            if (!WorkflowStatus.IsCorrectionPrePublishStatus(dataPackageWorkflowStatus))
            {
                return gridConfigurationCopy;
            }

            logger.Debug("Checking for correction columns since in correction workflow");

            var columns = new List<DTOs.Output.Column>();

            foreach (var column in gridConfigurationCopy.Columns)
            {
                if (column.ForCorrectionWorkflow == true)
                {
                    logger.Debug("Showing additional column {columnName} since in correction workflow", column.Field);
                    columns.Add(column with { Hidden = false });
                }
                else
                {
                    columns.Add(column);
                }
            }

            return gridConfigurationCopy with { Columns = columns };
        }

        public async Task<List<PriceSeriesAggregation>> GetPriceSeriesFromAggregation(
            List<string> priceSeriesIds,
            DateTime assessedDateTime)
        {
            return await priceEntryRepository.GetPriceSeriesDetails(priceSeriesIds, assessedDateTime);
        }

        public async Task<List<PriceSeries>> GetPriceSeriesDetailsForGrid(
            PriceSeriesGrid priceSeriesGrid,
            DateTime assessedDateTime,
            UserPreference? userPreference,
            List<PriceSeriesAggregation> priceSeriesAggregations,
            List<PeriodCalculatorOutputItem> absolutePeriods)
        {
            using (LogContext.PushProperty("AssessedDateTime", assessedDateTime))
            {
                var seriesItemTypeCode = SeriesItemTypeCodeFactory.GetSeriesItemTypeCode(priceSeriesGrid.SeriesItemTypeCode!);
                var priceSeriesService = seriesItemTypeServiceFactory.GetPriceItemService(seriesItemTypeCode);

                foreach (var series in priceSeriesAggregations)
                {
                    series.LastAssessment = GetLastAssessment(series, absolutePeriods, assessedDateTime);
                }

                var priceSeriesOutput = mapper.Map<List<PriceSeries>>(priceSeriesAggregations);
                var extendedPriceSeries = await priceSeriesService.ExtendPriceSeries(
                                              priceSeriesOutput,
                                              assessedDateTime,
                                              absolutePeriods);

                var priceSeriesInContentBlockOrder = extendedPriceSeries
                   .OrderBy(x => priceSeriesGrid.PriceSeriesIds?.IndexOf(x.Id))
                   .ToList();

                if (userPreference is not null)
                {
                    logger.Debug($"Price series order as per user preference");

                    var priceSeriesGridUserPreference = userPreference?.PriceSeriesGrids?.SingleOrDefault(x => x.Id == priceSeriesGrid.Id);

                    return priceSeriesInContentBlockOrder
                       .OrderBy(
                            x =>
                            {
                                var index = priceSeriesGridUserPreference?.PriceSeriesIds.IndexOf(x.Id);
                                return index == -1 ? int.MaxValue : index;
                            })
                       .ToList();
                }

                logger.Debug($"Price series order as per price series grid");

                return priceSeriesInContentBlockOrder;
            }
        }

        public async Task<PriceEntryDataSaveResponse> SavePriceEntryData(PriceItemInput priceItemInput)
        {
            var seriesItemTypeCode = SeriesItemTypeCodeFactory.GetSeriesItemTypeCode(priceItemInput.SeriesItemTypeCode);

            var priceItemService = seriesItemTypeServiceFactory.GetPriceItemService(seriesItemTypeCode);

            var savedPriceItem = await priceItemService.SavePriceEntryData(priceItemInput);

            await eventDispatcher.DispatchAsync(new PriceSeriesItemSavedEvent
            {
                SeriesId = savedPriceItem.SeriesId,
                AssessedDateTime = savedPriceItem.AssessedDateTime,
                OperationType = OperationType.Parse(priceItemInput.OperationType!)
            });

            return new PriceEntryDataSaveResponse { Id = savedPriceItem.Id };
        }

        private static bool ShouldApplyAbsolutePeriodFilter(string? periodCode, bool? hasRelDefPerspective)
        {
            return hasRelDefPerspective.HasValue &&
                   !hasRelDefPerspective.Value &&
                   RelativeFulfilmentPeriodCode.IsAllowed(periodCode);
        }

        private PriceSeriesItem? GetLastAssessment(
            PriceSeriesAggregation series,
            List<PeriodCalculatorOutputItem> absolutePeriods,
            DateTime assessedDateTime)
        {
            DateTime? fulfilmentFrom = null;
            DateTime? fulfilmentUntil = null;

            var hasRelDefPerspective = series.RelativeFulfilmentPeriod?.PeriodType.HasRelativeDefPerspective;

            var absolutePeriod = absolutePeriodDomainService.FilterAbsolutePeriod(absolutePeriods, series, assessedDateTime);

            Func<PriceSeriesItem, bool> filter = (_) => true;

            if (ShouldApplyAbsolutePeriodFilter(absolutePeriod?.PeriodCode, hasRelDefPerspective))
            {
                fulfilmentFrom = UtcDateTime.GetUtcDateTime(absolutePeriod?.FromDate);
                fulfilmentUntil = UtcDateTime.GetUtcDateTime(absolutePeriod?.UntilDate);

                filter = (x) => x.FulfilmentFromDate == fulfilmentFrom &&
                                x.FulfilmentUntilDate == fulfilmentUntil;
            }

            return series.LastAssessments?.OrderByDescending(x => x.AssessedDateTime).FirstOrDefault(filter);
        }
    }
}
