namespace BusinessLayer.DTO
{
    public record SaveContentBlockResponse
    {
        public string ContentBlockId { get; set; } = default!;
        public string Version { get; set; } = default!;
    }
}
