namespace Infrastructure.Services.Workflow.Dtos
{
    using System.Collections.Generic;

    public class TransitionToNextStateRequest
    {
        public required string BusinessKey { get; set; }

        public Dictionary<string, object>? Variables { get; set; }
    }
}
