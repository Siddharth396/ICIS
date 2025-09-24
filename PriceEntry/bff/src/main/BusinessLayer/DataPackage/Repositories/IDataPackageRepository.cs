namespace BusinessLayer.DataPackage.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using BusinessLayer.DataPackage.Models;
    using BusinessLayer.DataPackage.Repositories.Models;

    using Infrastructure.MongoDB.Models;
    using Infrastructure.Services.Workflow;

    public interface IDataPackageRepository
    {
        Task Save(DataPackage dataPackage);

        Task<DataPackage?> GetDataPackage(DataPackageId dataPackageId);

        Task<List<DataPackage>> GetDataPackagesByPriceSeriesItemId(string priceSeriesItemId);

        Task<int?> GetContentBlockVersion(string contentBlockId, DateTime assessedDateTime);

        Task SetDataPackagePendingStatus(
            DataPackageId dataPackageId,
            WorkflowStatus status,
            AuditInfo auditInfo,
            Commentary? commentary);

        Task SetDataPackagePendingStatusAsNull(DataPackageId dataPackageId);
    }
}