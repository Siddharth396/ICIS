namespace Infrastructure.Services.OAuth
{
    using System.Diagnostics.CodeAnalysis;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Identity.Client;

    using Serilog;

    [ExcludeFromCodeCoverage]
    public class TokenAcquisitionHandler : DelegatingHandler
    {
        private const string AuthorizationScheme = "Bearer";

        private readonly IConfidentialClientApplication app;

        private readonly string[] scopes;

        private readonly ILogger logger = Log.ForContext<TokenAcquisitionHandler>();

        public TokenAcquisitionHandler(IConfidentialClientApplication app, string[] scopes)
        {
            this.app = app;
            this.scopes = scopes;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            logger.Information("Acquiring access token from OAuth server.");

            var result = await app.AcquireTokenForClient(scopes).ExecuteAsync(cancellationToken);

            logger.Information($"Access token acquired. It will expire on {result.ExpiresOn}");

            request.Headers.Authorization = new AuthenticationHeaderValue(AuthorizationScheme, result.AccessToken);

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
