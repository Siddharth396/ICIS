namespace BusinessLayer.DataPackage.Handler
{
    using System.Threading.Tasks;

    using BusinessLayer.ContentBlock.Services;
    using BusinessLayer.PriceEntry.Repositories;
    using BusinessLayer.PriceEntry.Services;

    using Infrastructure.EventDispatcher;
    using Infrastructure.Services.AuditInfoService;

    using PriceDeltaType = BusinessLayer.PriceEntry.ValueObjects.PriceDeltaType;

    public class NonMarketAdjustmentDisabledEventHandler : IEventHandler<NonMarketAdjustmentDisabledEvent>
    {
        private readonly PriceDeltaTypeRepository priceDeltaTypeRepository;

        private readonly IAuditInfoService auditInfoService;

        private readonly IContentBlockDomainService contentBlockDomainService;

        private readonly IPriceSeriesItemsDomainService priceSeriesItemsDomainService;

        public NonMarketAdjustmentDisabledEventHandler(
            PriceDeltaTypeRepository priceDeltaTypeRepository,
            IAuditInfoService auditInfoService,
            IContentBlockDomainService contentBlockDomainService,
            IPriceSeriesItemsDomainService priceSeriesItemsDomainService)
        {
            this.priceDeltaTypeRepository = priceDeltaTypeRepository;
            this.auditInfoService = auditInfoService;
            this.contentBlockDomainService = contentBlockDomainService;
            this.priceSeriesItemsDomainService = priceSeriesItemsDomainService;
        }

        public async Task HandleAsync(NonMarketAdjustmentDisabledEvent @event)
        {
            var contentBlock = await contentBlockDomainService.GetContentBlock(
                                   @event.ContentBlockId,
                                   @event.ContentBlockVersion);

            var priceSeriesIds = contentBlock?.GetPriceSeriesIds() ?? [];
            var auditInfo = auditInfoService.GetAuditInfoForCurrentUser();
            var priceDeltaType = await priceDeltaTypeRepository.GetPriceDeltaType(PriceDeltaType.Regular);
            var priceSeriesItems = await priceSeriesItemsDomainService.GetPriceSeriesItems(priceSeriesIds, @event.AssessedDateTime);

            foreach (var priceSeriesItem in priceSeriesItems)
            {
                priceSeriesItem.CancelNonMarketAdjustment(auditInfo, priceDeltaType.Guid);
                await priceSeriesItemsDomainService.SavePriceSeriesItem(priceSeriesItem);
            }
        }
    }
}
