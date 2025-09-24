namespace BusinessLayer.DTO
{
    public record SaveContentBlockRequest
    {
        public string ContentBlockId { get; set; } = default!;
        public string Filter { get; set; } = default!;
    }
}

