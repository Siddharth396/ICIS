namespace BusinessLayer.PriceEntry.Services.Factories
{
    using BusinessLayer.PriceEntry.Services.Calculators.DerivedPrice;
    using BusinessLayer.PriceEntry.ValueObjects;

    public interface IDerivedPriceCalculatorFactory
    {
        IDerivedPriceCalculator GetDerivedPriceCalculator(DerivationFunctionKey derivationFunctionKey);
    }
}
