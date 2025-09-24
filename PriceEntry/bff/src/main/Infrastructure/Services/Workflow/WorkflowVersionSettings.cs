namespace Infrastructure.Services.Workflow
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class WorkflowVersionSettings
    {
        [Required]
        public required string Default { get; set; }

        public Dictionary<string, string> Overrides { get; set; } = new();

        public string GetVersion(string version) => Overrides.GetValueOrDefault(version.ToUpper(), Default);
    }
}
