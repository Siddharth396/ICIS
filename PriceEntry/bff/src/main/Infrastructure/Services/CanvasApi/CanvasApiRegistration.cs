namespace Infrastructure.Services.CanvasApi
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Net.Http;

    using Infrastructure.Services.OAuth;

    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using Microsoft.Identity.Client;

    using Polly;

    [ExcludeFromCodeCoverage]
    public static class CanvasApiRegistration
    {
        public static void RegisterCanvasApiServices(this IServiceCollection services)
        {
            services
               .AddOptions<CanvasApiSettings>()
               .BindConfiguration(CanvasApiSettings.ConfigurationSectionName)
               .ValidateDataAnnotations()
               .ValidateOnStart();

            services.AddSingleton<CanvasApiSettings>(
                provider => provider.GetRequiredService<IOptions<CanvasApiSettings>>().Value);

            services
               .AddHttpClient<ICanvasApiService, CanvasApiService>(
                    (provider, client) =>
                    {
                        var settings = provider.GetRequiredService<CanvasApiSettings>();
                        client.Timeout = settings.Timeout;
                        client.DefaultRequestHeaders.Add("workspaceId", settings.WorkspaceId);
                    })
               .AddPolicyHandler(HttpRetryPolicy)
               .AddHttpMessageHandler(
                    provider =>
                    {
                        var app = provider.GetRequiredService<IConfidentialClientApplication>();
                        return new TokenAcquisitionHandler(app, ["all"]);
                    });
        }

        private static IAsyncPolicy<HttpResponseMessage> HttpRetryPolicy(
            IServiceProvider services,
            HttpRequestMessage requestMessage)
        {
            var settings = services.GetRequiredService<CanvasApiSettings>();
            return HttpRetryPolicyFactory.CreateRetryPolicy<CanvasApiService>(settings.MaxRetries);
        }
    }
}
