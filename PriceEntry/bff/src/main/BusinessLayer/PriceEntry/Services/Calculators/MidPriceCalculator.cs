namespace BusinessLayer.PriceEntry.Services.Calculators
{
    public class MidPriceCalculator : IMidPriceCalculator
    {
        public decimal? CalculateMidPrice(decimal? left, decimal? right)
        {
            if (left.HasValue && right.HasValue)
            {
                return (left + right) / 2;
            }

            return null;
        }
    }
}