namespace BusinessLayer.PriceEntry.Services.SeriesItemTypes
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading.Tasks;

    using BusinessLayer.Helpers;
    using BusinessLayer.PriceEntry.DTOs.Input;
    using BusinessLayer.PriceEntry.Repositories;
    using BusinessLayer.PriceEntry.Repositories.Models;
    using BusinessLayer.PriceEntry.Services.Calculators.Periods;
    using BusinessLayer.PriceEntry.ValueObjects;
    using BusinessLayer.PriceSeriesSelection.Repositories;
    using Infrastructure.Configuration;
    using Infrastructure.Services.AuditInfoService;
    using Infrastructure.Services.Workflow;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Internal;
    using Serilog;

    using PriceDeltaType = BusinessLayer.PriceEntry.ValueObjects.PriceDeltaType;
    using PriceSeriesOutput = BusinessLayer.PriceEntry.DTOs.Output.PriceSeries;

    public abstract class BasePriceItemService<T> : IPriceItemService
        where T : BasePriceItem, new()
    {
        protected readonly ISystemClock Clock;

        protected readonly IPriceSeriesItemsDomainService PriceSeriesItemsDomainService;

        private readonly IAuditInfoService auditInfoService;

        private readonly IAbsolutePeriodDomainService absolutePeriodDomainService;

        private readonly PriceSeriesRepository priceSeriesRepository;

        private readonly PriceDeltaTypeRepository priceDeltaTypeRepository;

        private readonly ILogger logger;

        private readonly IConfiguration configuration;

        protected BasePriceItemService(
            PriceSeriesRepository priceSeriesRepository,
            PriceDeltaTypeRepository priceDeltaTypeRepository,
            ISystemClock clock,
            IAuditInfoService auditInfoService,
            IAbsolutePeriodDomainService absolutePeriodDomainService,
            IPriceSeriesItemsDomainService priceSeriesItemsDomainService,
            ILogger logger,
            IConfiguration configuration)
        {
            this.priceSeriesRepository = priceSeriesRepository;
            this.priceDeltaTypeRepository = priceDeltaTypeRepository;
            this.auditInfoService = auditInfoService;
            this.absolutePeriodDomainService = absolutePeriodDomainService;
            this.PriceSeriesItemsDomainService = priceSeriesItemsDomainService;
            Clock = clock;
            this.logger = logger.ForContext<BasePriceItemService<T>>();
            this.configuration = configuration;
        }

        public async Task<(bool Valid, List<string> PriceSeriesItemIds)> ValidatePriceSeriesItems(List<string> seriesIds, DateTime assessedDateTime)
        {
            var priceSeriesItems = await PriceSeriesItemsDomainService.GetPriceSeriesItems(seriesIds, assessedDateTime);

            var priceSeriesItemsCurrentOrPendingChanges = priceSeriesItems.Select(x => x.PendingChangesOrOriginal()).ToList();

            if (priceSeriesItemsCurrentOrPendingChanges.Count == 0)
            {
                return (false, []);
            }

            var validPriceSeriesItemIds = priceSeriesItemsCurrentOrPendingChanges
               .Where(item => IsPriceSeriesItemValid((T)item))
               .Select(item => item.Id)
               .ToList();

            var allPriceSeriesItemsAreValid = validPriceSeriesItemIds.Count == priceSeriesItems.Count
                                              && priceSeriesItems.Count == seriesIds.Count;

            return (allPriceSeriesItemsAreValid, validPriceSeriesItemIds);
        }

        public virtual Task<List<PriceSeriesOutput>> ExtendPriceSeries(
            List<PriceSeriesOutput> priceSeries,
            DateTime assessedDateTime,
            List<PeriodCalculatorOutputItem> absolutePeriods)
        {
            return Task.FromResult(priceSeries);
        }

        public virtual Task<bool> AreAllReferencePriceSeriesPublishedOrInSameContentBlock(List<string> priceSeriesIds, DateTime assessedDateTime, List<string> otherGridsPriceSeriesIds)
        {
            return Task.FromResult(true);
        }

        public async Task<BasePriceItem> SavePriceEntryData(PriceItemInput priceItemInput)
        {
            var localLogger = logger.ForContext("SeriesId", priceItemInput.SeriesId)
                   .ForContext("SeriesItemTypeCode", priceItemInput.SeriesItemTypeCode);

            localLogger.Debug($"START : Saving price data for series {priceItemInput.SeriesId} and date {priceItemInput.AssessedDateTime}");

            var assessedDateTime = priceItemInput.AssessedDateTime;
            var currentPriceSeriesItem = await PriceSeriesItemsDomainService.GetPriceSeriesItem(priceItemInput.SeriesId, assessedDateTime);

            T priceItem;

            if (OperationType.Correction.Matches(priceItemInput.OperationType))
            {
                // On a Correction, PriceItem stays the same and PendingChanges holds changes
                priceItem = (T)currentPriceSeriesItem!;
                var status = priceItem?.PendingChanges?.Status;
                priceItem!.PendingChanges = await BuildPriceItemForSaving(priceItemInput, (T?)currentPriceSeriesItem);

                await AssignPriceItemValues((T?)priceItem.PendingChanges, priceItemInput, assessedDateTime, (T?)currentPriceSeriesItem);
                priceItem.PendingChanges.Status = status ?? WorkflowStatus.CorrectionDraft.Value;

                localLogger.Debug($"Last Modified for Price Series Item {priceItem.Id} Pending Change Node is {priceItem.PendingChanges.LastModified}");
                localLogger.Debug($"Status for Price Series Item {priceItem.Id} Pending Change Node is {priceItem.PendingChanges.Status}");
            }
            else
            {
                priceItem = await BuildPriceItemForSaving(priceItemInput, (T?)currentPriceSeriesItem);

                await AssignPriceItemValues(priceItem, priceItemInput, assessedDateTime, (T?)currentPriceSeriesItem);
                priceItem.Status = currentPriceSeriesItem?.Status ?? WorkflowStatus.Draft.Value;
            }

            priceItem.DlhRecordSource = configuration.GetDlhRecordSource();
            var savedPriceItem = await PriceSeriesItemsDomainService.SavePriceSeriesItem(priceItem);

            localLogger.Debug($"END : Price data saved successfully with seriesItemId {priceItem.Id}");

            return savedPriceItem;
        }

        public async Task DeltaCorrectionForNextDate(List<string> priceSeriesItemIds)
        {
            var localLogger = logger.ForContext("Scenario", "Delta correction triggered");

            var priceSeriesItems = await PriceSeriesItemsDomainService.GetPriceSeriesItemsBySeriesItemIds(priceSeriesItemIds);

            foreach (var priceSeriesItem in priceSeriesItems)
            {
                localLogger.Debug("Start fetching next price series item for: {priceSeriesItem.Id}", priceSeriesItem.Id);

                var nextPriceSeriesItem = await GetNextPriceSeriesItem(priceSeriesItem);
                if (nextPriceSeriesItem != null)
                {
                    localLogger.Debug($"Price series item found for next date with price series item id: {nextPriceSeriesItem.Id} and applies from :{nextPriceSeriesItem.AssessedDateTime}");

                    var item = DeltaCalculationForNextDate((T)priceSeriesItem, (T)nextPriceSeriesItem);
                    var auditInfoForCurrentUser = auditInfoService.GetAuditInfoForCurrentUser();
                    item.LastModified = auditInfoForCurrentUser;

                    await PriceSeriesItemsDomainService.SavePriceSeriesItem(item);
                }
                else
                {
                    localLogger.Debug($"Price series item not found for next date with price series item id: {priceSeriesItem.Id} and applies from :{priceSeriesItem.AssessedDateTime}");
                }
            }
        }

        [ExcludeFromCodeCoverage(Justification = "melamine and styrene price series do not have price_series_link. uncovered part will be covered" +
    "when LNG moves to advance correction workflow")]
        public async Task<BasePriceItem?> GetNextPriceSeriesItem(BasePriceItem priceSeriesItem)
        {
            var localLogger = logger.ForContext("Scenario: GetNextPriceSeriesItem : ", priceSeriesItem.Id);
            localLogger.Debug("Get next price series item for: {priceSeriesItem.Id}", priceSeriesItem.Id);

            DateTime? fulfilmentFrom = null;
            DateTime? fulfilmentUntil = null;

            var priceSeries = await priceSeriesRepository.GetPriceSeriesById(priceSeriesItem.SeriesId);
            var hasRelDefPerspective = priceSeries?.HasRelativeDefPerspective();

            var seriesIds = new List<string> { priceSeriesItem.SeriesId };

            if (hasRelDefPerspective.HasValue && !hasRelDefPerspective.Value)
            {
                var subjectPriceSeries = await priceSeriesRepository.GetSubjectPriceSeriesFromObjectPriceSeriesId(
                 priceSeriesItem.SeriesId,
                 PriceSeriesLinkReasonCode.HasSubsequentAssessmentForSameFulfilmentPeriod);

                if (subjectPriceSeries != null)
                {
                    seriesIds.Add(subjectPriceSeries.Id);
                }

                (fulfilmentFrom, fulfilmentUntil) = await PriceSeriesItemsDomainService.GetFulfilmentDates(priceSeriesItem.SeriesId, priceSeriesItem.AssessedDateTime, priceSeriesItem);
            }

            localLogger.Debug("SeriesIds passed to get next price series item: {seriesIds}", string.Join(",", seriesIds));

            var nextPriceSeriesItem = await PriceSeriesItemsDomainService.GetNextPriceSeriesItem(
                                          seriesIds,
                                          priceSeriesItem.AssessedDateTime,
                                          fulfilmentFrom,
                                          fulfilmentUntil);

            return nextPriceSeriesItem;
        }

        protected abstract Task<T> BuildPriceItemForSaving(PriceItemInput priceItemInput, T? current);

        protected abstract bool IsPriceSeriesItemValid(T item);

        protected abstract T DeltaCalculationForNextDate(T priceSeriesItem, T nextPriceSeriesItem);

        protected abstract PriceDeltaType GetPriceDeltaType(T item);

        private async Task AssignPriceItemValues(T? priceItem, PriceItemInput priceItemInput, DateTime assessedDateTime, T? currentPriceSeriesItem)
        {
            // TODO: We are retrieving priceSeries multiple times within different methods,
            // TODO: this should only be called once and cascaded down, so some refactoring will be needed
            var priceSeries = await priceSeriesRepository.GetPriceSeriesById(priceItemInput.SeriesId);

            var absolutePeriod = await absolutePeriodDomainService.GetAbsolutePeriod(priceSeries, assessedDateTime);

            var auditInfoForCurrentUser = auditInfoService.GetAuditInfoForCurrentUser();
            var isFirstTimeItemIsSaved = currentPriceSeriesItem == null;

            priceItem.Created = isFirstTimeItemIsSaved ? auditInfoForCurrentUser : currentPriceSeriesItem!.Created;
            priceItem.Id = currentPriceSeriesItem?.Id ?? Guid.NewGuid().ToString();
            priceItem.SeriesItemId = GetSeriesItemId(currentPriceSeriesItem, priceItemInput.OperationType);
            priceItem.PreviousVersions = currentPriceSeriesItem?.PreviousVersions ?? new List<BasePriceItem>();
            priceItem.SeriesId = priceItemInput.SeriesId;
            priceItem.AssessedDateTime = assessedDateTime;
            priceItem.PeriodLabel = GetPeriodLabel(absolutePeriod, currentPriceSeriesItem);

            if (PeriodLabelTypeCode.RelativeFulfilmentTime.Matches(priceSeries.PeriodLabelTypeCode))
            {
                priceItem.AppliesFromDateTime = assessedDateTime;
                priceItem.FulfilmentFromDate = currentPriceSeriesItem?.FulfilmentFromDate ?? UtcDateTime.GetUtcDateTime(absolutePeriod?.FromDate);
                priceItem.FulfilmentUntilDate = currentPriceSeriesItem?.FulfilmentUntilDate ?? UtcDateTime.GetUtcDateTime(absolutePeriod?.UntilDate);
            }

            if (PeriodLabelTypeCode.ReferenceTime.Matches(priceSeries.PeriodLabelTypeCode))
            {
                priceItem.AppliesFromDateTime = currentPriceSeriesItem?.AppliesFromDateTime ?? UtcDateTime.GetUtcDateTime(absolutePeriod?.FromDate);
                priceItem.AppliesUntilDateTime = currentPriceSeriesItem?.AppliesUntilDateTime ?? UtcDateTime.GetUtcDateTime(absolutePeriod?.UntilDate);
            }

            // AppliesFromDateTime should always have a value. So if it is null we will set it to assessedDateTime.
            // This can be null in case of plt-none or unsupported ref period
            priceItem.AppliesFromDateTime ??= assessedDateTime;

            priceItem.LastModified = auditInfoForCurrentUser;
            priceItem.PriceDeltaTypeGuid = await GetPriceDeltaTypeGuid(priceItem);
            priceItem.ReleaseDate = null; // Not currently used - will be populated when publishing schedules available
        }

        private async Task<string> GetPriceDeltaTypeGuid(T item)
        {
            var type = GetPriceDeltaType(item);
            var priceDeltaType = await priceDeltaTypeRepository.GetPriceDeltaType(type);
            return priceDeltaType.Guid;
        }

        private string GetSeriesItemId(T? currentPriceSeriesItem, string type)
        {
            if (OperationType.Correction.Matches(type))
            {
                return Guid.NewGuid().ToString();
            }

            return currentPriceSeriesItem?.SeriesItemId ?? Guid.NewGuid().ToString();
        }

        private string? GetPeriodLabel(
            PeriodCalculatorOutputItem? periodCalculatorOutputItem,
            BasePriceItem? currentPriceSeriesItem)
        {
            return currentPriceSeriesItem?.PeriodLabel != null ? currentPriceSeriesItem.PeriodLabel : periodCalculatorOutputItem?.Label;
        }
    }
}