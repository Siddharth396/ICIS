namespace BusinessLayer.PricingContentPackage.Services
{
    using System.Threading.Tasks;

    using BusinessLayer.DataPackage.Repositories.Models;

    public interface IContentPackageService
    {
        Task SaveAndNotifyOnContentPackageUpdates(DataPackage dataPackageId, string userId);
    }
}
