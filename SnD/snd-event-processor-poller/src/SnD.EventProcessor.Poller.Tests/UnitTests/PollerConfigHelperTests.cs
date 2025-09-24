using Xunit;
using SnD.EventProcessor.Poller.Tests.Infrastructure;
using System.Collections.Generic;
using Amazon.Extensions.NETCore.Setup;
using SnD.EventProcessor.Poller.Constants;
using Microsoft.Extensions.Configuration;

namespace SnD.EventProcessor.Poller.Tests.UnitTests
{
    public class PollerConfigHelperTests: IClassFixture<TestPollerConfigHelperProvider>
    {
        private readonly TestPollerConfigHelperProvider _pollerConfigHelperProvider;

        public PollerConfigHelperTests(TestPollerConfigHelperProvider pollerConfigHelperProvider)
        {
            _pollerConfigHelperProvider = pollerConfigHelperProvider;
        }

        [Fact]
        public void Get_poller_config_should_return_poller_config()
        {
            // Arrange
            var inMemoryPollerConfig = new Dictionary<string, string>
            {
                {Messages.PollerConfigNames.MaxInitialDelayInSeconds, "1"},
                {Messages.PollerConfigNames.IntervalInSeconds, "5"},
                {Messages.PollerConfigNames.MessageBatchSize, "10"}
            };
            var pollerConfigHelper = _pollerConfigHelperProvider.CreatePollerConfigHelper(inMemoryPollerConfig);

            // Act
            var result = pollerConfigHelper.GetPollerConfig();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(TestPollerConfig.PollerConfigWithFiveSecondsInterval.MaxInitialDelayInSeconds, result.MaxInitialDelayInSeconds);
            Assert.Equal(TestPollerConfig.PollerConfigWithFiveSecondsInterval.IntervalInSeconds, result.IntervalInSeconds);
            Assert.Equal(TestPollerConfig.PollerConfigWithFiveSecondsInterval.MessageBatchSize, result.MessageBatchSize);
        }

        [Theory]
        [MemberData(nameof(GetPollerConfigTestData))]
        public void Get_poller_config_should_return_error_when_missing_configuration(Dictionary<string, string> config, string errorMessage)
        {
            // Arrange
            var pollerConfigHelper = _pollerConfigHelperProvider.CreatePollerConfigHelper(config);

            // Act
            // Assert
            var exception = Assert.Throws<ConfigurationException>(() => pollerConfigHelper.GetPollerConfig());
            Assert.Contains(errorMessage, exception.Message);
        }

        [Fact]
        public void Get_sqs_config_should_return_sqs_config()
        {
            // Arrange            
            var inMemoryPollerConfig = new Dictionary<string, string>
            {
                {Messages.PollerConfigNames.SQSAccessKey, "TEST_ACCESS_KEY"},
                {Messages.PollerConfigNames.SQSSecret, "TEST_SECRET"},
                {Messages.PollerConfigNames.SQSRegion, "euwest-1"},
                {Messages.PollerConfigNames.SQSMessageVisibilityTimeout,"1"},
                {Messages.PollerConfigNames.SQSMessageWaitTime, "1"},
                {Messages.PollerConfigNames.SQSRequestTimeout, "10"},
                {Messages.PollerConfigNames.SQSMaxRetryCount, "3"},
            };
            var pollerConfigHelper = _pollerConfigHelperProvider.CreatePollerConfigHelper(inMemoryPollerConfig);

            // Act
            var result = pollerConfigHelper.GetSqsConfig();

            // Assert
            Assert.NotNull(result);
            Assert.Equal("TEST_ACCESS_KEY", result.SQSAccessKey);
            Assert.Equal("TEST_SECRET", result.SQSSecret);
            Assert.Equal("euwest-1", result.SQSRegion);
            Assert.Equal(1, result.MessageVisibilityTimeout);
            Assert.Equal(1, result.MessageWaitTime);
            Assert.Equal(10, result.RequestTimeOutInSeconds);
            Assert.Equal(3, result.MaxRetryCount);
        }

        [Theory]
        [MemberData(nameof(GetSQSConfigTestData))]
        public void Get_sqs_config_should_return_error_when_missing_configuration(Dictionary<string, string> config, string errorMessage)
        {
            // Arrange
            var pollerConfigHelper = _pollerConfigHelperProvider.CreatePollerConfigHelper(config);

            // Act
            // Assert
            var exception = Assert.Throws<ConfigurationException>(() => pollerConfigHelper.GetSqsConfig());
            Assert.Contains(errorMessage, exception.Message);
        }

        [Fact]
        public void Get_snd_queue_names_should_return_queue_names()
        {
            // Arrange
            var queueNames = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("SnDQueueNames:0", "Queue-1"),
                new KeyValuePair<string, string>("SnDQueueNames:1", "Queue-2"),
                new KeyValuePair<string, string>("SnDQueueNames:2", "Queue-3")
            };
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(queueNames)
                .Build();
            var pollerConfigHelper = new PollerConfigHelper(configuration);

            // Act
            var result = pollerConfigHelper.GetSnDQueueNames();

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Queue-1", result[0]);
            Assert.Equal("Queue-2", result[1]);
            Assert.Equal("Queue-3", result[2]);
        }

        [Fact]
        public void Get_snd_queue_names_should_throw_when_missing_configuration()
        {
            // Arrange
            var queueNames = new List<KeyValuePair<string, string>> { };
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(queueNames)
                .Build();
            var pollerConfigHelper = new PollerConfigHelper(configuration);

            // Act
            // Assert
            Assert.Throws<ConfigurationException>(() => pollerConfigHelper.GetSnDQueueNames());
        }

