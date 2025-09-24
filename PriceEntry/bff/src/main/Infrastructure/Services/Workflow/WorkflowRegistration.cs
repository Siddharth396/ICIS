namespace Infrastructure.Services.Workflow
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Net.Http;

    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using Polly;

    [ExcludeFromCodeCoverage]
    public static class WorkflowRegistration
    {
        public static IServiceCollection RegisterWorkflowServices(this IServiceCollection services)
        {
            services.AddHttpClient<IDataPackageWorkflowService, DataPackageWorkflowService>((provider, client) =>
                {
                    var workflowSettings = provider.GetRequiredService<WorkflowSettings>();
                    client.Timeout = workflowSettings.Timeout;
                })
               .AddPolicyHandler(HttpRetryPolicy);

            services.AddOptions<WorkflowSettings>()
               .BindConfiguration(WorkflowSettings.ConfigurationSectionName)
               .ValidateDataAnnotations()
               .ValidateOnStart();

            services.AddSingleton<WorkflowSettings>(
                provider => provider.GetRequiredService<IOptions<WorkflowSettings>>().Value);

            return services;
        }

        private static IAsyncPolicy<HttpResponseMessage> HttpRetryPolicy(IServiceProvider services, HttpRequestMessage requestMessage)
        {
            var settings = services.GetRequiredService<WorkflowSettings>();
            return HttpRetryPolicyFactory.CreateRetryPolicy<DataPackageWorkflowService>(settings.MaxRetries);
        }
    }
}
