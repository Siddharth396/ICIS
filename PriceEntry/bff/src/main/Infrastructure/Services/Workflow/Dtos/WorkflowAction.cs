namespace Infrastructure.Services.Workflow.Dtos
{
    using System.Text.Json.Serialization;

    public class WorkflowAction
    {
        [JsonPropertyName("isAllowed")]
        public bool IsAllowed { get; set; }

        [JsonPropertyName("key")]
        public required string Key { get; set; }

        [JsonPropertyName("value")]
        public required string Value { get; set; }
    }
}
