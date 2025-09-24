namespace BusinessLayer.Registration
{
    using BusinessLayer.Commentary.Services;
    using BusinessLayer.ContentBlock.Services;
    using BusinessLayer.ContentPackageGroup.Services;
    using BusinessLayer.DataPackage.Handler;
    using BusinessLayer.DataPackage.Services;
    using BusinessLayer.ImpactedPrices.Services;
    using BusinessLayer.PriceEntry.Handler;
    using BusinessLayer.PriceEntry.Services;
    using BusinessLayer.PriceEntry.Services.Calculators;
    using BusinessLayer.PriceEntry.Services.Calculators.DerivedPrice;
    using BusinessLayer.PriceEntry.Services.Calculators.Periods;
    using BusinessLayer.PriceEntry.Services.Factories;
    using BusinessLayer.PriceEntry.Services.SeriesItemTypes.CharterRateSingleValue;
    using BusinessLayer.PriceEntry.Services.SeriesItemTypes.Range;
    using BusinessLayer.PriceEntry.Services.SeriesItemTypes.SingleValue;
    using BusinessLayer.PriceEntry.Services.SeriesItemTypes.SingleValueWithReference;
    using BusinessLayer.PriceSeries.Services;
    using BusinessLayer.PricingContentPackage.Services;
    using BusinessLayer.UserPreference.Services;

    using Infrastructure.EventDispatcher;

    using Microsoft.Extensions.DependencyInjection;

    public static class ServicesRegistration
    {
        public static void RegisterServices(this IServiceCollection service)
        {
            service.AddScoped<IImpactedPricesService, ImpactedPricesService>();
            service.AddScoped<IAuthoringService, PriceEntryService>();
            service.AddScoped<ISubscriberService, PriceEntryService>();
            service.AddScoped<IContentBlockService, ContentBlockService>();
            service.AddScoped<PriceSeriesSelection.Services.IPriceSeriesService, PriceSeriesSelection.Services.PriceSeriesService>();
            service.AddScoped<ICommentaryService, CommentaryService>();
            service.AddScoped<IUserPreferenceService, UserPreferenceService>();
            service.AddScoped<IPriceSeriesItemEditabilityService, PriceSeriesItemEditabilityService>();

            service.AddScoped<RangePriceService>();
            service.AddScoped<SingleValueWithReferencePriceService>();
            service.AddScoped<SingleValuePriceService>();
            service.AddScoped<CharterRateSingleValuePriceService>();

            service.AddScoped<IDerivedPriceService, DerivedPriceService>();

            service.AddScoped<ISeriesItemTypeServiceFactory, SeriesItemTypeServiceFactory>();
            service.AddScoped<IDerivedPriceCalculatorFactory, DerivedPriceCalculatorFactory>();

            service.AddSingleton<IDeltaCalculator, DeltaCalculator>();
            service.AddSingleton<IMidPriceCalculator, MidPriceCalculator>();
            service.AddSingleton<IOutrightPriceCalculator, OutrightPriceCalculator>();
            service.AddTransient<IPeriodCalculator, PeriodCalculator>();
            service.AddScoped<IEventHandler<PriceSeriesItemSavedEvent>, SendPubNubEventHandler>();
            service.AddScoped<IEventHandler<PriceSeriesItemSavedEvent>, SaveDerivedPriceEventHandler>();
            service.AddScoped<IEventHandler<PriceSeriesItemSavedEvent>, UpdateReferencePriceEventHandler>();

            service.AddScoped<IDataPackageService, DataPackageService>();
            service.AddScoped<IDataPackageMetadataDomainService, DataPackageMetadataDomainService>();
            service.AddScoped<IReferencePriceService, ReferencePriceService>();
            service.AddScoped<IDataPackageDomainService, DataPackageDomainService>();
            service.AddScoped<IContentBlockDomainService, ContentBlockDomainService>();
            service.AddScoped<IPriceSeriesItemsDomainService, PriceSeriesItemsDomainService>();
            service.AddScoped<ICommentaryDomainService, CommentaryDomainService>();

            service.AddScoped<PeriodAvgDerivedPriceCalculator>();
            service.AddScoped<RegionalAvgDerivedPriceCalculator>();

            service.AddScoped<IAbsolutePeriodDomainService, AbsolutePeriodDomainService>();
            service.AddScoped<IHalfMonthPriceSeriesItemService, HalfMonthPriceSeriesItemService>();

            service.AddScoped<IEventHandler<DataPackageUpdatedEvent>, SendContentPackageToCanvasEventHandler>();
            service.AddScoped<IEventHandler<DataPackageUpdatedEvent>, UpdatePriceSeriesItemsStatusEventHandler>();
            service.AddScoped<IEventHandler<DataPackageUpdatedEvent>, UpdateCommentaryEventHandler>();
            service.AddScoped<IEventHandler<NonMarketAdjustmentDisabledEvent>, NonMarketAdjustmentDisabledEventHandler>();

            service.AddScoped<IContentPackageService, ContentPackageService>();
            service.AddScoped<IPublicationScheduleService, PublicationScheduleService>();
            service.AddScoped<IContentPackageGroupDomainService, ContentPackageGroupDomainService>();

            service.AddScoped<IPriceSeriesDomainService, PriceSeriesDomainService>();

            service.AddMemoryCache();
        }
    }
}
