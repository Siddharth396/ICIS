namespace Infrastructure.Services.CanvasApi
{
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Threading.Tasks;

    using Infrastructure.Services.CanvasApi.Models;

    using Serilog;
    using Serilog.Context;

    public class CanvasApiService : ICanvasApiService
    {
        private readonly HttpClient httpClient;

        private readonly ILogger logger;

        private readonly CanvasApiSettings settings;

        public CanvasApiService(HttpClient httpClient, CanvasApiSettings settings, ILogger logger)
        {
            this.logger = logger.ForContext<CanvasApiService>();
            this.httpClient = httpClient;
            this.settings = settings;
        }

        public async Task<bool> SendContentPackage(ContentPackageRequestModel model)
        {
            if (!settings.Enabled)
            {
                logger.Warning("Canvas API is disabled, skipping sending content package to Canvas API.");
                return true;
            }

            using (LogContext.PushProperty("ContentPackageId", model?.ContentPackage?.ContentPackageId))
            {
                logger.Debug("Preparing to make request to Canvas API");

                var response = await httpClient.PostAsJsonAsync(
                                   settings.GetContentPackagesEndpointFullUrl(),
                                   model);

                logger.Debug($"Request to Canvas API Done, status code is {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }

                logger.ForContext("Flow", "CanvasApiServiceRequest")
                   .ForContext("Scenario", "CanvasApiFailedRequest")
                   .ForContext("StatusCode", response.StatusCode)
                   .ForContext("Reason", response.ReasonPhrase)
                   .Error("Failed to send content package to Canvas API.");

                return false;
            }
        }
    }
}
