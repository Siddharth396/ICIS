namespace BusinessLayer.DTO
{
    public record ContentBlockResponse
    {
        public string ContentBlockId { get; set; } = default!;
        public string Version { get; set; } = default!;
        public string Filter { get; set; } = default!;
    }
}
