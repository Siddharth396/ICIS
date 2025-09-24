namespace Infrastructure.Services.Workflow
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public class WorkflowSettings
    {
        public const string ConfigurationSectionName = "Workflow";

        [Required]
        [Url]
        public required string BaseUrl { get; set; }

        [Required]
        public required string StartWorkflowEndpoint { get; set; }

        [Required]
        public required string StateTransitionEndpoint { get; set; }

        [Required]
        public required string VersionEndpoint { get; set; }

        [Required]
        public required string NextActionsEndpoint { get; set; }

        [Required]
        public required TimeSpan Timeout { get; set; }

        [Required]
        [Range(1, 10)]
        public required int MaxRetries { get; set; }

        [Required]
        public required string WorkspaceId { get; set; }

        [Required]
        public required string ProcessDefinitionKey { get; set; }

        [Required]
        public required WorkflowVersionSettings Versions { get; set; }

        [Required]
        public required bool WorkflowCorrectionToggle { get; set; }

        [Required]
        public required bool ShowRepublishButtonToggle { get; set; }

        public string GetStartWorkflowEndpointFullUrl() => GetEndpointFullUrl(StartWorkflowEndpoint);

        public string GetStateTransitionEndpointFullUrl() => GetEndpointFullUrl(StateTransitionEndpoint);

        public string GetVersionEndpointFullUrl() => GetEndpointFullUrl(VersionEndpoint);

        public string GetNextActionsEndpointFullUrl(string businessKey)
        {
            return GetEndpointFullUrl(NextActionsEndpoint.Replace("{businessKey}", businessKey));
        }

        private string GetEndpointFullUrl(string endpoint) => $"{BaseUrl.TrimEnd('/')}/{endpoint.TrimStart('/')}";
    }
}
