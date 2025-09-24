namespace Infrastructure.Services
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;

    using Polly;

    using Serilog;

    [ExcludeFromCodeCoverage]
    public static class HttpRetryPolicyFactory
    {
        public static IAsyncPolicy<HttpResponseMessage> CreateRetryPolicy<T>(int maxRetries)
        {
            var serviceName = typeof(T).Name;
            var flowContext = $"{serviceName}Request";
            var timeoutScenario = $"{serviceName}Timeout";
            var errorScenario = $"{serviceName}Error";
            var retryScenario = $"{serviceName}Retry";

            return Policy<HttpResponseMessage>
               .Handle<HttpRequestException>()
               .Or<TaskCanceledException>()
               .OrResult(r => r.StatusCode == HttpStatusCode.RequestTimeout || (int)r.StatusCode >= 500)
               .WaitAndRetryAsync(
                    maxRetries,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(3, retryAttempt)),
                    (
                        outcome,
                        span,
                        retryAttempt,
                        context) =>
                    {
                        var logger = Log.ForContext("Flow", flowContext);
                        var exception = outcome.Exception;

                        if (outcome.Result != null)
                        {
                            logger = logger
                               .ForContext("StatusCode", outcome.Result.StatusCode)
                               .ForContext("Message", outcome.Result.ReasonPhrase);
                        }

                        if (retryAttempt >= maxRetries)
                        {
                            if (exception is TaskCanceledException)
                            {
                                logger
                                   .ForContext("Scenario", timeoutScenario)
                                   .Error(
                                        exception,
                                        "Request to {ServiceName} timed out after {RetryAttempt} retries.",
                                        serviceName,
                                        retryAttempt);
                            }
                            else
                            {
                                logger
                                   .ForContext("Scenario", errorScenario)
                                   .Error(
                                        exception,
                                        "Failed to make a request to {ServiceName} after {RetryAttempt} retries.",
                                        serviceName,
                                        retryAttempt);
                            }
                        }
                        else
                        {
                            if (exception is TaskCanceledException)
                            {
                                logger
                                   .ForContext("Scenario", timeoutScenario)
                                   .Warning(
                                        exception,
                                        "Request to {ServiceName} timed out. Retry attempt: {RetryAttempt}",
                                        serviceName,
                                        retryAttempt);
                            }
                            else
                            {
                                logger
                                   .ForContext("Scenario", retryScenario)
                                   .Warning(
                                        exception,
                                        "Retried failed request to {ServiceName}. Retry attempt: {RetryAttempt}",
                                        serviceName,
                                        retryAttempt);
                            }
                        }
                    });
        }
    }
}
