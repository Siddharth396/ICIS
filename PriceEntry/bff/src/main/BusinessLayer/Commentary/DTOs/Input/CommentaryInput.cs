namespace BusinessLayer.Commentary.DTOs.Input
{
    using System;

    public class CommentaryInput
    {
        public required string ContentBlockId { get; set; }

        public required string CommentaryId { get; set; }

        public required string Version { get; set; }

        public required DateTime AssessedDateTime { get; set; }

        public string? OperationType { get; set; }
    }
}
