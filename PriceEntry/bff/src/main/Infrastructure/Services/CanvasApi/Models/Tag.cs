namespace Infrastructure.Services.CanvasApi.Models
{
    public class Tag
    {
        public required string TagId { get; set; }

        public required string Type { get; set; }

        public string? Category { get; set; }
    }
}