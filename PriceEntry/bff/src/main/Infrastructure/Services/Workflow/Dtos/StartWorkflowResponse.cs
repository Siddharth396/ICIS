namespace Infrastructure.Services.Workflow.Dtos
{
    using System.Text.Json.Serialization;

    public class StartWorkflowResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
    }
}
