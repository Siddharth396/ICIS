using Amazon.SQS;
using Amazon.SQS.Model;
using Moq;
using SnD.EventProcessor.Poller.Contracts;
using SnD.EventProcessor.Poller.Model;
using SnD.EventProcessor.Poller.Services;
using System.Threading;
using System.Threading.Tasks;

namespace SnD.EventProcessor.Poller.Tests.Infrastructure
{
    public class SQSMessageProcessorProvider
    {
        public string TestQueueName => "test_queue_name";
        public string TestQueueUrl => "test_queue_url";
        public CancellationToken CancellationToken => new CancellationToken();

        public ISQSMessageProcessor GetMessageProcessor(IAmazonSQS mockSQSClient)
        {
            var sqsConfig = new SQSConfig { MessageWaitTime = 10, MessageVisibilityTimeout = 10 };
            var mockPollerConfigHelper = new Mock<IPollerConfigHelper>();
            mockPollerConfigHelper.Setup(x => x.GetSqsConfig()).Returns(sqsConfig);

            var mockSQSClientService = new Mock<ISQSClientService>();
            mockSQSClientService.Setup(cs => cs.GetClient()).Returns(mockSQSClient);
            var messageProcessor = new SQSMessageProcessor(mockSQSClientService.Object, mockPollerConfigHelper.Object);

            return messageProcessor;
        }

        public Mock<IAmazonSQS> GetSQSClientForGetQueueUrl(GetQueueUrlResponse response)
        {
            var mockSQSClient = new Mock<IAmazonSQS>();
            mockSQSClient.Setup(x => x.GetQueueUrlAsync(TestQueueName, CancellationToken))
                .Returns(Task.FromResult(response));

            return mockSQSClient;
        }

        public Mock<IAmazonSQS> GetSQSClientForReceiveMessage(ReceiveMessageResponse response)
        {
            var mockSQSClient = new Mock<IAmazonSQS>();            
            mockSQSClient.Setup(x => x.ReceiveMessageAsync(It.IsAny<ReceiveMessageRequest>(), CancellationToken))
                .Returns(Task.FromResult(response));

            return mockSQSClient;
        }

        public Mock<IAmazonSQS> GetSQSClientForDeleteMessage(DeleteMessageRequest request, DeleteMessageResponse response)
        {
            var mockSQSClient = new Mock<IAmazonSQS>();
            mockSQSClient.Setup(x => x.DeleteMessageAsync(request.QueueUrl, request.ReceiptHandle, CancellationToken))
                .Returns(Task.FromResult(response));

            return mockSQSClient;
        }
    }
}
