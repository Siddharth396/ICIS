namespace BusinessLayer.DataPackage.Services
{
    using System.Threading.Tasks;

    using BusinessLayer.DataPackage.Models;
    using BusinessLayer.PriceEntry.ValueObjects;

    public interface IDataPackageMetadataDomainService
    {
        Task<DataPackageId> GetDataPackageId(DataPackageKey dataPackageKey);

        Task ToggleNonMarketAdjustment(DataPackageKey dataPackageKey, bool enabled);

        Task<bool> IsNonMarketAdjustmentEnabled(DataPackageKey dataPackageKey);
    }
}
