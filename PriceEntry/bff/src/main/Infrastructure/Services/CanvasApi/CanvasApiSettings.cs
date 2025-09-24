namespace Infrastructure.Services.CanvasApi
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class CanvasApiSettings
    {
        public const string ConfigurationSectionName = "Canvas";

        [Required]
        [Url]
        public required string BaseUrl { get; set; }

        [Required]
        public required string ContentPackagesEndpoint { get; set; }

        [Required]
        public required string ReviewUrlPath { get; set; }

        [Required]
        public required string VersionEndpoint { get; set; }

        [Required]
        public required TimeSpan Timeout { get; set; }

        [Required]
        [Range(1, 10)]
        public required int MaxRetries { get; set; }

        [Required]
        public required string WorkspaceId { get; set; }

        [Required]
        public required bool Enabled { get; set; }

        public string GetContentPackagesEndpointFullUrl() => GetEndpointFullUrl(ContentPackagesEndpoint);

        public string GetVersionEndpointFullUrl() => GetEndpointFullUrl(VersionEndpoint);

        private string GetEndpointFullUrl(string endpoint) => $"{BaseUrl.TrimEnd('/')}/{endpoint.TrimStart('/')}";
    }
}
