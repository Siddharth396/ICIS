namespace Infrastructure.Services.OAuth
{
    using System.ComponentModel.DataAnnotations;

    public class OAuthOptions
    {
        public const string ConfigurationSectionName = "OAuth";

        [Url]
        public required string Authority { get; set; }

        public required string ClientId { get; set; }

        public required string ClientSecret { get; set; }

        public required string VersionEndpoint { get; set; }

        public string GetVersionEndpointFullUrl()
        {
            return GetEndpointFullUrl(VersionEndpoint);
        }

        private string GetEndpointFullUrl(string endpoint)
        {
            return $"{Authority.TrimEnd('/')}/{endpoint.TrimStart('/')}";
        }
    }
}
