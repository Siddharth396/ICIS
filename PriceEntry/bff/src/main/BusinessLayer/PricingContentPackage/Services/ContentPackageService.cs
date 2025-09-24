namespace BusinessLayer.PricingContentPackage.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using BusinessLayer.ContentBlock.Services;
    using BusinessLayer.DataPackage.Models;
    using BusinessLayer.DataPackage.Repositories.Models;
    using BusinessLayer.Helpers;
    using BusinessLayer.PriceSeriesSelection.Repositories;
    using BusinessLayer.PriceSeriesSelection.Repositories.Models;
    using BusinessLayer.PricingContentPackage.Repositories;
    using BusinessLayer.PricingContentPackage.Repositories.Models;

    using Infrastructure.Services.CanvasApi;
    using Infrastructure.Services.Workflow;

    using Microsoft.Extensions.Internal;

    using Version = BusinessLayer.ContentBlock.ValueObjects.Version;

    public class ContentPackageService : IContentPackageService
    {
        private readonly ICanvasApiService canvasApiService;

        private readonly ISystemClock clock;

        private readonly IContentBlockDomainService contentBlockDomainService;

        private readonly ContentPackageRepository contentPackageRepository;

        private readonly PriceSeriesRepository priceSeriesRepository;

        public ContentPackageService(
            PriceSeriesRepository priceSeriesRepository,
            ContentPackageRepository contentPackageRepository,
            ICanvasApiService canvasApiService,
            ISystemClock clock,
            IContentBlockDomainService contentBlockDomainService)
        {
            this.priceSeriesRepository = priceSeriesRepository;
            this.contentPackageRepository = contentPackageRepository;
            this.canvasApiService = canvasApiService;
            this.clock = clock;
            this.contentBlockDomainService = contentBlockDomainService;
        }

        public async Task SaveAndNotifyOnContentPackageUpdates(DataPackage dataPackage, string userId)
        {
            var now = clock.UtcNow;
            var existingContentPackage =
                await contentPackageRepository.GetContentPackage(new DataPackageId(dataPackage.Id));
            var contentPackage = existingContentPackage
                                 ?? await BuildFreshContentPackage(dataPackage, now, userId);

            contentPackage.Status = dataPackage!.Status;
            SetCommentaryVersion(contentPackage, existingContentPackage, dataPackage);
            SetContentPackageVersion(contentPackage, dataPackage!);
            SetModifiedInformation(contentPackage, now, userId);
            SetPublishedInformation(
                contentPackage,
                dataPackage!,
                now,
                userId);

            await contentPackageRepository.Save(contentPackage);

            _ = await canvasApiService.SendContentPackage(contentPackage.MapToContentPackageRequestModel());
        }

        private static void SetCommentaryVersion(ContentPackage contentPackage, ContentPackage? existingContentPackage, DataPackage dataPackage)
        {
            if (existingContentPackage == null)
            {
                // Commentary already handled in BuildFreshContentPackage
                return;
            }

            if (dataPackage.Commentary == null)
            {
                // A commentary is not supposed to be removed once it was added.
                // Its content might be empty, but that is just another version in the history.
                // If it is null here, it means that the commentary was not saved ever.
                return;
            }

            var existingCommentaryBlock =
                contentPackage.Contents.ContentBlocks.FirstOrDefault(x => x.Id == dataPackage.Commentary.CommentaryId);

            if (existingCommentaryBlock == null)
            {
                var buildCommentaryBlock = BuildCommentaryBlock(dataPackage);

                if (buildCommentaryBlock != null)
                {
                    contentPackage.Contents.ContentBlocks.Add(buildCommentaryBlock);
                }
            }
            else
            {
                existingCommentaryBlock.Version = dataPackage.Commentary!.Version;
            }
        }

        private static IEnumerable<Repositories.Models.ContentBlock> BuildContentBlocks(DataPackage dataPackage)
        {
            var priceEntryContentBlock = new Repositories.Models.ContentBlock
            {
                Version = dataPackage.ContentBlock.Version.ToString(),
                Id = dataPackage.ContentBlock.Id,
                Name = "price-entry-capability",
                Config =
                [
                    new Config { Key = "isReviewMode", Value = "true" },
                    new Config
                    {
                        Key = "assessedDateTime", Value = dataPackage.AssessedDateTime.ToUnixTimeMilliseconds().ToString()
                    }
                ],
                Tags =
                [
                    new Tag { TagId = "price_entry", Type = "price_entry_type", Category = null },
                    GetDatasetTypePricingTag()
                ],
                SequenceId = $"{dataPackage.SequenceId}-prcent",
            };

            yield return priceEntryContentBlock;

            var commentaryBlock = BuildCommentaryBlock(dataPackage);

            if (commentaryBlock != null)
            {
                yield return commentaryBlock;
            }
        }

        private static Repositories.Models.ContentBlock? BuildCommentaryBlock(DataPackage dataPackage)
        {
            if (dataPackage.Commentary == null)
            {
                return null;
            }

            var commentaryBlock = new Repositories.Models.ContentBlock
            {
                Version = dataPackage.Commentary!.Version,
                Id = dataPackage.Commentary.CommentaryId,
                Name = "richtext-capability",
                Config = [],
                Tags =
                [
                    new Tag { TagId = "price-commentary", Type = "commentary_type", Category = null }
                ],
                SequenceId = $"{dataPackage.SequenceId}-rchtxt",
            };
            return commentaryBlock;
        }

        private static Contents BuildContents(DataPackage dataPackage)
        {
            return new Contents { ContentBlocks = BuildContentBlocks(dataPackage).ToList() };
        }

        private static Config[] BuildMetadata(List<PriceSeries> series)
        {
            var uniqueKeyValuePairs = new Dictionary<string, string>();

            for (var index = 0; index < series.Count; index++)
            {
                var s = series[index];
                uniqueKeyValuePairs.TryAdd(s.Commodity.Name.English!, "commodity_name");
                uniqueKeyValuePairs.TryAdd(s.Location.Name.English!, "location_name");
                uniqueKeyValuePairs.TryAdd(s.Location.Region.Name.English!, "location_name");
                uniqueKeyValuePairs.TryAdd(s.ItemFrequency.Name.English!, "frequency_name");
                uniqueKeyValuePairs.TryAdd(s.Id, "price_series_id");

                if (!string.IsNullOrWhiteSpace(s.PriceSettlementType?.Code))
                {
                    uniqueKeyValuePairs.TryAdd(s.PriceSettlementType!.Code, "price_settlement_type_code");
                }

                uniqueKeyValuePairs.TryAdd(s.PriceCategory.Code, "price_category_code");

                if (!string.IsNullOrWhiteSpace(s.TradeTerms?.Code))
                {
                    uniqueKeyValuePairs.TryAdd(s.TradeTerms!.Code, "trade_term_code");
                }
            }

            // Using the keys from the dictionary as values, and the values as keys by intention
            // This ensures that we have unique key-value pairs in the metadata
            return uniqueKeyValuePairs
               .Where(x => !string.IsNullOrWhiteSpace(x.Key))
               .Select(x => new Config { Key = x.Value, Value = x.Key })
               .OrderBy(x => x.Key)
               .ToArray();
        }

        private static Tag[] BuildTags(List<PriceSeries> priceSeries)
        {
            var uniqueTags = new HashSet<(string TagId, string Type, string? Category)>();

            foreach (var series in priceSeries)
            {
                uniqueTags.Add((series.Commodity.Guid.ToString(), "commodity", null));
                uniqueTags.Add((series.Location.Guid.ToString(), "location", null));
                uniqueTags.Add((series.Location.Region.Guid.ToString(), "location", null));
                uniqueTags.Add((series.ItemFrequency.Guid.ToString(), "frequency", null));

                if (!string.IsNullOrWhiteSpace(series.PriceSettlementType?.Code))
                {
                    uniqueTags.Add((series.PriceSettlementType!.Guid.ToString(), "price_settlement_type", null));
                }

                uniqueTags.Add((series.PriceCategory.Guid.ToString(), "price_category", null));

                if (!string.IsNullOrWhiteSpace(series.TradeTerms?.Code))
                {
                    uniqueTags.Add((series.TradeTerms!.Guid.ToString(), "trade_term", null));
                }
            }

            var datasetTypePricingTag = GetDatasetTypePricingTag();
            uniqueTags.Add((datasetTypePricingTag.TagId, datasetTypePricingTag.Type, datasetTypePricingTag.Category));

            return uniqueTags
                .Where(x => !string.IsNullOrWhiteSpace(x.TagId))
                .Select(x => new Tag { TagId = x.TagId, Type = x.Type, Category = x.Category })
                .ToArray();
        }

        private static Tag GetDatasetTypePricingTag()
        {
            return new Tag { TagId = "dataset-type/pricing", Type = "dataset_type", Category = null };
        }

        private static Title BuildTitle(List<PriceSeries> series)
        {
            // All series in a content block should be for the same commodity and region,
            // and we just take the first one for calculating the title
            var firstSeries = series.First();
            return new Title
            {
                Locale = "en",
                Text = $"{firstSeries.Commodity.Name.English} {firstSeries.Location.Region.Name.English}"
            };
        }

        private static void SetContentPackageVersion(ContentPackage contentPackage, DataPackage dataPackage)
        {
            if (WorkflowStatus.IsPublishedOrCorrectionPublishedStatusMatch(dataPackage.Status))
            {
                contentPackage.Version++;
                contentPackage.Revision = 0;
            }
            else
            {
                contentPackage.Revision++;
            }
        }

        private static void SetModifiedInformation(
            ContentPackage contentPackage,
            DateTimeOffset now,
            string userId)
        {
            contentPackage.ModifiedBy = userId;
            contentPackage.ModifiedOn = now.ToUnixTimeMilliseconds();
        }

        private static void SetPublishedInformation(
            ContentPackage contentPackage,
            DataPackage dataPackage,
            DateTimeOffset now,
            string userId)
        {
            if (!WorkflowStatus.IsPublishedOrCorrectionPublishedStatusMatch(dataPackage.Status))
            {
                return;
            }

            contentPackage.PublishedBy = userId;
            contentPackage.PublishedOn = now.ToUnixTimeMilliseconds();

            foreach (var contentBlock in contentPackage.Contents.ContentBlocks)
            {
                contentBlock.LastPublishedDate = now.ToUnixTimeMilliseconds();
            }
        }

        private async Task<ContentPackage> BuildFreshContentPackage(
            DataPackage dataPackage,
            DateTimeOffset now,
            string userId)
        {
            var contentBlock = await contentBlockDomainService.GetContentBlock(
                                   dataPackage.ContentBlock.Id,
                                   Version.From(dataPackage.ContentBlock.Version));

            var series = await priceSeriesRepository.GetPriceSeriesByIds(contentBlock!.GetPriceSeriesIds());

            var contentPackage = new ContentPackage
            {
                ContentPackageId = dataPackage.Id,
                AppliesFrom = dataPackage.AssessedDateTime.ToUnixTimeMilliseconds(),
                Title = BuildTitle(series),
                CreatedBy = userId,
                CreatedOn = now.ToUnixTimeMilliseconds(),
                ModifiedBy = userId,
                ModifiedOn = now.ToUnixTimeMilliseconds(),
                Version = 0,
                Revision = 0,
                Status = string.Empty,
                Metadata = BuildMetadata(series),
                Tags = BuildTags(series),
                Contents = BuildContents(dataPackage),
                SequenceId = dataPackage.SequenceId,
            };

            return contentPackage;
        }
    }
}
