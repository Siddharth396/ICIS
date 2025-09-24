using Newtonsoft.Json;
using SnD.EventProcessor.Poller.Contracts;
using SnD.EventProcessor.Poller.Model;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System;

namespace SnD.EventProcessor.Poller.Services
{
    public class AuthorizationService : IAuthorizationService
    {
        private static DateTime expiryTime = default;
        private static string bearerToken = "";

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IPollerConfigHelper _pollerConfigHelper;
        private readonly ILogger<AuthorizationService> _logger;

        public AuthorizationService(
            IHttpClientFactory httpClientFactory,
            IPollerConfigHelper pollerConfigHelper,
            ILogger<AuthorizationService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _pollerConfigHelper = pollerConfigHelper;
            _logger = logger;
        }

        public async Task<string> GetBearerTokenAsync(string correlationID)
        {
            if (DateTime.UtcNow <= expiryTime || Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "dev")
            {
                return bearerToken;
            }

            var authorizationToken = await GetAuthTokenAsync(correlationID);

            expiryTime = DateTime.Now.AddSeconds(authorizationToken.ExpiresIn).ToUniversalTime();

            _logger.LogInformation($"CorrelationId = {correlationID}. The bearer token will expire at: {expiryTime}");

            bearerToken = $"{authorizationToken.TokenType} {authorizationToken.AccessToken}";

            return bearerToken;
        }

        private async Task<AuthorizationTokenMetadata> GetAuthTokenAsync(string correlationID)
        {
            var httpClient = _httpClientFactory.CreateClient("HttpClientWithPolyRetry");

            _logger.LogInformation($" CorrelationId = {correlationID}. Getting Authorization Token.");

            var authorizationRequestDetail = _pollerConfigHelper.GetDetailsForAuthorization();

            HttpContent content = new FormUrlEncodedContent(
                                           new List<KeyValuePair<string, string>>()
                                           {
                                                new KeyValuePair<string, string>("grant_type", authorizationRequestDetail.GrantType),
                                                new KeyValuePair<string, string>("client_id", authorizationRequestDetail.AuthClientId.Trim()),
                                                new KeyValuePair<string, string>("client_secret", authorizationRequestDetail.AuthClientSecret.Trim())
                                           });

            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

            var response = await httpClient.PostAsync(authorizationRequestDetail.DomainUrl, content);
            var statusCode = response.StatusCode;

            var responseBody = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(responseBody))
            {
                _logger.LogError($"CorrelationId: {correlationID}. StatusCode: {statusCode}, Authorization token metadata is null.");
                throw new Exception();
            }

            var authorizationTokenMetadata = JsonConvert.DeserializeObject<AuthorizationTokenMetadata>(responseBody);

            if (string.IsNullOrWhiteSpace(authorizationTokenMetadata.AccessToken))
            {
                _logger.LogError($"CorrelationId = {correlationID}. StatusCode: {statusCode}, AccessToken is null.");
                throw new Exception();
            }

            _logger.LogInformation($"CorrelationId = {correlationID}, Authorization token metadata received.");

            return authorizationTokenMetadata;
        }
    }
}
