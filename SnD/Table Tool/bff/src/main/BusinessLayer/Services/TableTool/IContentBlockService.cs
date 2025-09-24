namespace BusinessLayer.Services.TableTool
{
    using System.Threading.Tasks;

    using BusinessLayer.DTO;
    using BusinessLayer.Services.Models;

    public interface IContentBlockService
    {
        Task<Response<ContentBlockResponse>> GetContentBlockConfiguration(string contentBlockId, string version);
        Task<Response<SaveContentBlockResponse>> SaveContentBlockConfiguration(SaveContentBlockRequest contentBlockInput);
    }
}
