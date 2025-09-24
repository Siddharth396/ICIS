namespace Infrastructure.Services.CanvasApi
{
    using System.Threading.Tasks;

    using Infrastructure.Services.CanvasApi.Models;

    public interface ICanvasApiService
    {
        Task<bool> SendContentPackage(ContentPackageRequestModel model);
    }
}
