namespace BusinessLayer.ContentBlock.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using BusinessLayer.ContentBlock.Repositories;
    using BusinessLayer.ContentBlock.Repositories.Models;
    using BusinessLayer.ContentBlock.ValueObjects;

    using Infrastructure.Attributes.BusinessAnnotations;

    [DomainService]
    public class ContentBlockDomainService : IContentBlockDomainService
    {
        private readonly ContentBlockRepository contentBlockRepository;

        public ContentBlockDomainService(ContentBlockRepository contentBlockRepository)
        {
            this.contentBlockRepository = contentBlockRepository;
        }

        public async Task<ContentBlock?> GetContentBlock(string contentBlockId, Version version)
        {
            ContentBlock? contentBlock;
            if (version == Version.Latest)
            {
                contentBlock = await contentBlockRepository.GetLatestContentBlock(contentBlockId);
            }
            else
            {
                contentBlock = await contentBlockRepository.GetContentBlockByVersion(contentBlockId, version);
            }

            return contentBlock;
        }

        public Task<List<string>> GetContentBlockIds(string priceSeriesId)
        {
            return contentBlockRepository.GetContentBlockIds(priceSeriesId);
        }

        public async Task<Version> GetLatestVersionFor(string contentBlockId)
        {
            return await contentBlockRepository.GetLatestVersionForContentBlock(contentBlockId);
        }

        public async Task SaveContentBlock(ContentBlock contentBlock)
        {
            await contentBlockRepository.SaveContentBlock(contentBlock);
        }
    }
}
