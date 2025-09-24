namespace BusinessLayer.PriceEntry.Services
{
    using System.Threading.Tasks;

    using BusinessLayer.DataPackage.Services;
    using BusinessLayer.PriceEntry.ValueObjects;

    using Infrastructure.Attributes.BusinessAnnotations;
    using Infrastructure.Services.Workflow;

    [ApplicationService]
    public class PriceSeriesItemEditabilityService : IPriceSeriesItemEditabilityService
    {
        private readonly IDataPackageDomainService dataPackageDomainService;

        public PriceSeriesItemEditabilityService(IDataPackageDomainService dataPackageDomainService)
        {
            this.dataPackageDomainService = dataPackageDomainService;
        }

        public async Task<bool> IsPriceSeriesItemEditable(
            DataPackageKey dataPackageKey,
            bool isReviewMode,
            string? priceSeriesItemStatus)
        {
            var dataPackage = await dataPackageDomainService.GetDataPackage(dataPackageKey);
            var workflowStatus = dataPackage == null ? WorkflowStatus.None : new WorkflowStatus(dataPackage.Status);

            var isPriceSeriesItemEditable = workflowStatus == WorkflowStatus.None
                                            || (isReviewMode
                                                    ? workflowStatus.IsCopyEditorAllowedToEdit()
                                                    : workflowStatus.IsEditorAllowedToEdit());

            if (WorkflowStatus.IsCorrectionPrePublishStatus(workflowStatus))
            {
                return isPriceSeriesItemEditable;
            }

            return !WorkflowStatus.IsPublishedOrCorrectionPublishedStatusMatch(priceSeriesItemStatus)
                   && isPriceSeriesItemEditable;
        }
    }
}
