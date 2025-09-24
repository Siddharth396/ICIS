namespace BusinessLayer.PriceEntry.Services.Factories
{
    using System;

    using BusinessLayer.PriceEntry.Services.Calculators.DerivedPrice;
    using BusinessLayer.PriceEntry.ValueObjects;

    using Microsoft.Extensions.DependencyInjection;

    public class DerivedPriceCalculatorFactory : IDerivedPriceCalculatorFactory
    {
        private readonly IServiceProvider serviceProvider;

        public DerivedPriceCalculatorFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public IDerivedPriceCalculator GetDerivedPriceCalculator(DerivationFunctionKey derivationFunctionKey)
        {
            return derivationFunctionKey.Value switch
            {
                DerivationFunctionKey.RegionalAvg => serviceProvider.GetRequiredService<RegionalAvgDerivedPriceCalculator>(),
                DerivationFunctionKey.PeriodAvg => serviceProvider
                   .GetRequiredService<PeriodAvgDerivedPriceCalculator>(),
                _ => throw new ArgumentException($"Unknown type {derivationFunctionKey.Value}", nameof(derivationFunctionKey))
            };
        }
    }
}
