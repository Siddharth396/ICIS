namespace BusinessLayer.ContentBlock.DTOs.Output
{
    using System.Collections.Generic;

    public class ContentBlockSaveResponse
    {
        public bool IsValid => ErrorCodes?.Count == 0;

        public required string ContentBlockId { get; set; }

        public required int Version { get; set; }

        public List<string>? ErrorCodes { get; set; }
    }
}