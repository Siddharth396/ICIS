namespace BusinessLayer.PricingContentPackage.Services
{
    using System.Collections.Generic;
    using System.Linq;

    using Infrastructure.Services.CanvasApi.Models;

    using ContentPackage = BusinessLayer.PricingContentPackage.Repositories.Models.ContentPackage;

    public static class ContentPackageMapper
    {
        public static ContentPackageRequestModel MapToContentPackageRequestModel(this ContentPackage contentPackage)
        {
            return new ContentPackageRequestModel { ContentPackage = MapToCanvasApiModel(contentPackage) };
        }

        private static IEnumerable<Config> MapConfigs(Repositories.Models.Config[] configs)
        {
            // a for loop is usually more efficient than a foreach loop or LINQ
            for (var index = 0; index < configs.Length; index++)
            {
                var metadata = configs[index];
                yield return new Config { Key = metadata.Key, Value = metadata.Value };
            }
        }

        private static ContentBlock MapContentBlock(Repositories.Models.ContentBlock contentBlock)
        {
            return new ContentBlock
            {
                Id = contentBlock.Id,
                SequenceId = contentBlock.SequenceId,
                Name = contentBlock.Name,
                Version = contentBlock.Version,
                DisplayMode = contentBlock.DisplayMode,
                IsValid = contentBlock.IsValid,
                LastPublishedDate = contentBlock.LastPublishedDate,
                Tags = MapTags(contentBlock.Tags).ToArray(),
                Config = MapConfigs(contentBlock.Config).ToArray(),
                CapabilityConfig = new CapabilityConfig { Value = contentBlock.CapabilityConfig?.Value }
            };
        }

        private static IEnumerable<ContentBlock> MapContentBlocks(List<Repositories.Models.ContentBlock> contentBlocks)
        {
            // a for loop is usually more efficient than a foreach loop or LINQ
            for (var index = 0; index < contentBlocks.Count; index++)
            {
                yield return MapContentBlock(contentBlocks[index]);
            }
        }

        private static Contents MapContents(ContentPackage contentPackage)
        {
            return new Contents { ContentBlocks = MapContentBlocks(contentPackage.Contents.ContentBlocks).ToArray() };
        }

        private static IEnumerable<Tag> MapTags(Repositories.Models.Tag[] tags)
        {
            // a for loop is usually more efficient than a foreach loop or LINQ
            for (var index = 0; index < tags.Length; index++)
            {
                var tag = tags[index];
                yield return new Tag { TagId = tag.TagId, Type = tag.Type, Category = tag.Category };
            }
        }

        private static Title MapTitle(ContentPackage contentPackage)
        {
            return new Title { Text = contentPackage.Title.Text, Locale = contentPackage.Title.Locale };
        }

        private static Infrastructure.Services.CanvasApi.Models.ContentPackage MapToCanvasApiModel(
            ContentPackage contentPackage)
        {
            return new Infrastructure.Services.CanvasApi.Models.ContentPackage
            {
                ContentPackageId = contentPackage.ContentPackageId,
                AppliesFrom = contentPackage.AppliesFrom,
                CreatedBy = contentPackage.CreatedBy,
                CreatedOn = contentPackage.CreatedOn,
                ModifiedBy = contentPackage.ModifiedBy,
                ModifiedOn = contentPackage.ModifiedOn,
                Revision = contentPackage.Revision,
                Version = contentPackage.Version,
                Status = contentPackage.Status,
                PublishedBy = contentPackage.PublishedBy,
                PublishedOn = contentPackage.PublishedOn,
                Title = MapTitle(contentPackage),
                Tags = MapTags(contentPackage.Tags).ToArray(),
                Metadata = MapConfigs(contentPackage.Metadata).ToArray(),
                Contents = MapContents(contentPackage),
                SequenceId = contentPackage.SequenceId
            };
        }
    }
}
