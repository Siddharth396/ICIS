using Amazon.SQS.Model;
using SnD.EventProcessor.Poller.Tests.Infrastructure;
using System;
using System.Threading.Tasks;
using Xunit;

namespace SnD.EventProcessor.Poller.Tests.UnitTests.SQSMessageProcessorTests
{
    public partial class SQSMessageProcessorTests
    {
        public class GetQueueUrlAsyncTests : IClassFixture<SQSMessageProcessorProvider>
        {
            private readonly SQSMessageProcessorProvider _messageProcessorProvider;

            public GetQueueUrlAsyncTests(SQSMessageProcessorProvider messageProcessorProvider)
            {
                _messageProcessorProvider = messageProcessorProvider;
            }

            [Fact]
            public async Task Get_queue_url_should_return_valid_url_given_correct_queue_name()
            {
                // Arrange
                var getQueueUrlResponse = new GetQueueUrlResponse() 
                { 
                    HttpStatusCode = System.Net.HttpStatusCode.OK, 
                    QueueUrl = _messageProcessorProvider.TestQueueUrl 
                };
                var sqsClient = _messageProcessorProvider.GetSQSClientForGetQueueUrl(getQueueUrlResponse);
                var messageProcessor = _messageProcessorProvider.GetMessageProcessor(sqsClient.Object);

                // Act
                var result = await messageProcessor.GetQueueUrlAsync(
                    _messageProcessorProvider.TestQueueName, 
                    _messageProcessorProvider.CancellationToken);

                // Assert
                Assert.NotNull(result);
                Assert.NotEmpty(result);
                Assert.Equal(_messageProcessorProvider.TestQueueUrl, result);
            }

            [Fact]
            public async Task Get_queue_url_should_throw_when_queue_name_is_invalid()
            {
                // Arrange
                var geQueueUrlResponse = new GetQueueUrlResponse()
                {
                    HttpStatusCode = System.Net.HttpStatusCode.OK,
                    QueueUrl = _messageProcessorProvider.TestQueueUrl
                };
                var sqsClient = _messageProcessorProvider.GetSQSClientForGetQueueUrl(geQueueUrlResponse);
                var messageProcessor = _messageProcessorProvider.GetMessageProcessor(sqsClient.Object);

                // Act
                // Assert
                var result = await Assert.ThrowsAsync<ArgumentException>( async () 
                    => await messageProcessor.GetQueueUrlAsync("invalid_queue_name", _messageProcessorProvider.CancellationToken));
            }

            [Fact]
            public async Task Get_queue_url_should_throw_when_invaid_status_code_is_returned()
            {
                // Arrange
                var geQueueUrlResponse = new GetQueueUrlResponse()
                {
                    HttpStatusCode = System.Net.HttpStatusCode.BadRequest,
                    QueueUrl = _messageProcessorProvider.TestQueueUrl
                };
                var sqsClient = _messageProcessorProvider.GetSQSClientForGetQueueUrl(geQueueUrlResponse);
                var messageProcessor = _messageProcessorProvider.GetMessageProcessor(sqsClient.Object);


                // Act
                // Assert
                var result = await Assert.ThrowsAsync<ArgumentException>(async ()
                   => await messageProcessor.GetQueueUrlAsync(
                       _messageProcessorProvider.TestQueueName, 
                       _messageProcessorProvider.CancellationToken));
            }
        }
    }
}
