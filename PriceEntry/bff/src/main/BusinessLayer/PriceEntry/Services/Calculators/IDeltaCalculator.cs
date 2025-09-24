namespace BusinessLayer.PriceEntry.Services.Calculators
{
    public interface IDeltaCalculator
    {
        PriceDeltaResult GetPriceDelta(decimal? currentPrice, decimal? previousPrice);
    }
}
