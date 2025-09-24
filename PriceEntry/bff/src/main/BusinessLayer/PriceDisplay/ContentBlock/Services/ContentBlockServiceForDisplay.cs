namespace BusinessLayer.PriceDisplay.ContentBlock.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using BusinessLayer.PriceDisplay.ContentBlock.DTOs.Input;
    using BusinessLayer.PriceDisplay.ContentBlock.DTOs.Output;
    using BusinessLayer.PriceDisplay.ContentBlock.Repositories;
    using BusinessLayer.PriceDisplay.ContentBlock.Repositories.Models;
    using BusinessLayer.PriceDisplay.PriceSeries.Services;

    using Infrastructure.Services.AuditInfoService;

    using Serilog;

    using Version = BusinessLayer.ContentBlock.ValueObjects.Version;

    public class ContentBlockServiceForDisplay : IContentBlockServiceForDisplay
    {
        private readonly ContentBlockRepositoryForDisplay contentBlockRepository;
        private readonly IAuditInfoService auditInfoService;
        private readonly ILogger logger;
        private readonly IPriceSeriesService priceSeriesService;

        public ContentBlockServiceForDisplay(
            ContentBlockRepositoryForDisplay contentBlockRepository,
            IAuditInfoService auditInfoService,
            ILogger logger,
            IPriceSeriesService priceSeriesService)
        {
            this.contentBlockRepository = contentBlockRepository;
            this.auditInfoService = auditInfoService;
            this.logger = logger.ForContext<ContentBlockServiceForDisplay>();
            this.priceSeriesService = priceSeriesService;
        }

        public async Task<ContentBlockDefinitionForDisplay?> GetContentBlock(string contentBlockId, Version version)
        {
            var localLogger = logger.ForContext("ContentBlockId", contentBlockId).ForContext("Version", version);

            ContentBlockForDisplay? contentBlock;

            if (version == Version.Latest)
            {
                localLogger.ForContext("Scenario", "LatestContentBlock")
                    .Debug($"Fetching contentBlockForPriceDisplay for contentBlockId {contentBlockId}");

                contentBlock = await contentBlockRepository.GetLatestContentBlock(contentBlockId);
            }
            else
            {
                localLogger.ForContext("Scenario", "ContentBlockByIdAndVersion")
                    .Debug($"Fetching contentBlockForPriceDisplay for contentBlockId {contentBlockId} and Version {version}");

                contentBlock = await contentBlockRepository.GetContentBlockByVersion(contentBlockId, version);
            }

            if (contentBlock == null)
            {
                localLogger.ForContext("Scenario", "NotFound").Warning($"contentBlockForPriceDisplay is not found for contentBlockId {contentBlockId}");

                return null;
            }

            return new ContentBlockDefinitionForDisplay
            {
                ContentBlockId = contentBlock.ContentBlockId,
                Version = contentBlock.Version,
                Title = contentBlock.Title,
                Columns = contentBlock.Columns,
                Rows = contentBlock.Rows,
                SelectedFilters = contentBlock.SelectedFilters
            };
        }

        public async Task<ContentBlockDefinitionForDisplay> GetContentBlockFromInputParameters(
            List<string> seriesIds,
            DateTime assessedDateTime)
        {
            var priceSeriesList = await priceSeriesService.GetPriceSeriesDetails(seriesIds);

            return new ContentBlockDefinitionForDisplay()
            {
                ContentBlockId = Guid.NewGuid().ToString(),
                Version = 1,
                Rows = priceSeriesList.Select((priceSeries, index) => new RowForDisplay
                {
                    PriceSeriesId = priceSeries.Id,
                    DisplayOrder = index + 1,
                    SeriesItemTypeCode = priceSeries.SeriesItemTypeCode
                }).ToList(),
                AssessedDateTime = assessedDateTime
            };
        }

        public async Task<ContentBlockSaveResponseForDisplay> SaveContentBlock(ContentBlockForDisplayInput contentBlockInput)
        {
            var localLogger = logger.ForContext("ContentBlockId", contentBlockInput.ContentBlockId);

            var latestVersion =
                await contentBlockRepository.GetLatestVersionForContentBlock(contentBlockInput.ContentBlockId);

            localLogger.Debug($"Latest version of contentBlock is {latestVersion}");

            var contentBlock = new ContentBlockForDisplay
            {
                Id = Guid.NewGuid().ToString(),
                ContentBlockId = contentBlockInput.ContentBlockId,
                Version = latestVersion.Increment().Value,
                Title = contentBlockInput.Title,
                Columns = contentBlockInput.Columns?.Select(c => new ColumnForDisplay()
                {
                    Field = c.Field,
                    DisplayOrder = c.DisplayOrder,
                    Hidden = c.Hidden
                }).ToList(),
                Rows = contentBlockInput.Rows?.Select(r => new RowForDisplay()
                {
                    PriceSeriesId = r.PriceSeriesId,
                    DisplayOrder = r.DisplayOrder,
                    SeriesItemTypeCode = r.SeriesItemTypeCode
                }).ToList(),
                SelectedFilters = contentBlockInput.SelectedFilters == null ? null : new SelectedFilters()
                {
                    SelectedCommodities = contentBlockInput.SelectedFilters.SelectedCommodities,
                    SelectedAssessedFrequencies = contentBlockInput.SelectedFilters.SelectedAssessedFrequencies,
                    SelectedPriceCategories = contentBlockInput.SelectedFilters.SelectedPriceCategories,
                    SelectedTransactionTypes = contentBlockInput.SelectedFilters.SelectedTransactionTypes,
                    SelectedRegions = contentBlockInput.SelectedFilters.SelectedRegions,
                    IsInactiveIncluded = contentBlockInput.SelectedFilters.IsInactiveIncluded
                },
                LastModified = auditInfoService.GetAuditInfoForCurrentUser()
            };

            var contentBlockSaveResponse = new ContentBlockSaveResponseForDisplay
            {
                ContentBlockId = contentBlock.ContentBlockId,
                Version = contentBlock.Version
            };

            localLogger.Debug($"Updated version of contentBlock is {contentBlock.Version}");

            await contentBlockRepository.SaveContentBlock(contentBlock);

            localLogger.Debug($"ContentBlock saved successfully with contentBlockId {contentBlock.ContentBlockId} and version {contentBlock.Version}");

            return contentBlockSaveResponse;
        }
    }
}
