namespace BusinessLayer.DataPackage.DTOs.Output
{
    public class ContentBlockInError
    {
        public required string ContentBlockId { get; set; }

        public required string Error { get; set; }
    }
}
