namespace BusinessLayer.PriceEntry.Services.Calculators
{
    public interface IMidPriceCalculator
    {
        decimal? CalculateMidPrice(decimal? left, decimal? right);
    }
}
