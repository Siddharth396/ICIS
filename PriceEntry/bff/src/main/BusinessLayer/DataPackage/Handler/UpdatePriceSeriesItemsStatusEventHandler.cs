namespace BusinessLayer.DataPackage.Handler
{
    using System.Linq;

    using System.Threading.Tasks;

    using BusinessLayer.PriceEntry.Services;
    using BusinessLayer.PriceEntry.Services.Factories;

    using Infrastructure.EventDispatcher;
    using Infrastructure.MongoDB.Models;
    using Infrastructure.Services.Workflow;

    using Microsoft.Extensions.Internal;

    using Serilog;

    public class UpdatePriceSeriesItemsStatusEventHandler : IEventHandler<DataPackageUpdatedEvent>
    {
        private readonly ISystemClock clock;

        private readonly IPriceSeriesItemsDomainService priceSeriesItemsDomainService;

        private readonly ISeriesItemTypeServiceFactory seriesItemTypeServiceFactory;

        private readonly ILogger logger;

        public UpdatePriceSeriesItemsStatusEventHandler(
            IPriceSeriesItemsDomainService priceSeriesItemsDomainService,
            ISeriesItemTypeServiceFactory seriesItemTypeServiceFactory,
            ISystemClock clock,
            ILogger logger)
        {
            this.priceSeriesItemsDomainService = priceSeriesItemsDomainService;
            this.seriesItemTypeServiceFactory = seriesItemTypeServiceFactory;
            this.clock = clock;
            this.logger = logger.ForContext<UpdatePriceSeriesItemsStatusEventHandler>();
        }

        public async Task HandleAsync(DataPackageUpdatedEvent @event)
        {
            var dataPackageStatus = new WorkflowStatus(@event.DataPackage.Status);

            var localLogger = logger.ForContext("DataPackageUpdateEvent Received, Handling PriceSeriesItemStatusUpdates", dataPackageStatus);

            localLogger.Debug($"DataPackageUpdateEvent Received for Status {dataPackageStatus}");

            if (WorkflowStatus.IsCorrectionStatus(dataPackageStatus) || @event.Status == WorkflowStatus.Cancelled)
            {
                foreach (var priceSeriesItemGroup in @event.DataPackage.PriceSeriesItemGroups)
                {
                    localLogger.Debug($"DataPackageUpdateEvent Received for Correction Status {dataPackageStatus}, Fetching PriceSeriesItems");

                    var priceSeriesItems = await priceSeriesItemsDomainService.GetPriceSeriesItemsBySeriesItemIds(priceSeriesItemGroup.PriceSeriesItemIds);
                    var idsOfSeriesItemsWithPendingChanges = priceSeriesItems.Where(x => x.PendingChanges != null).Select(x => x.Id).ToList();

                    if (idsOfSeriesItemsWithPendingChanges.Count == 0)
                    {
                        localLogger.Debug("No price series item found with pending changes.");

                        continue;
                    }

                    if (@event.Status == WorkflowStatus.Cancelled)
                    {
                        localLogger.Debug(
                            "Cancelling a correction, setting the PendingChanges for modified PriceSeriesItems to null");

                        await priceSeriesItemsDomainService.CorrectionCancelled(idsOfSeriesItemsWithPendingChanges);
                    }
                    else if (dataPackageStatus == WorkflowStatus.CorrectionPublished)
                    {
                        localLogger.Debug(
                            "Publishing a correction, overwriting the modified price series items with their respective PendingChanges");

                        await priceSeriesItemsDomainService.CorrectionPublished(
                            idsOfSeriesItemsWithPendingChanges,
                            @event.DataPackage.LastModified);

                        var priceItemService = seriesItemTypeServiceFactory.GetPriceItemService(
                            SeriesItemTypeCodeFactory.GetSeriesItemTypeCode(priceSeriesItemGroup.SeriesItemTypeCode));

                        await priceItemService.DeltaCorrectionForNextDate(idsOfSeriesItemsWithPendingChanges);
                    }
                    else if (WorkflowStatus.IsCorrectionPrePublishStatus(dataPackageStatus))
                    {
                        localLogger.Debug(
                            $"Changing status as part of a correction, changing the PendingChanges status of the modified PriceSeriesItems to {dataPackageStatus}");
                        await priceSeriesItemsDomainService.StatusChangedDuringCorrection(
                            idsOfSeriesItemsWithPendingChanges,
                            dataPackageStatus,
                            @event.DataPackage.LastModified);
                    }
                }
            }
            else
            {
                await priceSeriesItemsDomainService.StatusChanged(
                    @event.DataPackage.GetPriceSeriesItemIds(),
                    @event.Status,
                    new AuditInfo { User = @event.UserId, Timestamp = clock.UtcNow.UtcDateTime });
            }
        }
    }
}