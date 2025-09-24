namespace Infrastructure.Services.PeriodGenerator
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class PeriodGeneratorSettings
    {
        public const string ConfigurationSectionName = "PeriodGeneratorService";

        [Required]
        [Url]
        public required string BaseUrl { get; set; }

        [Required]
        public required string PeriodsEndpoint { get; set; }

        [Required]
        public required string SchedulesEndpoint { get; set; }

        [Required]
        public required string VersionEndpoint { get; set; }

        [Required]
        public required TimeSpan Timeout { get; set; }

        [Required]
        [Range(1, 10)]
        public required int MaxRetries { get; set; }

        public string GetPeriodsEndpointFullUrl() => GetEndpointFullUrl(PeriodsEndpoint);

        public string GetVersionEndpointFullUrl() => GetEndpointFullUrl(VersionEndpoint);

        public string GetSchedulesEndpointFullUrl(string scheduleId)
        {
            return GetEndpointFullUrl(SchedulesEndpoint).Replace("{scheduleId}", scheduleId);
        }

        private string GetEndpointFullUrl(string endpoint) => $"{BaseUrl.TrimEnd('/')}/{endpoint.TrimStart('/')}";
    }
}
