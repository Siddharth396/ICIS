namespace BusinessLayer.DataPackage.DTOs.Output
{
    using System.Collections.Generic;

    public class CreateDataPackageResponse
    {
        public CreateDataPackageResponse()
        {
            IsSuccess = true;
            ContentBlocksInError = new List<ContentBlockInError>();
        }

        public bool IsSuccess { get; private set; }

        public List<ContentBlockInError> ContentBlocksInError { get; }

        public void AddContentBlockError(string contentBlockId, string error)
        {
            IsSuccess = false;

            ContentBlocksInError.Add(new ContentBlockInError
            {
                ContentBlockId = contentBlockId,
                Error = error
            });
        }

        public void Merge(CreateDataPackageResponse? other)
        {
            if (other is null)
            {
                return;
            }

            IsSuccess = IsSuccess && other.IsSuccess;

            ContentBlocksInError.AddRange(other.ContentBlocksInError);
        }
    }
}
