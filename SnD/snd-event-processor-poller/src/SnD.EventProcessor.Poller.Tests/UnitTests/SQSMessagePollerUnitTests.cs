using Amazon.Extensions.NETCore.Setup;
using Microsoft.Extensions.Logging;
using Moq;
using SnD.EventProcessor.Poller.Contracts;
using SnD.EventProcessor.Poller.Model;
using SnD.EventProcessor.Poller.Services;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace SnD.EventProcessor.Poller.Tests.UnitTests
{
    public class SQSMessagePollerUnitTests: IClassFixture<MessagePollerData>
    {
        private readonly MessagePollerData _messagePollerData;

        public SQSMessagePollerUnitTests(MessagePollerData messagePollerData)
        {
            _messagePollerData = messagePollerData;
        }

        [Fact]
        public async Task Poll_queue_async_Should_process_valid_messages()
        {
            // Arrange
            var mockPollCofigHelper = new Mock<IPollerConfigHelper>();
            mockPollCofigHelper.Setup(x => x.GetPollerConfig()).Returns(_messagePollerData.PollerConfig);
            mockPollCofigHelper.Setup(x => x.GetSnDQueueNames()).Returns(_messagePollerData.SndQueueNames);

            var mockMessageProcessor = new Mock<ISQSMessageProcessor>();
            mockMessageProcessor.Setup(x => x.GetQueueUrlAsync(It.IsAny<string>(), _messagePollerData.Token))
                .Returns(Task.FromResult(_messagePollerData.TestQueueUrl));
            mockMessageProcessor.Setup(x => x.ReceiveMessageAsync(
                _messagePollerData.TestQueueUrl, 
                _messagePollerData.BatchSize, 
                _messagePollerData.Token))
                .Returns(Task.FromResult(_messagePollerData.MessageList));

            var mockSnDEventProcessorClient = new Mock<ISnDEventProcessorClient>();
            mockSnDEventProcessorClient.Setup(x => x.PostEventToApiAsync(It.IsAny<SnDEvent>(), It.IsAny<string>()))
                .Returns(Task.FromResult((true, HttpStatusCode.OK)));

            var mockLogger = new Mock<ILogger<SQSMessagePoller>>();

            var poller = new SQSMessagePoller(
                mockMessageProcessor.Object,
                mockPollCofigHelper.Object,
                mockSnDEventProcessorClient.Object,
                mockLogger.Object);

            // Act
            await poller.PollQueuesAsync(_messagePollerData.Token);

            // Assert
            mockPollCofigHelper.Verify(x => x.GetPollerConfig(), Times.Once);
            mockPollCofigHelper.Verify(x => x.GetSnDQueueNames(), Times.Once);
            mockMessageProcessor.Verify(x => x.GetQueueUrlAsync(It.IsAny<string>(), _messagePollerData.Token), Times.Once);
            mockMessageProcessor.Verify(x => x.ReceiveMessageAsync(It.IsAny<string>(), It.IsAny<int>(), _messagePollerData.Token), Times.Once);
            mockMessageProcessor.Verify(x => x.DeleteMessageAsync(It.IsAny<string>(), It.IsAny<string>(), _messagePollerData.Token), Times.Once);
            mockSnDEventProcessorClient.Verify(x => x.PostEventToApiAsync(It.IsAny<SnDEvent>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task Poll_queue_async_should_throw_when_get_poller_config_throws()
        {
            // Arrange
            var mockPollCofigHelper = new Mock<IPollerConfigHelper>();
            mockPollCofigHelper.Setup(x => x.GetPollerConfig()).Throws(new ConfigurationException("configuration exception"));
            mockPollCofigHelper.Setup(x => x.GetSnDQueueNames()).Returns(_messagePollerData.SndQueueNames);

            var mockMessageProcessor = new Mock<ISQSMessageProcessor>();
            var mockSnDEventProcessorClient = new Mock<ISnDEventProcessorClient>();
            var mockLogger = new Mock<ILogger<SQSMessagePoller>>();

            var poller = new SQSMessagePoller(
                mockMessageProcessor.Object,
                mockPollCofigHelper.Object,
                mockSnDEventProcessorClient.Object,
                mockLogger.Object);

            // Act
            // Assert
            await Assert.ThrowsAsync<ConfigurationException>(() => poller.PollQueuesAsync(_messagePollerData.Token));
        }

        [Fact]
        public async Task Poll_queue_async_should_throw_when_get_snd_queuenames_throws()
        {
            // Arrange
            var mockPollCofigHelper = new Mock<IPollerConfigHelper>();
            mockPollCofigHelper.Setup(x => x.GetPollerConfig()).Returns(_messagePollerData.PollerConfig);
            mockPollCofigHelper.Setup(x => x.GetSnDQueueNames()).Throws(new ConfigurationException("configuration exception")); ;

            var mockMessageProcessor = new Mock<ISQSMessageProcessor>();

            var mockSnDEventProcessorClient = new Mock<ISnDEventProcessorClient>();
            var mockLogger = new Mock<ILogger<SQSMessagePoller>>();

            var poller = new SQSMessagePoller(
                mockMessageProcessor.Object,
                mockPollCofigHelper.Object,
                mockSnDEventProcessorClient.Object,
                mockLogger.Object);

            // Act
            // Assert
            await Assert.ThrowsAsync<ConfigurationException>(() => poller.PollQueuesAsync(_messagePollerData.Token));
        }

        [Fact]
        public async Task Poll_queue_async_should_throw_when_get_queue_url_async_throws()
        {
            // Arrange
            var mockPollCofigHelper = new Mock<IPollerConfigHelper>();
            mockPollCofigHelper.Setup(x => x.GetPollerConfig()).Returns(_messagePollerData.PollerConfig);
            mockPollCofigHelper.Setup(x => x.GetSnDQueueNames()).Returns(_messagePollerData.SndQueueNames);

            var mockMessageProcessor = new Mock<ISQSMessageProcessor>();
            mockMessageProcessor.Setup(x => x.GetQueueUrlAsync(It.IsAny<string>(), _messagePollerData.Token))
                   .Throws(new ConfigurationException("configuration exception"));

            var mockSnDEventProcessorClient = new Mock<ISnDEventProcessorClient>();
            var mockLogger = new Mock<ILogger<SQSMessagePoller>>();

            var poller = new SQSMessagePoller(
                mockMessageProcessor.Object,
                mockPollCofigHelper.Object,
                mockSnDEventProcessorClient.Object,
                mockLogger.Object);

            // Act
            // Assert
            await Assert.ThrowsAsync<ConfigurationException>(() => poller.PollQueuesAsync(_messagePollerData.Token));
        }

        [Fact]
        public async Task Poll_queue_async_should_throw_when_receive_messages_async_throws()
        {
            // Arrange
            var mockPollCofigHelper = new Mock<IPollerConfigHelper>();
            mockPollCofigHelper.Setup(x => x.GetPollerConfig()).Returns(_messagePollerData.PollerConfig);
            mockPollCofigHelper.Setup(x => x.GetSnDQueueNames()).Returns(_messagePollerData.SndQueueNames);

            var mockMessageProcessor = new Mock<ISQSMessageProcessor>();
            mockMessageProcessor.Setup(x => x.GetQueueUrlAsync(It.IsAny<string>(), _messagePollerData.Token))
                    .Returns(Task.FromResult(_messagePollerData.TestQueueUrl));
            mockMessageProcessor.Setup(x => x.ReceiveMessageAsync(
                _messagePollerData.TestQueueUrl,
                _messagePollerData.BatchSize,
                _messagePollerData.Token))
                .Throws(new ConfigurationException("configuration exception"));

            var mockSnDEventProcessorClient = new Mock<ISnDEventProcessorClient>();
            var mockLogger = new Mock<ILogger<SQSMessagePoller>>();

            var poller = new SQSMessagePoller(
                mockMessageProcessor.Object,
                mockPollCofigHelper.Object,
                mockSnDEventProcessorClient.Object,
                mockLogger.Object);

            // Act
            // Assert
            await Assert.ThrowsAsync<ConfigurationException>(() => poller.PollQueuesAsync(_messagePollerData.Token));
        }

        [Fact]
        public async Task Poll_queue_async_should_throw_when_delete_message_async_throws()
        {
            // Arrange
            var mockPollCofigHelper = new Mock<IPollerConfigHelper>();
            mockPollCofigHelper.Setup(x => x.GetPollerConfig()).Returns(_messagePollerData.PollerConfig);
            mockPollCofigHelper.Setup(x => x.GetSnDQueueNames()).Returns(_messagePollerData.SndQueueNames);

            var mockMessageProcessor = new Mock<ISQSMessageProcessor>();
            mockMessageProcessor.Setup(x => x.GetQueueUrlAsync(It.IsAny<string>(), _messagePollerData.Token))
                    .Returns(Task.FromResult(_messagePollerData.TestQueueUrl));
            mockMessageProcessor.Setup(x => x.ReceiveMessageAsync(
                _messagePollerData.TestQueueUrl,
                _messagePollerData.BatchSize,
                _messagePollerData.Token))
                .Returns(Task.FromResult(_messagePollerData.MessageList));
            mockMessageProcessor.Setup(x => x.DeleteMessageAsync(
                _messagePollerData.TestQueueUrl, 
                "test_receipt_handle", 
                _messagePollerData.Token))
                .Throws(new ConfigurationException("configuration exception"));

            var mockSnDEventProcessorClient = new Mock<ISnDEventProcessorClient>();
            mockSnDEventProcessorClient.Setup(x => x.PostEventToApiAsync(It.IsAny<SnDEvent>(), It.IsAny<string>()))
                .Returns(Task.FromResult((true, HttpStatusCode.OK)));
            var mockLogger = new Mock<ILogger<SQSMessagePoller>>();

            var poller = new SQSMessagePoller(
                mockMessageProcessor.Object,
                mockPollCofigHelper.Object,
                mockSnDEventProcessorClient.Object,
                mockLogger.Object);

            // Act
            // Assert
            await Assert.ThrowsAsync<ConfigurationException>(() => poller.PollQueuesAsync(_messagePollerData.Token));
        }

        [Fact]
        public async Task Poll_queue_async_should_call_delete_message_async_when_snd_event_processor_client_returns_true_status()
        {
            // Arrange
            var mockPollCofigHelper = new Mock<IPollerConfigHelper>();
            mockPollCofigHelper.Setup(x => x.GetPollerConfig()).Returns(_messagePollerData.PollerConfig);
            mockPollCofigHelper.Setup(x => x.GetSnDQueueNames()).Returns(_messagePollerData.SndQueueNames);

            var mockMessageProcessor = new Mock<ISQSMessageProcessor>();
            mockMessageProcessor.Setup(x => x.GetQueueUrlAsync(It.IsAny<string>(), _messagePollerData.Token))
                    .Returns(Task.FromResult(_messagePollerData.TestQueueUrl));
            mockMessageProcessor.Setup(x => x.ReceiveMessageAsync(
                _messagePollerData.TestQueueUrl,
                _messagePollerData.BatchSize,
                _messagePollerData.Token))
                .Returns(Task.FromResult(_messagePollerData.MessageList));

            var mockSnDEventProcessorClient = new Mock<ISnDEventProcessorClient>();
            mockSnDEventProcessorClient.Setup(x => x.PostEventToApiAsync(It.IsAny<SnDEvent>(), It.IsAny<string>()))
                .Returns(Task.FromResult((true, HttpStatusCode.OK)));
            var mockLogger = new Mock<ILogger<SQSMessagePoller>>();

            var poller = new SQSMessagePoller(
                mockMessageProcessor.Object,
                mockPollCofigHelper.Object,
                mockSnDEventProcessorClient.Object,
                mockLogger.Object);

            // Act
            await poller.PollQueuesAsync(_messagePollerData.Token);

            // Assert
            mockMessageProcessor.Verify(x => x.DeleteMessageAsync(_messagePollerData.TestQueueUrl, It.IsAny<string>(), _messagePollerData.Token), Times.Once);
        }

        [Fact]
        public async Task Poll_queue_async_should_not_call_delete_message_async_when_snd_event_processor_client_returns_false_status()
        {
            // Arrange
            var mockPollCofigHelper = new Mock<IPollerConfigHelper>();
            mockPollCofigHelper.Setup(x => x.GetPollerConfig()).Returns(_messagePollerData.PollerConfig);
            mockPollCofigHelper.Setup(x => x.GetSnDQueueNames()).Returns(_messagePollerData.SndQueueNames);

            var mockMessageProcessor = new Mock<ISQSMessageProcessor>();
            mockMessageProcessor.Setup(x => x.GetQueueUrlAsync(It.IsAny<string>(), _messagePollerData.Token))
                    .Returns(Task.FromResult(_messagePollerData.TestQueueUrl));
            mockMessageProcessor.Setup(x => x.ReceiveMessageAsync(
                _messagePollerData.TestQueueUrl,
                _messagePollerData.BatchSize,
                _messagePollerData.Token))
                .Returns(Task.FromResult(_messagePollerData.MessageList));

            var mockSnDEventProcessorClient = new Mock<ISnDEventProcessorClient>();
            mockSnDEventProcessorClient.Setup(x => x.PostEventToApiAsync(It.IsAny<SnDEvent>(), It.IsAny<string>()))
                .Returns(Task.FromResult((false, HttpStatusCode.NotFound)));
            var mockLogger = new Mock<ILogger<SQSMessagePoller>>();

            var poller = new SQSMessagePoller(
                mockMessageProcessor.Object,
                mockPollCofigHelper.Object,
                mockSnDEventProcessorClient.Object,
                mockLogger.Object);

            // Act
            await poller.PollQueuesAsync(_messagePollerData.Token);

            // Assert
            mockMessageProcessor.Verify(x => x.DeleteMessageAsync(_messagePollerData.TestQueueUrl, It.IsAny<string>(), _messagePollerData.Token), Times.Never);
        }
    }
}

