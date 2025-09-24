namespace BusinessLayer.PriceDisplay.ContentBlock.DTOs.Output
{
    using System.Collections.Generic;

    public class ContentBlockSaveResponseForDisplay
    {
        public required string ContentBlockId { get; set; }

        public required int Version { get; set; }

        public List<string>? ErrorCodes { get; set; }
    }
}
