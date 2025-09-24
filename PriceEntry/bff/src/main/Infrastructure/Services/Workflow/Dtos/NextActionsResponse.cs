namespace Infrastructure.Services.Workflow.Dtos
{
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    public class NextActionsResponse
    {
        [JsonPropertyName("nextActions")]
        public required List<WorkflowAction> NextActions { get; set; }
    }
}
