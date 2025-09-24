namespace Test.Infrastructure.Stubs
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using global::Infrastructure.Services.CanvasApi;
    using global::Infrastructure.Services.CanvasApi.Models;

    public class CanvasApiServiceStub : ICanvasApiService
    {
        private readonly List<ContentPackageRequestModel> requests = new();

        public IReadOnlyList<ContentPackageRequestModel> Requests => requests;

        public Task<bool> SendContentPackage(ContentPackageRequestModel model)
        {
            requests.Add(model);
            return Task.FromResult(true);
        }

        public void ClearRequests()
        {
            requests.Clear();
        }
    }
}
