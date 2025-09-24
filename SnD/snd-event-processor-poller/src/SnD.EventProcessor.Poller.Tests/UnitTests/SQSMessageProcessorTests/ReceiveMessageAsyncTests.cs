using Amazon.SQS.Model;
using SnD.EventProcessor.Poller.Tests.Infrastructure;
using System;
using System.Threading.Tasks;
using Xunit;

namespace SnD.EventProcessor.Poller.Tests.UnitTests.SQSMessageProcessorTests
{
    public partial class SQSMessageProcessorTests
    {
        public class ReceiveMessageAsyncTests: IClassFixture<SQSMessageProcessorProvider>
        {
            private readonly SQSMessageProcessorProvider _messageProcessorProvider;

            public ReceiveMessageAsyncTests(SQSMessageProcessorProvider messageProcessorProvider)
            {
                _messageProcessorProvider = messageProcessorProvider;
            }

            [Fact]
            public async Task Receive_message_should_return_messages()
            {
                // Arrange
                var response = new ReceiveMessageResponse
                {
                    HttpStatusCode = System.Net.HttpStatusCode.OK,
                    Messages = { 
                        new Message { MessageId = "TestMessageId", ReceiptHandle = "TestReceiptHandle", Body = "Test Body" },
                        new Message { MessageId = "TestMessageId-2", ReceiptHandle = "TestReceiptHandle-2", Body = "Test Body-2" }
                    }
                };

                int batchSize = 2;

                var sqsClient = _messageProcessorProvider.GetSQSClientForReceiveMessage(response);
                var messageProcessor = _messageProcessorProvider.GetMessageProcessor(sqsClient.Object);

                // Act
                var result = await messageProcessor.ReceiveMessageAsync(
                    _messageProcessorProvider.TestQueueUrl,
                    batchSize, 
                    _messageProcessorProvider.CancellationToken);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(response.Messages.Count, result.Count);
                Assert.Equal(response.Messages[0], response.Messages[0]);
            }

            [Fact]
            public async Task Receive_message_should_throw_when_sqs_client_returns_invalid_status_code()
            {
                // Arrange
                var response = new ReceiveMessageResponse
                {
                    HttpStatusCode = System.Net.HttpStatusCode.BadRequest,
                    Messages = { 
                        new Message { MessageId = "TestMessageId", ReceiptHandle = "TestReceiptHandle", Body = "Test Body" },
                    }
                };

                int batchSize = 1;

                var sqsClient = _messageProcessorProvider.GetSQSClientForReceiveMessage(response);
                var messageProcessor = _messageProcessorProvider.GetMessageProcessor(sqsClient.Object);

                // Act
                // Assert
                var result = await Assert.ThrowsAsync<ArgumentException>(async ()
                  => await messageProcessor.ReceiveMessageAsync(
                     _messageProcessorProvider.TestQueueUrl,
                      batchSize,
                      _messageProcessorProvider.CancellationToken));
                
            }
        }
    }
}
