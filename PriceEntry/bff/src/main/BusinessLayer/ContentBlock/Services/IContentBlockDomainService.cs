namespace BusinessLayer.ContentBlock.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using BusinessLayer.ContentBlock.Repositories.Models;
    using BusinessLayer.ContentBlock.ValueObjects;

    public interface IContentBlockDomainService
    {
        Task<ContentBlock?> GetContentBlock(string contentBlockId, Version version);

        Task<List<string>> GetContentBlockIds(string priceSeriesId);

        Task<Version> GetLatestVersionFor(string contentBlockId);

        Task SaveContentBlock(ContentBlock contentBlock);
    }
}
