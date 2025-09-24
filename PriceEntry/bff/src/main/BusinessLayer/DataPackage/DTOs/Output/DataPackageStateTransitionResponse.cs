namespace BusinessLayer.DataPackage.DTOs.Output
{
    public class DataPackageStateTransitionResponse
    {
        public static DataPackageStateTransitionResponse Success => new() { IsSuccess = true };

        public bool IsSuccess { get; private set; }

        public string? ErrorCode { get; private set; }

        public static DataPackageStateTransitionResponse Error(string errorCode) => new() { IsSuccess = false, ErrorCode = errorCode };
    }
}
