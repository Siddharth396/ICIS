namespace BusinessLayer.PriceDisplay.ContentBlock.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using BusinessLayer.PriceDisplay.ContentBlock.DTOs.Input;
    using BusinessLayer.PriceDisplay.ContentBlock.DTOs.Output;

    using Version = BusinessLayer.ContentBlock.ValueObjects.Version;

    public interface IContentBlockServiceForDisplay
    {
        Task<ContentBlockDefinitionForDisplay?> GetContentBlock(string contentBlockId, Version version);

        Task<ContentBlockDefinitionForDisplay> GetContentBlockFromInputParameters(
            List<string> seriesIds,
            DateTime assessedDateTime);

        Task<ContentBlockSaveResponseForDisplay> SaveContentBlock(ContentBlockForDisplayInput contentBlockInput);
    }
}
