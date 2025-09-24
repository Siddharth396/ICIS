using Amazon.Extensions.NETCore.Setup;
using Amazon.SQS.Model;
using Microsoft.Extensions.Logging;
using SnD.EventProcessor.Poller.Contracts;
using SnD.EventProcessor.Poller.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SnD.EventProcessor.Poller.Services
{
    public class SQSMessagePoller : IMessagePoller
    {
        private readonly ISQSMessageProcessor _messageProcessor;
        private readonly IPollerConfigHelper _pollerConfigHelper;
        private readonly ISnDEventProcessorClient _sndEventProcessorClient;
        private readonly ILogger<SQSMessagePoller> _logger;

        public SQSMessagePoller(
            ISQSMessageProcessor messageProcessor,
            IPollerConfigHelper pollerConfigHelper,
            ISnDEventProcessorClient sndEventProcessorClient,
            ILogger<SQSMessagePoller> logger)
        {
            _messageProcessor = messageProcessor;
            _pollerConfigHelper = pollerConfigHelper;
            _sndEventProcessorClient = sndEventProcessorClient;
            _logger = logger;
        }
        public async Task PollQueuesAsync(CancellationToken cancellationToken)
        {
            var sndQueueNames = _pollerConfigHelper.GetSnDQueueNames();           
            foreach (string queueName in sndQueueNames)
            { 
                if (string.IsNullOrWhiteSpace(queueName))
                {
                    continue;
                }

                try
                {
                    var pollerConfig = _pollerConfigHelper.GetPollerConfig();
                    await PollQueueAsync(queueName, pollerConfig.MessageBatchSize, cancellationToken);
                }
                catch(ConfigurationException)
                { 
                    // if there is a configuration issue, Poller can't continue so throw the error
                    throw;
                }
                catch (Exception exception)
                {
                    // on all other exceptions, let the poller retry(don't throw the exception)
                    _logger.LogError(exception, $"Error while polling {queueName} queue");
                }
            }
        }

        private async Task PollQueueAsync(string queueName, int batchSize, CancellationToken cancellationToken)
        { 
            var queueUrl = await _messageProcessor.GetQueueUrlAsync(queueName, cancellationToken);
            bool polling = true;
            while (polling)
            {
                var messages = await _messageProcessor.ReceiveMessageAsync(queueUrl, batchSize, cancellationToken);  
                if (messages.Count > 0)
                { 
                    await ProcessMessageBatchAsync(messages, queueUrl, cancellationToken);
                    // if batch size = count then queue may contain more messages
                    polling = messages.Count == batchSize; 
                }
                else
                {
                    // message count is less than batch size so no more messages to process
                    polling = false;
                }
            }
        }

        private async Task ProcessMessageBatchAsync(List<Message> messages, string queueUrl, CancellationToken cancellationToken)
        {
            // var orderedMessages = messages.OrderBy(x => x.MessageAttributes["SentTimestamp"]);
            var correlationID = Guid.NewGuid().ToString();

            foreach (var message in messages)
            {
                if (message == null)
                {
                    continue;
                }

                var sndEvent = GetSnDEventFromMessage(message.Body);
                if (sndEvent == null)
                {
                    _logger.LogError($"Invalid message received. Message: {message.Body}");
                }

                var (eventProcessed, statusCode) = await _sndEventProcessorClient.PostEventToApiAsync(sndEvent, correlationID);
                if (eventProcessed)
                {                    
                    await _messageProcessor.DeleteMessageAsync(queueUrl, message.ReceiptHandle, cancellationToken);
                    _logger.LogInformation($"CorrelationId = {correlationID}. Message ({sndEvent.GetDetails()}) processed successfully.");
                }
                else
                {
                    _logger.LogError($"CorrelationId = {correlationID}. Unable to process message {message.Body}. Http Status Code: {statusCode}.");
                }
            }
        }

        private SnDEvent GetSnDEventFromMessage(string message)
        {
            if (string.IsNullOrEmpty(message)) return null;

            var sqsMessage = JsonSerializer.Deserialize<PublishMessage>(message);

            if (sqsMessage == null) return null;

            var sndEvent = JsonSerializer.Deserialize<SnDEvent>(sqsMessage.SnDMessage);

            if (sndEvent == null && !sndEvent.IsValid()) return null;

            return sndEvent;
        }
    }
}
