namespace BusinessLayer.PriceEntry.Services.Factories
{
    using System;

    using BusinessLayer.PriceEntry.Services.SeriesItemTypes;
    using BusinessLayer.PriceEntry.Services.SeriesItemTypes.CharterRateSingleValue;
    using BusinessLayer.PriceEntry.Services.SeriesItemTypes.Range;
    using BusinessLayer.PriceEntry.Services.SeriesItemTypes.SingleValue;
    using BusinessLayer.PriceEntry.Services.SeriesItemTypes.SingleValueWithReference;
    using BusinessLayer.PriceEntry.ValueObjects;

    using Microsoft.Extensions.DependencyInjection;

    public class SeriesItemTypeServiceFactory : ISeriesItemTypeServiceFactory
    {
        private readonly IServiceProvider serviceProvider;

        public SeriesItemTypeServiceFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public IPriceItemService GetPriceItemService(SeriesItemTypeCode type)
        {
            return type.Value switch
            {
                SeriesItemTypeCode.Range => serviceProvider.GetRequiredService<RangePriceService>(),
                SeriesItemTypeCode.SingleValueWithReference => serviceProvider
                   .GetRequiredService<SingleValueWithReferencePriceService>(),
                SeriesItemTypeCode.SingleValue => serviceProvider.GetRequiredService<SingleValuePriceService>(),
                SeriesItemTypeCode.CharterRateSingleValue => serviceProvider.GetRequiredService<CharterRateSingleValuePriceService>(),
                _ => throw new ArgumentException($"Unknown type {type.Value}", nameof(type))
            };
        }
    }
}
