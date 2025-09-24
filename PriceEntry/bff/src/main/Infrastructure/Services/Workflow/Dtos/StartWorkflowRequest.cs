namespace Infrastructure.Services.Workflow.Dtos
{
    using System.Collections.Generic;

    public class StartWorkflowRequest
    {
        public required string BusinessKey { get; set; }

        public required string ProcessDefinitionKey { get; set; }

        public Dictionary<string, object>? Variables { get; set; }
    }
}
