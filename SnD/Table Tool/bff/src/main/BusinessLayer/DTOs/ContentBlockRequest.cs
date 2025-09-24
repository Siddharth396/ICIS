namespace BusinessLayer.DTOs
{
    public record ContentBlockRequest
    {
        public string ContentBlockId { get; set; } = default!;
        public string Version { get; set; } = default!;
    }
}
