namespace BusinessLayer.PriceEntry.Services.Calculators
{
    using System;

    public class DeltaCalculator : IDeltaCalculator
    {
        public PriceDeltaResult GetPriceDelta(decimal? currentPrice, decimal? previousPrice)
        {
            if (!previousPrice.HasValue || !currentPrice.HasValue)
            {
                return new PriceDeltaResult { PriceDelta = null, PriceDeltaPercentage = null };
            }

            var priceDelta = currentPrice - previousPrice;
            decimal? priceDeltaPercentage;

            if (previousPrice != 0)
            {
                priceDeltaPercentage = priceDelta / Math.Abs(previousPrice.Value) * 100;
            }
            else
            {
                priceDeltaPercentage = priceDelta * 100;
            }

            return new PriceDeltaResult { PriceDelta = priceDelta, PriceDeltaPercentage = priceDeltaPercentage };
        }
    }
}
