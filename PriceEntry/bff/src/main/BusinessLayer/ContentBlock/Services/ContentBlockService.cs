namespace BusinessLayer.ContentBlock.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using BusinessLayer.Common.Mappers;
    using BusinessLayer.ContentBlock.DTOs.Input;
    using BusinessLayer.ContentBlock.DTOs.Output;
    using BusinessLayer.ContentBlock.Repositories.Models;
    using BusinessLayer.ContentPackageGroup.Services;
    using BusinessLayer.DataPackage.Services;
    using BusinessLayer.PriceSeriesSelection.Services;
    using BusinessLayer.PriceSeriesSelection.Validators;

    using Infrastructure.Attributes.BusinessAnnotations;
    using Infrastructure.Services.AuditInfoService;

    using Serilog;

    using Version = BusinessLayer.ContentBlock.ValueObjects.Version;

    [ApplicationService]
    public class ContentBlockService : IContentBlockService
    {
        private readonly IPriceSeriesService priceSeriesService;

        private readonly IAuditInfoService auditInfoService;

        private readonly IContentBlockDomainService contentBlockDomainService;

        private readonly IDataPackageDomainService dataPackageDomainService;

        private readonly ILogger logger;

        private readonly IModelMapper<ContentBlock, ContentBlockDefinition> contentBlockMapper;

        private readonly IContentPackageGroupDomainService contentPackageGroupDomainService;

        public ContentBlockService(
            IPriceSeriesService priceSeriesService,
            IAuditInfoService auditInfoService,
            IContentBlockDomainService contentBlockDomainService,
            IDataPackageDomainService dataPackageDomainService,
            ILogger logger,
            IModelMapper<ContentBlock, ContentBlockDefinition> contentBlockMapper,
            IContentPackageGroupDomainService contentPackageGroupDomainService)
        {
            this.priceSeriesService = priceSeriesService;
            this.auditInfoService = auditInfoService;
            this.contentBlockDomainService = contentBlockDomainService;
            this.dataPackageDomainService = dataPackageDomainService;
            this.logger = logger.ForContext<ContentBlockService>();
            this.contentBlockMapper = contentBlockMapper;
            this.contentPackageGroupDomainService = contentPackageGroupDomainService;
        }

        public async Task<ContentBlockDefinition?> GetContentBlock(
            string contentBlockId,
            Version version,
            DateTime assessedDateTime,
            bool isReviewMode)
        {
            var versionFromExistingDataPackage =
                await dataPackageDomainService.GetContentBlockVersionFromDataPackage(contentBlockId, assessedDateTime);

            var actualVersion = versionFromExistingDataPackage ?? version;

            var localLogger = logger.ForContext("ContentBlockId", contentBlockId)
               .ForContext("Version", actualVersion);

            var contentBlock = await contentBlockDomainService.GetContentBlock(contentBlockId, actualVersion);

            if (contentBlock == null)
            {
                localLogger.ForContext("Scenario", "NotFound").Warning($"ContentBlock is not found for contentBlockId {contentBlockId}");

                return null;
            }

            localLogger.Debug($"ContentBlock is found for contentBlockId {contentBlockId} and version {contentBlock.Version}");

            return contentBlockMapper.Map(contentBlock);
        }

        public Task<List<string>> GetContentBlockIds(string priceSeriesId)
        {
            return contentBlockDomainService.GetContentBlockIds(priceSeriesId);
        }

        public async Task<ContentBlockSaveResponse> SaveContentBlock(ContentBlockInput contentBlockInput)
        {
            var localLogger = logger.ForContext("ContentBlockId", contentBlockInput.ContentBlockId);

            var latestVersion =
                await contentBlockDomainService.GetLatestVersionFor(contentBlockInput.ContentBlockId);

            localLogger.Debug($"Latest version of contentBlock is {latestVersion}");

            var contentBlock = new ContentBlock
            {
                Id = Guid.NewGuid().ToString(),
                ContentBlockId = contentBlockInput.ContentBlockId,
                Version = latestVersion.Value,
                Title = contentBlockInput.Title,
                LastModified = auditInfoService.GetAuditInfoForCurrentUser(),
            };

            var contentBlockSaveResponse = new ContentBlockSaveResponse
            {
                ContentBlockId = contentBlock.ContentBlockId,
                Version = contentBlock.Version,
                ErrorCodes = new List<string>()
            };

            if (contentBlockInput.PriceSeriesGrids != null)
            {
                var priceSeriesIds = contentBlockInput.PriceSeriesGrids
                                .Where(x => x.PriceSeriesIds != null)
                                .SelectMany(x => x.PriceSeriesIds!).ToList();

                var priceSeries = await priceSeriesService.GetPriceSeriesByIds(priceSeriesIds);

                var priceSeriesGridsValidator = new PriceSeriesGridsValidator(priceSeries, contentBlockInput.PriceSeriesGrids);

                var result = priceSeriesGridsValidator.Validate();

                contentBlockSaveResponse.ErrorCodes = result.ErrorCodes;

                contentBlock.PriceSeriesGrids = result.PriceSeriesGrids;
            }

            if (contentBlockSaveResponse.IsValid)
            {
                var version = latestVersion.Increment();
                contentBlock.Version = version.Value;
                contentBlockSaveResponse.Version = contentBlock.Version;

                localLogger.Debug($"Updated version of contentBlock is {contentBlock.Version}");

                await contentBlockDomainService.SaveContentBlock(contentBlock);
                await contentPackageGroupDomainService.SaveContentPackageGroup(contentBlock.ContentBlockId, version, contentBlock.GetPriceSeriesIds());

                localLogger.Debug($"ContentBlock saved successfully with contentBlockId {contentBlock.ContentBlockId} and version {contentBlock.Version}");
            }
            else
            {
                localLogger.ForContext("Scenario", "PriceSeriesGridsValidation")
                           .ForContext("ErrorCode", contentBlockSaveResponse.ErrorCodes, destructureObjects: true)
                           .Warning("PriceSeriesGrids are not valid");
            }

            return contentBlockSaveResponse;
        }
    }
}