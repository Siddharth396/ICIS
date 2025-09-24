namespace BusinessLayer.DataPackage.Services
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading.Tasks;

    using BusinessLayer.Commentary.Services;
    using BusinessLayer.ContentPackageGroup.Services;
    using BusinessLayer.DataPackage.Models;
    using BusinessLayer.DataPackage.Repositories;
    using BusinessLayer.DataPackage.Repositories.Models;
    using BusinessLayer.PriceEntry.Repositories.Models;
    using BusinessLayer.PriceEntry.Services;
    using BusinessLayer.PriceEntry.ValueObjects;
    using BusinessLayer.PriceSeriesSelection.Services;

    using Infrastructure.Attributes.BusinessAnnotations;
    using Infrastructure.MongoDB.Models;
    using Infrastructure.Services.AuditInfoService;
    using Infrastructure.Services.Workflow;

    using ContentBlock = BusinessLayer.ContentBlock.Repositories.Models.ContentBlock;
    using Version = BusinessLayer.ContentBlock.ValueObjects.Version;

    [DomainService]
    public class DataPackageDomainService : IDataPackageDomainService
    {
        private readonly IAuditInfoService auditInfoService;

        private readonly ICommentaryDomainService commentaryDomainService;

        private readonly IDataPackageMetadataDomainService dataPackageMetadataDomainService;

        private readonly IDataPackageRepository dataPackageRepository;

        private readonly IPriceSeriesItemsDomainService priceSeriesItemsDomainService;

        private readonly IPriceSeriesService priceSeriesService;

        private readonly IContentPackageGroupDomainService contentPackageGroupDomainService;

        public DataPackageDomainService(
            IDataPackageRepository dataPackageRepository,
            IDataPackageMetadataDomainService dataPackageMetadataDomainService,
            ICommentaryDomainService commentaryDomainService,
            IAuditInfoService auditInfoService,
            IPriceSeriesService priceSeriesService,
            IPriceSeriesItemsDomainService priceSeriesItemsDomainService,
            IContentPackageGroupDomainService contentPackageGroupDomainService)
        {
            this.dataPackageRepository = dataPackageRepository;
            this.dataPackageMetadataDomainService = dataPackageMetadataDomainService;
            this.commentaryDomainService = commentaryDomainService;
            this.auditInfoService = auditInfoService;
            this.priceSeriesService = priceSeriesService;
            this.priceSeriesItemsDomainService = priceSeriesItemsDomainService;
            this.contentPackageGroupDomainService = contentPackageGroupDomainService;
        }

        public async Task<DataPackage> BuildDataPackage(DataPackageKey dataPackageKey, ContentBlock contentBlock)
        {
            var dataPackageId = await dataPackageMetadataDomainService.GetDataPackageId(dataPackageKey);

            var commentary = await GetCommentary(dataPackageKey.ContentBlockId, dataPackageKey.AssessedDateTime);

            var sequenceId = await contentPackageGroupDomainService.GetSequenceId(contentBlock.ContentBlockId, contentBlock.Version);

            var auditInfo = auditInfoService.GetAuditInfoForCurrentUser();

            var activePriceSeries = await priceSeriesService.GetActivePriceSeriesByIds(
                                          contentBlock!.GetPriceSeriesIds(),
                                          dataPackageKey.AssessedDateTime);

            var activePriceSeriesIds = activePriceSeries.Select(x => x.Id).ToList();
            var activePriceSeriesItems = await priceSeriesItemsDomainService.GetPriceSeriesItems(
                                             activePriceSeriesIds,
                                             dataPackageKey.AssessedDateTime);

            var dataPackage = new DataPackage
            {
                Id = dataPackageId.Value,
                ContentBlock =
                    new Repositories.Models.ContentBlock
                    {
                        Id = contentBlock.ContentBlockId,
                        Version = contentBlock.Version,
                    },
                AssessedDateTime = dataPackageKey.AssessedDateTime,
                Commentary = commentary,
                PriceSeriesItemGroups = CreatePriceSeriesItemGroups(contentBlock, activePriceSeriesItems),
                Status = WorkflowStatus.Draft.Value,
                Created = auditInfo,
                LastModified = auditInfo,
                SequenceId = sequenceId
            };

            return dataPackage;
        }

        public async Task StatusChanged(DataPackage dataPackage, WorkflowStatus status, AuditInfo auditInfo)
        {
            dataPackage.Status = status.Value;
            dataPackage.LastModified = auditInfo;

            var commentaryBlock = await GetCommentary(dataPackage.ContentBlock.Id, dataPackage.AssessedDateTime);

            if (commentaryBlock != null)
            {
                dataPackage.Commentary = commentaryBlock;
            }

            await Save(dataPackage);
        }

        public async Task CorrectionStarted(
            DataPackageId dataPackageId,
            WorkflowId workflowId,
            WorkflowBusinessKey workflowBusinessKey)
        {
            var dataPackage = await dataPackageRepository.GetDataPackage(dataPackageId);
            var dataPackageWithPendingChanges = CreateDataPackageWithPendingChanges(dataPackage!);
            dataPackageWithPendingChanges.PendingChanges!.Status = WorkflowStatus.CorrectionDraft.Value;
            dataPackageWithPendingChanges.PendingChanges.WorkflowData = new WorkflowData
            {
                Id = workflowId.Value, BusinessKey = workflowBusinessKey.Value
            };
            dataPackageWithPendingChanges.PendingChanges.LastModified = auditInfoService.GetAuditInfoForCurrentUser();
            await Save(dataPackageWithPendingChanges);
        }

        public Task CorrectionCancelled(DataPackageId dataPackageId)
        {
            return dataPackageRepository.SetDataPackagePendingStatusAsNull(dataPackageId);
        }

        public async Task StatusChangedDuringCorrection(
            DataPackage dataPackage,
            WorkflowStatus status,
            AuditInfo auditInfo)
        {
            var commentary = await GetCommentary(dataPackage.ContentBlock.Id, dataPackage.AssessedDateTime);

            await dataPackageRepository.SetDataPackagePendingStatus(
                dataPackage.GetDataPackageId(),
                status,
                auditInfo,
                commentary);
        }

        public async Task CorrectionPublished(DataPackageId dataPackageId, AuditInfo auditInfo)
        {
            var dataPackage = await dataPackageRepository.GetDataPackage(dataPackageId);

            var newItem = dataPackage!.PendingChanges;
            newItem!.LastModified = auditInfo;
            newItem.PendingChanges = null;
            newItem.Status = WorkflowStatus.CorrectionPublished.Value;

            await Save(newItem);
        }

        public Task Save(DataPackage dataPackage)
        {
            return dataPackageRepository.Save(dataPackage);
        }

        public async Task<Commentary?> GetCommentary(string contentBlockId, DateTime assessedDateTime)
        {
            var commentary = await commentaryDomainService.GetCurrentCommentary(contentBlockId, assessedDateTime);
            if (commentary is null)
            {
                return null;
            }

            return new Commentary { CommentaryId = commentary.CommentaryId, Version = commentary.Version };
        }

        public async Task<Version?> GetContentBlockVersionFromDataPackage(string contentBlockId, DateTime assessedDateTime)
        {
            var version = await dataPackageRepository.GetContentBlockVersion(contentBlockId, assessedDateTime);
            return version == null ? null : Version.From(version.Value);
        }

        public async Task<DataPackage?> GetDataPackage(DataPackageKey dataPackageKey)
        {
            var dataPackageId = await dataPackageMetadataDomainService.GetDataPackageId(dataPackageKey);
            return await GetDataPackage(dataPackageId);
        }

        public Task<DataPackage?> GetDataPackage(WorkflowBusinessKey workflowBusinessKey)
        {
            var dataPackageId = new DataPackageId(workflowBusinessKey.Value);
            return GetDataPackage(dataPackageId);
        }

        public async Task<DataPackage?> GetDataPackage(DataPackageId dataPackageId)
        {
            var dataPackage = await dataPackageRepository.GetDataPackage(dataPackageId);
            return dataPackage?.PendingChangesOrOriginal();
        }

        [ExcludeFromCodeCoverage(
            Justification = "Excluding from coverage until LNG is switched to the advanced workflow")]
        public async Task<List<DataPackage>> GetDataPackagesByPriceSeriesItemId(string priceSeriesItemId)
        {
            return await dataPackageRepository.GetDataPackagesByPriceSeriesItemId(priceSeriesItemId);
        }

        private static DataPackage CreateDataPackageWithPendingChanges(DataPackage dataPackageInput)
        {
            var dataPackage = new DataPackage
            {
                Id = dataPackageInput.Id,
                ContentBlock = dataPackageInput.ContentBlock,
                AssessedDateTime = dataPackageInput.AssessedDateTime,
                PriceSeriesItemGroups = dataPackageInput.PriceSeriesItemGroups,
                Status = dataPackageInput.Status,
                WorkflowData = dataPackageInput.WorkflowData,
                Commentary = dataPackageInput.Commentary,
                LastModified = dataPackageInput.LastModified,
                Created = dataPackageInput.Created,
                PendingChanges = dataPackageInput,
                SequenceId = dataPackageInput.SequenceId,
            };
            return dataPackage;
        }

        private static List<PriceSeriesItemGroup> CreatePriceSeriesItemGroups(ContentBlock contentBlock, List<BasePriceItem> activePriceSeriesItems)
        {
            var priceSeriesItemGroups = new List<PriceSeriesItemGroup>();

            if (contentBlock.PriceSeriesGrids?.Count > 0)
            {
                foreach (var priceSeriesGrid in contentBlock.PriceSeriesGrids)
                {
                    priceSeriesItemGroups.Add(
                        new PriceSeriesItemGroup
                        {
                            SeriesItemTypeCode = priceSeriesGrid.SeriesItemTypeCode!,
                            PriceSeriesItemIds = activePriceSeriesItems
                               .Where(x => priceSeriesGrid.PriceSeriesIds!.Contains(x.SeriesId))
                               .Select(x => x.Id)
                               .ToList()
                        });
                }
            }

            return priceSeriesItemGroups;
        }
    }
}
