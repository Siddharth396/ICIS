namespace Infrastructure.Services.PeriodGenerator
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
    public static class PeriodGeneratorRegistration
    {
        public static IServiceCollection RegisterPeriodGeneratorServices(this IServiceCollection services)
        {
            services.AddHttpClient<IPeriodGeneratorService, PeriodGeneratorService>(
                (provider, client) =>
                {
                    var periodGeneratorSettings = provider.GetRequiredService<PeriodGeneratorSettings>();
                    client.Timeout = periodGeneratorSettings.Timeout;
                })
               .AddPolicyHandler(HttpRetryPolicy)
               .AddHttpMessageHandler(
                    provider =>
                    {
                        var app = provider.GetRequiredService<IConfidentialClientApplication>();
                        return new TokenAcquisitionHandler(app, scopes: ["all"]);
                    });

            services.AddOptions<PeriodGeneratorSettings>()
               .BindConfiguration(PeriodGeneratorSettings.ConfigurationSectionName)
               .ValidateDataAnnotations()
               .ValidateOnStart();

            services.AddSingleton<PeriodGeneratorSettings>(
                provider => provider.GetRequiredService<IOptions<PeriodGeneratorSettings>>().Value);

            return services;
        }

        private static IAsyncPolicy<HttpResponseMessage> HttpRetryPolicy(IServiceProvider services, HttpRequestMessage requestMessage)
        {
            var settings = services.GetRequiredService<PeriodGeneratorSettings>();
            return HttpRetryPolicyFactory.CreateRetryPolicy<PeriodGeneratorService>(settings.MaxRetries);
        }
    }
}
