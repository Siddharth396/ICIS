namespace BusinessLayer.PriceEntry.Services.Calculators
{
    public interface IOutrightPriceCalculator
    {
        decimal? GetOutrightPrice(decimal? price, decimal? offset);
    }
}
