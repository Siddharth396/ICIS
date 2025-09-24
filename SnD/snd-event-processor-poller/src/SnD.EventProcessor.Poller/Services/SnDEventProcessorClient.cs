using SnD.EventProcessor.Poller.Contracts;
using SnD.EventProcessor.Poller.Model;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text;
using static System.Net.Mime.MediaTypeNames;
using System.Net;
using System;
using SnD.EventProcessor.Poller.Constants;
using System.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http.Headers;

namespace SnD.EventProcessor.Poller.Services
{
    public class SnDEventProcessorClient : ISnDEventProcessorClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IPollerConfigHelper _pollerConfigHelper;
        private readonly ILogger<SnDEventProcessorClient> _logger;
        private readonly IAuthorizationService _authorizationService;

        public SnDEventProcessorClient(
            IHttpClientFactory httpClientFactory,
            IPollerConfigHelper pollerConfigHelper,
            ILogger<SnDEventProcessorClient> logger,
            IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
            _httpClientFactory = httpClientFactory;
            _pollerConfigHelper = pollerConfigHelper;
            _logger = logger;
        }
        public async Task<(bool, HttpStatusCode)> PostEventToApiAsync(SnDEvent sndEvent, string correlationID)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("HttpClientWithPolyRetry");
                var headers = httpClient.DefaultRequestHeaders;
                headers.Add("X-Correlation-ID", correlationID);

                var isKP = false;

                string eventProcessorApiUrl = string.Empty;

                if (SnDEntityTypes.EntityTypesToMigrateToKP.Contains(sndEvent.EntityType))
                {
                    isKP = true;
                    eventProcessorApiUrl = _pollerConfigHelper.GetSnDKafkaProducerApiUrl();

                    if (_pollerConfigHelper.IsAuthorizationEnabled())
                    {
                        headers.Add("Authorization", await _authorizationService.GetBearerTokenAsync(correlationID));
                    }
                }
                else
                {
                    eventProcessorApiUrl = _pollerConfigHelper.GetSnDEventProcessorApiUrl();
                }

                var sndEventJson = new StringContent(System.Text.Json.JsonSerializer.Serialize(sndEvent), Encoding.UTF8, Application.Json);
                var result = await httpClient.PostAsync(eventProcessorApiUrl, sndEventJson);

                if (result != null && (result.StatusCode == HttpStatusCode.OK ||
                                       result.StatusCode == HttpStatusCode.Accepted ||
                                       result.StatusCode == HttpStatusCode.NoContent ||
                                       (result.StatusCode == HttpStatusCode.NotFound && isKP) ||
                                       result.StatusCode == HttpStatusCode.MultiStatus ||
                                       result.StatusCode == HttpStatusCode.BadRequest))
                {
                    return (true, result.StatusCode);
                }

                return (false, result.StatusCode);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"CorrelationId = {correlationID}. Unable to send events to SnD Event Processor");
                return (false, HttpStatusCode.InternalServerError);
                // We are swallowing the exception here because we don't want to stop the poller
            }
        }        
    }
}
