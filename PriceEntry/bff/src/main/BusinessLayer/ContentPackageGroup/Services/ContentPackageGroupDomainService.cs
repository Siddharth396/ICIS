namespace BusinessLayer.ContentPackageGroup.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using BusinessLayer.ContentPackageGroup.Repositories;
    using BusinessLayer.ContentPackageGroup.Repositories.Models;
    using BusinessLayer.ContentPackageGroup.ValueObjects;

    using Infrastructure.Attributes.BusinessAnnotations;

    using Version = BusinessLayer.ContentBlock.ValueObjects.Version;

    [DomainService]
    public class ContentPackageGroupDomainService : IContentPackageGroupDomainService
    {
        private readonly ContentPackageGroupRepository contentPackageGroupRepository;

        public ContentPackageGroupDomainService(ContentPackageGroupRepository contentPackageGroupRepository)
        {
            this.contentPackageGroupRepository = contentPackageGroupRepository;
        }

        public async Task SaveContentPackageGroup(string contentBlockId, Version contentBlockVersion, List<string> priceSeriesIds)
        {
            if (priceSeriesIds.Count == 0)
            {
                return;
            }

            var uniqueSeriesIds = new HashSet<string>(priceSeriesIds);
            var sequenceId = new SequenceId(uniqueSeriesIds);
            var contentPackageGroup = await contentPackageGroupRepository.GetBySequenceId(sequenceId);
            if (contentPackageGroup == null)
            {
                contentPackageGroup = new ContentPackageGroup
                {
                    Id = Guid.NewGuid().ToString(),
                    SequenceId = sequenceId.Value,
                    PriceSeriesIds = uniqueSeriesIds.ToList(),
                    ContentPackageGroupContentBlocks =
                    [
                        new ContentPackageGroupContentBlock { ContentBlockId = contentBlockId, Version = contentBlockVersion.Value }
                    ]
                };
            }
            else
            {
                var contentBlockPersisted = contentPackageGroup.ContentPackageGroupContentBlocks.Any(
                    x => x.ContentBlockId == contentBlockId && x.Version == contentBlockVersion.Value);

                if (!contentBlockPersisted)
                {
                    contentPackageGroup.ContentPackageGroupContentBlocks.Add(
                        new ContentPackageGroupContentBlock { ContentBlockId = contentBlockId, Version = contentBlockVersion.Value });
                }
            }

            await contentPackageGroupRepository.UpsertContentPackageGroup(contentPackageGroup);
        }

        public async Task<string> GetSequenceId(string contentBlockId, int version)
        {
            return await contentPackageGroupRepository.GetSequenceId(contentBlockId, version);
        }
    }
}
