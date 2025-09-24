namespace BusinessLayer.DataPackage.Services
{
    using System.Diagnostics.CodeAnalysis;

    using BusinessLayer.DataPackage.Models;
    using BusinessLayer.DataPackage.Repositories.Models;
    using BusinessLayer.PriceEntry.ValueObjects;

    using Version = BusinessLayer.ContentBlock.ValueObjects.Version;

    public static class DataPackageExtensions
    {
        [ExcludeFromCodeCoverage(Justification = "Excluding from coverage until LNG is switched to the advanced workflow")]
        public static DataPackageKey GetDataPackageKey(this DataPackage dataPackage)
        {
            return new DataPackageKey(
                dataPackage.ContentBlock.Id,
                Version.From(dataPackage.ContentBlock.Version),
                dataPackage.AssessedDateTime);
        }

        public static DataPackageId GetDataPackageId(this DataPackage dataPackage)
        {
            return new DataPackageId(dataPackage.Id);
        }
    }
}