        [Fact]
        public void Get_snd_event_processor_api_url_should_return_url()
        {
            // Arrange
            var inMemoryPollerConfig = new Dictionary<string, string>
            {
                {Messages.PollerConfigNames.SnDEventProcessorApiUrl, "Test_URL"}
            };
            var pollerConfigHelper = _pollerConfigHelperProvider.CreatePollerConfigHelper(inMemoryPollerConfig);

            // Act
            var result = pollerConfigHelper.GetSnDEventProcessorApiUrl();

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test_URL", result);
        }

        [Fact]
        public void Get_snd_event_processor_api_url_should_throw_when_missing_configuration()
        {
            // Arrange
            var inMemoryPollerConfig = new Dictionary<string, string> { };
            var pollerConfigHelper = _pollerConfigHelperProvider.CreatePollerConfigHelper(inMemoryPollerConfig);

            // Act
            Assert.Throws<ConfigurationException>(() => pollerConfigHelper.GetSnDEventProcessorApiUrl());
        }

        public static IEnumerable<object[]> GetPollerConfigTestData()
        {
            var configWithoutMaxInitialDelay = new Dictionary<string, string>
            {
                {Messages.PollerConfigNames.IntervalInSeconds, "5"},
                {Messages.PollerConfigNames.MessageBatchSize, "10"}
            };

            var configWithoutInterval = new Dictionary<string, string>
            {
                {Messages.PollerConfigNames.MaxInitialDelayInSeconds, "1"},
                {Messages.PollerConfigNames.MessageBatchSize, "10"}
            };

            var configWithoutBatchSize = new Dictionary<string, string>
            {
                {Messages.PollerConfigNames.MaxInitialDelayInSeconds, "1"},
                {Messages.PollerConfigNames.IntervalInSeconds, "5"}
            };

            var emptyConfig = new Dictionary<string, string>()
            {
                {Messages.PollerConfigNames.PollerConfig, ""},
            };

            yield return new object[] { configWithoutMaxInitialDelay, Messages.ConfigErrorMessages.MissingPollerMaxInitialDelay };
            yield return new object[] { configWithoutInterval, Messages.ConfigErrorMessages.MissingPollerInterval };
            yield return new object[] { configWithoutBatchSize, Messages.ConfigErrorMessages.MissingPollerMessageBatchSize };
            yield return new object[] { emptyConfig, Messages.ConfigErrorMessages.MissingConfiguration };

        }

        public static IEnumerable<object[]> GetSQSConfigTestData()
        {
            var configWithoutRegion = new Dictionary<string, string>
            { 
                {Messages.PollerConfigNames.SQSMessageVisibilityTimeout,"1"},
                {Messages.PollerConfigNames.SQSMessageWaitTime, "1"},
                {Messages.PollerConfigNames.SQSRequestTimeout, "10"},
                {Messages.PollerConfigNames.SQSMaxRetryCount, "3"}
            };

            var configWithoutMessageVisibilityTimeout = new Dictionary<string, string>
            {
                {Messages.PollerConfigNames.SQSAccessKey, "TEST_ACCESS_KEY"},
                {Messages.PollerConfigNames.SQSSecret, "TEST_SECRET"},
                {Messages.PollerConfigNames.SQSRegion, "euwest-1"},
                {Messages.PollerConfigNames.SQSMessageWaitTime, "1"},
                {Messages.PollerConfigNames.SQSRequestTimeout, "10"},
                {Messages.PollerConfigNames.SQSMaxRetryCount, "3"}
            };

            var configWithoutMessageWaitTime = new Dictionary<string, string>
            { 
                {Messages.PollerConfigNames.SQSRegion, "euwest-1"},
                {Messages.PollerConfigNames.SQSMessageVisibilityTimeout,"1"},
                {Messages.PollerConfigNames.SQSRequestTimeout, "10"},
                {Messages.PollerConfigNames.SQSMaxRetryCount, "3"}
            };

            var configWithoutSQSRequestTimeout = new Dictionary<string, string>
            {
                {Messages.PollerConfigNames.SQSRegion, "euwest-1"},
                {Messages.PollerConfigNames.SQSMessageVisibilityTimeout,"1"},
                {Messages.PollerConfigNames.SQSMessageWaitTime, "1"},
                {Messages.PollerConfigNames.SQSMaxRetryCount, "3"}
            };

            var configWithoutSQSMaxRetryCount = new Dictionary<string, string>
            { 
                {Messages.PollerConfigNames.SQSRegion, "euwest-1"},
                {Messages.PollerConfigNames.SQSMessageVisibilityTimeout,"1"},
                {Messages.PollerConfigNames.SQSMessageWaitTime, "1"},
                {Messages.PollerConfigNames.SQSRequestTimeout, "10"}
            };

            var emptyConfig = new Dictionary<string, string>()
            {
                {Messages.PollerConfigNames.PollerConfig, ""},
            };

            yield return new object[] { configWithoutRegion, Messages.ConfigErrorMessages.MissingSQSRegion };
            yield return new object[] { configWithoutMessageVisibilityTimeout, Messages.ConfigErrorMessages.MissingSQSMessageVisibilityTimeout };
            yield return new object[] { configWithoutMessageWaitTime, Messages.ConfigErrorMessages.MissingSQSMessageWaitTime };
            yield return new object[] { configWithoutSQSRequestTimeout, Messages.ConfigErrorMessages.MissingSQSRequestTimeout };
            yield return new object[] { configWithoutSQSMaxRetryCount, Messages.ConfigErrorMessages.MissingSQSRetryCount };
            yield return new object[] { emptyConfig, Messages.ConfigErrorMessages.MissingConfiguration };
        }
    }
}
