namespace BusinessLayer.PriceEntry.Services
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading.Tasks;

    using BusinessLayer.DataPackage.Models;
    using BusinessLayer.DataPackage.Services;
    using BusinessLayer.PriceEntry.DTOs.Input;
    using BusinessLayer.PriceEntry.Services.Factories;
    using BusinessLayer.PriceEntry.ValueObjects;
    using BusinessLayer.PriceSeriesSelection.Repositories;
    using BusinessLayer.PriceSeriesSelection.Repositories.Models;

    using Infrastructure.Services.CanvasApi;
    using Infrastructure.Services.Workflow;

    using Serilog;
    using Serilog.Context;

    public class DerivedPriceService : IDerivedPriceService
    {
        private readonly PriceSeriesRepository priceSeriesRepository;

        private readonly IAuthoringService priceEntryService;

        private readonly IDerivedPriceCalculatorFactory derivedPriceCalculatorFactory;

        private readonly IDataPackageService dataPackageService;

        private readonly IHalfMonthPriceSeriesItemService halfMonthPriceSeriesItemService;

        private readonly CanvasApiSettings canvasApiSettings;

        private readonly ILogger logger;

        private readonly IDataPackageDomainService dataPackageDomainService;

        private readonly IPriceSeriesItemsDomainService priceSeriesItemsDomainService;

        public DerivedPriceService(
            PriceSeriesRepository priceSeriesRepository,
            IAuthoringService priceEntryService,
            IDerivedPriceCalculatorFactory derivedPriceCalculatorFactory,
            IDataPackageService dataPackageService,
            IHalfMonthPriceSeriesItemService halfMonthPriceSeriesItemService,
            CanvasApiSettings canvasApiSettings,
            ILogger logger,
            IDataPackageDomainService dataPackageDomainService,
            IPriceSeriesItemsDomainService priceSeriesItemsDomainService)
        {
            this.priceSeriesRepository = priceSeriesRepository;
            this.priceEntryService = priceEntryService;
            this.derivedPriceCalculatorFactory = derivedPriceCalculatorFactory;
            this.dataPackageService = dataPackageService;
            this.halfMonthPriceSeriesItemService = halfMonthPriceSeriesItemService;
            this.canvasApiSettings = canvasApiSettings;
            this.logger = logger;
            this.dataPackageDomainService = dataPackageDomainService;
            this.priceSeriesItemsDomainService = priceSeriesItemsDomainService;
        }

        public async Task UpdateDerivedPrices(string seriesId, DateTime assessedDateTime, OperationType operationType)
        {
            using (LogContext.PushProperty("Flow", nameof(UpdateDerivedPrices)))
            using (LogContext.PushProperty("SeriesId", seriesId))
            using (LogContext.PushProperty("assessedDateTime", assessedDateTime))
            {
                var derivedPriceSeries = await priceSeriesRepository.GetDerivedPriceSeries(seriesId);
                var validDerivedPriceSeries = derivedPriceSeries.Where(PriceSeries.IsValidDerivedSeries).ToList();

                foreach (var derivedSeries in validDerivedPriceSeries)
                {
                    if (!await IsDerivedSeriesImpacted(seriesId, derivedSeries, assessedDateTime))
                    {
                        continue;
                    }

                    var derivationFunctionKey =
                        DerivationFunctionKeyFactory.GetDerivationFunctionKey(
                            derivedSeries.DerivationInputs!.First(x => x.SeriesId.Equals(seriesId)).DerivationFunctionKey);

                    var derivedPriceCalculator =
                        derivedPriceCalculatorFactory.GetDerivedPriceCalculator(derivationFunctionKey);

                    var calculatedPrice = await derivedPriceCalculator.CalculatePrice(
                                              derivedSeries,
                                              assessedDateTime,
                                              operationType);

                    await IfCorrectionThenTriggerCorrectionWorkflowForDerivedPriceSeriesItemDataPackages(
                        operationType,
                        derivedSeries,
                        assessedDateTime);

                    await priceEntryService.SavePriceEntryData(
                        new PriceItemInput
                        {
                            AssessedDateTime = assessedDateTime,
                            SeriesId = derivedSeries.Id,
                            SeriesItemTypeCode = derivedSeries.SeriesItemTypeCode,
                            SeriesItem = new SeriesItem { Price = calculatedPrice },
                            OperationType = operationType.Value
                        });
                }
            }
        }

        public async Task<bool> IsDerivedSeriesImpacted(string seriesId, PriceSeries derivedPriceSeries, DateTime assessedDateTime)
        {
            var derivativePriceSeriesItemId = await priceSeriesItemsDomainService.GetPriceSeriesItemIdBySeriesId(seriesId, assessedDateTime);

            var derivativePriceSeriesItem = await priceSeriesItemsDomainService.GetPriceSeriesItemBySeriesItemId(derivativePriceSeriesItemId, assessedDateTime);

            var derivationFunctionKey = DerivationFunctionKeyFactory.GetDerivationFunctionKey(derivedPriceSeries.DerivationInputs!.First(x => x.SeriesId.Equals(seriesId)).DerivationFunctionKey);

            // If the derivation function is period, then determine if the updated price actually impacts the derived price
            if (DerivationFunctionKey.PeriodAvgFunctionKey.Equals(derivationFunctionKey))
            {
                return derivativePriceSeriesItem != null
                       && await halfMonthPriceSeriesItemService.IsHalfMonthPriceSeriesItem(derivativePriceSeriesItem, derivedPriceSeries, assessedDateTime);
            }

            // If the derivation function is not period, then it must be regional (IsValidDerivedSeries ensures it must be one of these two) and for this calculation all the inputs always impact the derived price
            return DerivationFunctionKey.RegionalAvgFunctionKey.Equals(derivationFunctionKey);
        }

        [ExcludeFromCodeCoverage(Justification = "Excluding from coverage until LNG is switched to the advanced workflow")]
        private async Task IfCorrectionThenTriggerCorrectionWorkflowForDerivedPriceSeriesItemDataPackages(
            OperationType operationType,
            PriceSeries derivedSeries,
            DateTime assessedDateTime)
        {
            if (OperationType.Correction.Equals(operationType))
            {
                var localLogger = logger.ForContext("OperationType", operationType);

                var priceSeriesItemId = await priceSeriesItemsDomainService.GetPriceSeriesItemIdBySeriesId(derivedSeries.Id, assessedDateTime);
                var dataPackages = await dataPackageDomainService.GetDataPackagesByPriceSeriesItemId(priceSeriesItemId);

                if (dataPackages.Count <= 0)
                {
                    return;
                }

                var publishedOrCorrectionPublishedDataPackages = dataPackages.Where(
                    dataPackage => WorkflowStatus.IsPublishedOrCorrectionPublishedStatusMatch(dataPackage!.Status));

                foreach (var dataPackage in publishedOrCorrectionPublishedDataPackages)
                {
                    localLogger.ForContext("DataPackageId", dataPackage.Id);

                    var reviewUrl = $"{canvasApiSettings.ReviewUrlPath}/{dataPackage.Id}";
                    var response = await dataPackageService.InitiateCorrectionForDataPackage(
                                       dataPackage.GetDataPackageKey(),
                                       ReviewPageUrl.Create(reviewUrl));

                    switch (response)
                    {
                        case DataPackageStatusChangeResult.NotFound:
                            localLogger.Fatal("Data Package {dataPackageId} not found", dataPackage.Id);
                            break;
                        case DataPackageStatusChangeResult.CorrectionFailed:
                            localLogger.Fatal("Failed to initiate correction workflow and data package {dataPackageId} not put into correction", dataPackage.Id);
                            break;
                        case DataPackageStatusChangeResult.Success:
                            localLogger.Information("Correction workflow initiated for data package {dataPackageId}", dataPackage.Id);
                            break;
                        default:
                            localLogger.Error("Unknown response {response} for data package {dataPackageId}", response, dataPackage.Id);
                            break;
                    }
                }
            }
        }
    }
}