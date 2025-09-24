namespace BusinessLayer.DataPackage.Models
{
    public enum DataPackageStatusChangeResult
    {
        None = 0,

        Success = 1,

        NotFound = 2,

        InvalidStatus = 3,

        PendingChangesNotFound = 4,

        CorrectionFailed = 5
    }
}
