using Amazon.Extensions.NETCore.Setup;
using Microsoft.Extensions.Configuration;
using SnD.EventProcessor.Poller.Constants;
using SnD.EventProcessor.Poller.Contracts;
using SnD.EventProcessor.Poller.Model;
using System.Text;

namespace SnD.EventProcessor.Poller
{
    public class PollerConfigHelper : IPollerConfigHelper
    {
        private readonly IConfiguration _configuration;

        public PollerConfigHelper(IConfiguration configuration)
        {           
            _configuration = configuration;
        }

        public SQSConfig GetSqsConfig()
        {
            SQSConfig config = _configuration.GetSection(SQSConfig.Name).Get<SQSConfig>();

            if (config == null)
            {                
                throw new ConfigurationException(Messages.ConfigErrorMessages.MissingConfiguration);
            }

            var messageBuilder = new StringBuilder();

            if (string.IsNullOrWhiteSpace(config.SQSRegion))
            {
                messageBuilder.AppendLine(Messages.ConfigErrorMessages.MissingSQSRegion);
            }

            if(config.MessageWaitTime <= 0)
            {
                messageBuilder.AppendLine(Messages.ConfigErrorMessages.MissingSQSMessageWaitTime);
            }

            if(config.MessageVisibilityTimeout <= 0)
            {
                messageBuilder.AppendLine(Messages.ConfigErrorMessages.MissingSQSMessageVisibilityTimeout);
            }

            if (config.RequestTimeOutInSeconds <= 0)
            {
                messageBuilder.AppendLine(Messages.ConfigErrorMessages.MissingSQSRequestTimeout);
            }

            if (config.MaxRetryCount <= 0)
            {
                messageBuilder.AppendLine(Messages.ConfigErrorMessages.MissingSQSRetryCount);
            }

            var message = messageBuilder.ToString();
            if (string.IsNullOrWhiteSpace(message))
            {
                return config;
            }
            
            throw new ConfigurationException(message);
        }

        public PollerConfig GetPollerConfig()
        {
            PollerConfig config = _configuration.GetSection(PollerConfig.Name).Get<PollerConfig>();
           
            if (config == null)
            {                
                throw new ConfigurationException(Messages.ConfigErrorMessages.MissingConfiguration);
            }

            var messageBuilder = new StringBuilder();
            if (config.IntervalInSeconds <= 0)
            {
                messageBuilder.AppendLine(Messages.ConfigErrorMessages.MissingPollerInterval);
            }

            if (config.MaxInitialDelayInSeconds <= 0)
            {
                messageBuilder.AppendLine(Messages.ConfigErrorMessages.MissingPollerMaxInitialDelay);
            }

            if (config.MessageBatchSize <= 0)
            {
                messageBuilder.AppendLine(Messages.ConfigErrorMessages.MissingPollerMessageBatchSize);
            }

            var message = messageBuilder.ToString();
            if (string.IsNullOrWhiteSpace(message))
            {
                return config;
            }

            throw new ConfigurationException(message);
        }

        public string[] GetSnDQueueNames()
        {
            string[] sndQueueNames = _configuration.GetSection("SnDQueueNames").Get<string[]>();

            if (sndQueueNames == null || sndQueueNames.Length == 0)
            {
                throw new ConfigurationException(Messages.ConfigErrorMessages.MissingSQSQueueNames);
            }

            return sndQueueNames;
        }

        public string GetSnDEventProcessorApiUrl()
        {
            var apiUrl = _configuration.GetValue<string>("SnDEventProcessorApiUrl");
            if (string.IsNullOrWhiteSpace(apiUrl))
            {
                throw new ConfigurationException(Messages.ConfigErrorMessages.MissingEventProcessorApiUrl);
            }

            return apiUrl;
        }
        public string GetSnDKafkaProducerApiUrl()
        {
            var apiUrl = _configuration.GetValue<string>("SnDKafkaProducerApiUrl");
            if (string.IsNullOrWhiteSpace(apiUrl))
            {
                throw new ConfigurationException(Messages.ConfigErrorMessages.MissingKafkaProducerApiUrl);
            }

            return apiUrl;
        }

        public AuthorizationRequestDetail GetDetailsForAuthorization()
        {
            var authorizationRequestDetails = _configuration.GetSection(nameof(AuthorizationRequestDetail)).Get<AuthorizationRequestDetail>();
            
            return authorizationRequestDetails ?? throw new ConfigurationException(Messages.ConfigErrorMessages.MissingAuthorizationRequestDetails);
        }

        public bool IsAuthorizationEnabled()
        {
            return _configuration.GetValue<bool>("EnableAuthorization");
        }
    }
}
