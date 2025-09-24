namespace SnD.EventProcessor.Poller.Constants
{
    public class Messages
    {
        public static class ConfigErrorMessages
        {
            public const string MissingConfiguration = "Unable to read SQS configuration from configuration.";
            public const string MissingSQSAccessKey = "Unable to read SQS Access Key from configuration.";
            public const string MissingSQSSecret = "Unable to read SQS Access secret from configuration.";
            public const string MissingSQSRegion = "Unable to read SQS Region from configuration.";
            public const string MissingSQSMessageWaitTime = "Unable to read Message Wait Time from configuration.";
            public const string MissingSQSMessageVisibilityTimeout = "Unable to read Message Visibility Timeout from configuration.";
            public const string MissingSQSRequestTimeout = "Unable to read SQS Request Timeout from configuration.";
            public const string MissingSQSRetryCount = "Unable to read SQS Request max retry count from configuration.";
            public const string MissingSQSQueueNames = "Unable to read SnD SQS Queue names from Configuration.";
            public const string MissingPollerMaxInitialDelay = "Unable to read SnD Poller Maximum Initial Delay from configuration.";
            public const string MissingPollerInterval = "Unable to read SnD Poller Interval from configuration.";
            public const string MissingPollerMessageBatchSize = "Unable to read SnD Poller Message Batch Size from configuration.";
            public const string MissingEventProcessorApiUrl = "Unable to read SnD Event Processor API Url from configuration";
            public const string MissingKafkaProducerApiUrl = "Unable to read Kafka Producer API Url from configuration";
            public const string MissingAuthorizationRequestDetails = "Unable to read authorization request detail from configuration";

        }

        public static class PollerConfigNames
        {
            public const string PollerConfig = "PollerConfig";
            public const string IntervalInSeconds = "PollerConfig:IntervalInSeconds";
            public const string MaxInitialDelayInSeconds = "PollerConfig:MaxInitialDelayInSeconds";
            public const string MessageBatchSize = "PollerConfig:MessageBatchSize";
            public const string SQSAccessKey = "SQSConfig:SQSAccessKey";
            public const string SQSSecret = "SQSConfig:SQSSecret";
            public const string SQSRegion = "SQSConfig:SQSRegion";
            public const string SQSMessageWaitTime = "SQSConfig:MessageWaitTime";
            public const string SQSMessageVisibilityTimeout = "SQSConfig:MessageVisibilityTimeout";
            public const string SQSRequestTimeout = "SQSConfig:RequestTimeOutInSeconds";
            public const string SQSMaxRetryCount = "SQSConfig:MaxRetryCount";
            public const string SnDQueueNames = "SnDQueueNames";
            public const string SnDEventProcessorApiUrl = "SnDEventProcessorApiUrl";
        }
    }
}
