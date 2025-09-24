namespace BusinessLayer.PriceEntry.Services.Factories
{
    using BusinessLayer.PriceEntry.Services.SeriesItemTypes;
    using BusinessLayer.PriceEntry.ValueObjects;

    public interface ISeriesItemTypeServiceFactory
    {
        IPriceItemService GetPriceItemService(SeriesItemTypeCode type);
    }
}
