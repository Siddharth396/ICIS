namespace BusinessLayer.DTO
{
    public record ValidationResponse
    {
        public bool Status { get; init; } = default!;
        public string ValidationMessage { get; init; } = default!;
        public string StatusCode { get; init; } = default!;
    }
}
