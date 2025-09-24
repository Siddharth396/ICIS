namespace BusinessLayer.ContentBlock.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using BusinessLayer.ContentBlock.DTOs.Input;
    using BusinessLayer.ContentBlock.DTOs.Output;

    using Version = BusinessLayer.ContentBlock.ValueObjects.Version;

    public interface IContentBlockService
    {
        Task<ContentBlockDefinition?> GetContentBlock(
            string contentBlockId,
            Version version,
            DateTime assessedDateTime,
            bool isReviewMode);

        Task<ContentBlockSaveResponse> SaveContentBlock(ContentBlockInput contentBlockInput);

        Task<List<string>> GetContentBlockIds(string priceSeriesId);
    }
}
