using Amazon;
using Amazon.Runtime;
using Amazon.SQS;
using Microsoft.Extensions.Configuration;
using SnD.EventProcessor.Poller.Contracts;
using System;

namespace SnD.EventProcessor.Poller.Services
{
    public class SQSClientService : ISQSClientService
    {
        private readonly IPollerConfigHelper _configHelper;

        public SQSClientService(IPollerConfigHelper configHelper)
        {
            _configHelper = configHelper;
        }

        public IAmazonSQS GetClient()
        {
            var sqsConfig = _configHelper.GetSqsConfig();

            var region = RegionEndpoint.GetBySystemName(sqsConfig.SQSRegion);
            var clientConfig = new AmazonSQSConfig
            {
                RegionEndpoint = region,
                Timeout = TimeSpan.FromSeconds(sqsConfig.RequestTimeOutInSeconds),
                RetryMode = RequestRetryMode.Standard,
                // Retry when
                // 1. AWS service is being throttled,
                // 2. The request times out or
                // 3. The HTTP connection fails
                MaxErrorRetry = sqsConfig.MaxRetryCount
            };

            AmazonSQSClient sqsClient;
<<<<<<< Updated upstream
=======
            if (aspnetCoreEnvironment == "development" || dotnetEnvironment == "development")
            {
                clientConfig.ServiceURL = sqsConfig.ServiceUrl;
                clientConfig.UseHttp = true;
                clientConfig.AuthenticationRegion = region.SystemName;
            }
>>>>>>> Stashed changes

            sqsClient = new AmazonSQSClient(clientConfig);

            return sqsClient;
        }
    }
}
