namespace Infrastructure.Services.CanvasApi.Models
{
    public class ContentPackage
    {
        public required long AppliesFrom { get; set; }

        public required string ContentPackageId { get; set; }

        public required Contents Contents { get; set; }

        public required string CreatedBy { get; set; }

        public required long CreatedOn { get; set; }

        public Config[]? Metadata { get; set; }

        public required string ModifiedBy { get; set; }

        public required long ModifiedOn { get; set; }

        public string? PublishedBy { get; set; }

        public long? PublishedOn { get; set; }

        public required int Revision { get; set; }

        public required string Status { get; set; }

        public required Tag[] Tags { get; set; }

        public required Title Title { get; set; }

        public required int Version { get; set; }

        public required string SequenceId { get; set; }
    }
}
