using Amazon.SQS.Model;
using Moq;
using SnD.EventProcessor.Poller.Tests.Infrastructure;
using System;
using System.Threading.Tasks;
using Xunit;

namespace SnD.EventProcessor.Poller.Tests.UnitTests.SQSMessageProcessorTests
{
    public partial class SQSMessageProcessorTests
    {
        public class DeleteMessageAsyncTests : IClassFixture<SQSMessageProcessorProvider>
        {
            private readonly SQSMessageProcessorProvider _messageProcessorProvider;

            public DeleteMessageAsyncTests(SQSMessageProcessorProvider messageProcessorProvider)
            {
                _messageProcessorProvider = messageProcessorProvider;
            }

            [Fact]
            public async Task Delete_message_should_delete_a_message_with_valid_handle()
            {
                // Arrange
                var requst = new DeleteMessageRequest
                {
                    QueueUrl = _messageProcessorProvider.TestQueueUrl,
                    ReceiptHandle = "Test_Receipt_Handle"
                };

                var response = new DeleteMessageResponse
                {
                    HttpStatusCode = System.Net.HttpStatusCode.OK
                };

                var sqsClient = _messageProcessorProvider.GetSQSClientForDeleteMessage(requst, response);
                var processor = _messageProcessorProvider.GetMessageProcessor(sqsClient.Object);

                // Act
                await processor.DeleteMessageAsync(
                    _messageProcessorProvider.TestQueueUrl,
                    "Test_Receipt_Handle",
                    _messageProcessorProvider.CancellationToken);

                // Assert
                sqsClient.Verify(c => c.DeleteMessageAsync(
                    requst.QueueUrl, 
                    requst.ReceiptHandle, 
                    _messageProcessorProvider.CancellationToken), Times.AtLeastOnce);
            }

            [Fact]
            public async Task Delete_message_should_throw_when_message_handle_is_invalid()
            {
                // Arrange
                var requst = new DeleteMessageRequest
                {
                    QueueUrl = _messageProcessorProvider.TestQueueUrl,
                    ReceiptHandle = "Test_Receipt_Handle"
                };

                var response = new DeleteMessageResponse
                {
                    HttpStatusCode = System.Net.HttpStatusCode.OK
                };

                var sqsClient = _messageProcessorProvider.GetSQSClientForDeleteMessage(requst, response);
                var processor = _messageProcessorProvider.GetMessageProcessor(sqsClient.Object);

                // Act
                // Assert
                var result = await Assert.ThrowsAsync<ArgumentException>(async () => await processor.DeleteMessageAsync(
                      _messageProcessorProvider.TestQueueUrl,
                      "Invalid_Receipt_Handle",
                      _messageProcessorProvider.CancellationToken));
            }

            [Fact]
            public async Task Delete_message_should_throw_when_queue_url_is_invalid()
            {
                // Arrange
                var requst = new DeleteMessageRequest
                {
                    QueueUrl = _messageProcessorProvider.TestQueueUrl,
                    ReceiptHandle = "Test_Receipt_Handle"
                };

                var response = new DeleteMessageResponse
                {
                    HttpStatusCode = System.Net.HttpStatusCode.OK
                };

                var sqsClient = _messageProcessorProvider.GetSQSClientForDeleteMessage(requst, response);
                var processor = _messageProcessorProvider.GetMessageProcessor(sqsClient.Object);

                // Act
                // Assert
                var result = await Assert.ThrowsAsync<ArgumentException>(async () => await processor.DeleteMessageAsync(
                      "Invalid_Queue_Url",
                      "Test_Receipt_Handle",
                      _messageProcessorProvider.CancellationToken));
            }
        }
    }
}
