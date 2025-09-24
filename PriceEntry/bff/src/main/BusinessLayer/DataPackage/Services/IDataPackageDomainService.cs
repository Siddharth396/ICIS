namespace BusinessLayer.DataPackage.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using BusinessLayer.DataPackage.Models;
    using BusinessLayer.DataPackage.Repositories.Models;
    using BusinessLayer.PriceEntry.ValueObjects;

    using Infrastructure.MongoDB.Models;
    using Infrastructure.Services.Workflow;

    using ContentBlock = BusinessLayer.ContentBlock.Repositories.Models.ContentBlock;
    using Version = BusinessLayer.ContentBlock.ValueObjects.Version;

    public interface IDataPackageDomainService
    {
        Task<DataPackage> BuildDataPackage(DataPackageKey dataPackageKey, ContentBlock contentBlock);

        Task CorrectionCancelled(DataPackageId dataPackageId);

        Task CorrectionPublished(DataPackageId dataPackageId, AuditInfo auditInfo);

        Task CorrectionStarted(
            DataPackageId dataPackageId,
            WorkflowId workflowId,
            WorkflowBusinessKey workflowBusinessKey);

        Task<Version?> GetContentBlockVersionFromDataPackage(string contentBlockId, DateTime assessedDateTime);

        Task<DataPackage?> GetDataPackage(DataPackageKey dataPackageKey);

        Task<DataPackage?> GetDataPackage(WorkflowBusinessKey workflowBusinessKey);

        Task<DataPackage?> GetDataPackage(DataPackageId dataPackageId);

        Task<List<DataPackage>> GetDataPackagesByPriceSeriesItemId(string priceSeriesItemId);

        Task Save(DataPackage dataPackage);

        Task StatusChanged(DataPackage dataPackage, WorkflowStatus status, AuditInfo auditInfo);

        Task StatusChangedDuringCorrection(DataPackage dataPackage, WorkflowStatus status, AuditInfo auditInfo);
    }
}
