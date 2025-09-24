namespace BusinessLayer.PriceEntry.Services.Factories
{
    using System;
    using System.Collections.Generic;

    using BusinessLayer.PriceEntry.ValueObjects;

    public static class DerivationFunctionKeyFactory
    {
        private static readonly Dictionary<string, DerivationFunctionKey> DerivationFunctionKeys = new()
        {
            { DerivationFunctionKey.RegionalAvg, DerivationFunctionKey.RegionalAvgFunctionKey },
            { DerivationFunctionKey.PeriodAvg, DerivationFunctionKey.PeriodAvgFunctionKey },
        };

        public static DerivationFunctionKey GetDerivationFunctionKey(string derivationFunctionKey)
        {
            if (!DerivationFunctionKeys.ContainsKey(derivationFunctionKey))
            {
                throw new ArgumentException($"Derivation function key {derivationFunctionKey} is not supported");
            }

            return DerivationFunctionKeys[derivationFunctionKey];
        }
    }
}
