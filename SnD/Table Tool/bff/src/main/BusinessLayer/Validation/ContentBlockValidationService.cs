namespace BusinessLayer.Validation
{
    using BusinessLayer.DTO;
    using BusinessLayer.DTOs;

    using Common.Constants;
    using Common.Helpers;

    public class ContentBlockValidationService : IContentBlockValidationService
    {
        public ValidationResponse ValidateContentBlockInput(SaveContentBlockRequest contentBlockInput)
        {

            if (IsContentBlockInputInvalid(contentBlockInput))
            {
                return SendValidationResponse(false);
            }

            return SendValidationResponse(true);
        }

        public ValidationResponse ValidateContentBlockRequest(ContentBlockRequest contentBlockInput)
        {

            if (IsContentBlockRequestInvalid(contentBlockInput))
            {
                return SendValidationResponse(false);
            }

            return SendValidationResponse(true);
        }

        private bool IsContentBlockInputInvalid(SaveContentBlockRequest contentBlockInput)
        {
            return string.IsNullOrWhiteSpace(contentBlockInput.ContentBlockId) ||
                string.IsNullOrWhiteSpace(contentBlockInput.Filter) ||
                !Helpers.IsJsonValid(contentBlockInput.Filter);
        }

        private bool IsContentBlockRequestInvalid(ContentBlockRequest contentBlockInput)
        {
            return string.IsNullOrWhiteSpace(contentBlockInput.ContentBlockId) ||
                string.IsNullOrWhiteSpace(contentBlockInput.Version);
        }

        private ValidationResponse SendValidationResponse(bool status)
        {
            return new ValidationResponse
            {
                Status = status,
                ValidationMessage = status ? string.Empty : ErrorMessages.BadRequestMessage,
                StatusCode = status ? string.Empty : StatusCode.BadRequest
            };
        }
    }

}
