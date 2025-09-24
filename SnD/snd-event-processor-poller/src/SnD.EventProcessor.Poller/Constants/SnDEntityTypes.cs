namespace SnD.EventProcessor.Poller.Constants
{
    public class SnDEntityTypes
    {
        public static string[] EntityTypes = new string[]
        {
            "snd-region",
            "snd-country",
            "snd-state",
            "snd-site",
            "snd-technology",
            "snd-route",
            "snd-licensor",
            "snd-product",
            "snd-company",
            "snd-physical-plant",
            "snd-plant-output",
            "snd-plant-installed-capacity",
            "snd-plant-ownership",
            "snd-plant-status",
            "snd-company-ownership",
            "snd-plant-allocation"
        };
        public static string[] EntityTypesToMigrateToKP = new string[]
        {
            "snd-production",
            "snd-consumption-factor",
            "snd-consumption-value",
            "snd-plant-assessed-capacity",
            "snd-country-trade-summary",
            "snd-country-trade-data",
            "snd-regional-trade"
        };
    }
}
