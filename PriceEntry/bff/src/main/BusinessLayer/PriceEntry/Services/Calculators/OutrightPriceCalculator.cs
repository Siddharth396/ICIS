namespace BusinessLayer.PriceEntry.Services.Calculators
{
    public class OutrightPriceCalculator : IOutrightPriceCalculator
    {
        public decimal? GetOutrightPrice(decimal? price, decimal? offset)
        {
            if (price.HasValue && offset.HasValue)
            {
                return price + offset;
            }

            return null;
        }
    }
}
