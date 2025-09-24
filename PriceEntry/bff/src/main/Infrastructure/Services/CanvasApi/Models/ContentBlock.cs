namespace Infrastructure.Services.CanvasApi.Models
{
    public class ContentBlock
    {
        public CapabilityConfig? CapabilityConfig { get; set; }

        public required Config[] Config { get; set; }

        public string? DisplayMode { get; set; }

        public required string Id { get; set; }

        public bool? IsValid { get; set; }

        public long? LastPublishedDate { get; set; }

        public required string Name { get; set; }

        public required Tag[] Tags { get; set; }

        public required string Version { get; set; }

        public required string SequenceId { get; set; }
    }
}
