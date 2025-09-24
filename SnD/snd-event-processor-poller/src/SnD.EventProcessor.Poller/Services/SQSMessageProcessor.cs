using Amazon.SQS.Model;
using SnD.EventProcessor.Poller.Contracts;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SnD.EventProcessor.Poller.Services
{
    public class SQSMessageProcessor : ISQSMessageProcessor
    {
        private readonly ISQSClientService _sqsClientService;
        private readonly IPollerConfigHelper _configHelper;

        public SQSMessageProcessor(ISQSClientService sqsClientService, IPollerConfigHelper configHelper)
        {
            _sqsClientService = sqsClientService;
            _configHelper = configHelper;
        }

        public async Task<string> GetQueueUrlAsync(string queueName, CancellationToken cancellationToken)
        {
            var client = _sqsClientService.GetClient();
            var response = await client.GetQueueUrlAsync(queueName, cancellationToken);
            if (response != null && response.HttpStatusCode == HttpStatusCode.OK)
            {
                return response.QueueUrl;
            }

            throw new ArgumentException($"Unable to get Queue URL for {queueName}. Http Status Code: {response?.HttpStatusCode}");
        }

        public async Task<List<Message>> ReceiveMessageAsync(string queueUrl, int messageBatchSize, CancellationToken cancellationToken)
        {
            var client = _sqsClientService.GetClient();
            var sqsConfig = _configHelper.GetSqsConfig();
            var request = new ReceiveMessageRequest
            {
                QueueUrl = queueUrl,
                MaxNumberOfMessages = messageBatchSize,
                WaitTimeSeconds = sqsConfig.MessageWaitTime,
                VisibilityTimeout = sqsConfig.MessageVisibilityTimeout
            };
            var response = await client.ReceiveMessageAsync(request, cancellationToken);
            if (response != null && response.HttpStatusCode == HttpStatusCode.OK)
            {
                return response.Messages;
            }

            throw new ArgumentException($"Unable to Receive messages from SQS Queue ({queueUrl}. Http Status Code: {response?.HttpStatusCode}");
        }

        public async Task DeleteMessageAsync(string queueUrl, string recieptHandle, CancellationToken cancellationToken)
        {
            var client = _sqsClientService.GetClient();
            var response = await client.DeleteMessageAsync(queueUrl, recieptHandle, cancellationToken);
            if (response == null || response.HttpStatusCode != HttpStatusCode.OK)
            {
                throw new ArgumentException($"Unable to delete message(handle: {recieptHandle}) from SQS Queue(url: {queueUrl}). Http Status Code: {response?.HttpStatusCode}");
            }
        }
    }
}
